using StreamVideo_Server.Network;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using NAudio.Wave; // Thư viện âm thanh

namespace StreamVideo_Server.Winforms
{
    public partial class FormMain : Form
    {
        private TcpServer _server;
        private FilterInfoCollection _filterInfoCollection;
        private VideoCaptureDevice _videoCaptureDevice;

        // Biến xử lý âm thanh
        private WaveInEvent _waveIn;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                _filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (_filterInfoCollection.Count > 0)
                    GhiLog($"Tìm thấy Webcam: {_filterInfoCollection[0].Name}");
                else
                    GhiLog("Không tìm thấy Webcam!");
            }
            catch { }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                _server = new TcpServer(9000);
                _server.OnLog += GhiLog;

                // 1. BẬT WEBCAM
                if (_filterInfoCollection != null && _filterInfoCollection.Count > 0)
                {
                    _videoCaptureDevice = new VideoCaptureDevice(_filterInfoCollection[0].MonikerString);
                    _videoCaptureDevice.NewFrame += Video_NewFrame;
                    _videoCaptureDevice.Start();
                    GhiLog("Đã bật Webcam.");
                }

                // 2. BẬT MICRO (AUDIO)
                if (WaveIn.DeviceCount > 0)
                {
                    _waveIn = new WaveInEvent();
                    _waveIn.DeviceNumber = 0;
                    _waveIn.WaveFormat = new WaveFormat(44100, 1); // 44.1kHz, Mono
                    _waveIn.DataAvailable += Audio_DataAvailable;
                    _waveIn.StartRecording();
                    GhiLog("Đã bật Microphone.");
                }

                lblStatus.Text = "Trạng thái: ĐANG PHÁT (Webcam + Mic)";
                lblStatus.ForeColor = Color.Green;
                btnStart.Enabled = false;
                btnStop.Enabled = true;

                await _server.BatDauAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        // Xử lý HÌNH ẢNH (Gửi loại 2)
        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (_server == null) return;
            try
            {
                using (Bitmap bmp = (Bitmap)eventArgs.Frame.Clone())
                {
                    // Resize 640x480
                    using (Bitmap resized = new Bitmap(bmp, new Size(640, 480)))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            resized.Save(ms, ImageFormat.Jpeg);
                            byte[] imgData = ms.ToArray();
                            _server.BroadcastVideoFrame(imgData);
                        }
                    }
                }
            }
            catch { }
        }

        // Xử lý ÂM THANH (Gửi loại 3)
        private void Audio_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (_server == null) return;
            byte[] audioData = new byte[e.BytesRecorded];
            Array.Copy(e.Buffer, audioData, e.BytesRecorded);
            _server.BroadcastAudio(audioData);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // Tắt Mic
            if (_waveIn != null)
            {
                _waveIn.StopRecording();
                _waveIn.Dispose();
                _waveIn = null;
            }

            // Tắt Cam
            if (_videoCaptureDevice != null && _videoCaptureDevice.IsRunning)
            {
                _videoCaptureDevice.SignalToStop();
                _videoCaptureDevice = null;
            }

            _server?.Dung();
            lblStatus.Text = "Trạng thái: ĐÃ DỪNG";
            lblStatus.ForeColor = Color.Red;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void GhiLog(string msg)
        {
            if (InvokeRequired) { Invoke(new Action<string>(GhiLog), msg); return; }
            lstLog.Items.Add(msg);
        }
    }
}