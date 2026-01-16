using StreamVideo_Server.Network;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using NAudio.Wave;

namespace StreamVideo_Server.Winforms
{
    public partial class FormMain : Form
    {
        private TcpServer _server;
        private FilterInfoCollection _filterInfoCollection;
        private VideoCaptureDevice _videoCaptureDevice;
        private WaveInEvent _waveIn;

        public FormMain()
        {
            InitializeComponent();
        }

        // Bỏ qua Form_Load
        private void FormMain_Load(object sender, EventArgs e) { }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                // --- 1. TÌM KIẾM WEBCAM ---
                _filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (_filterInfoCollection.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy Webcam nào trên máy tính này!\nHãy kiểm tra lại Driver.", "Lỗi Camera");
                }
                else
                {
                    GhiLog($"Tìm thấy Webcam: {_filterInfoCollection[0].Name}");

                    // Khởi động Webcam
                    _videoCaptureDevice = new VideoCaptureDevice(_filterInfoCollection[0].MonikerString);
                    _videoCaptureDevice.NewFrame += Video_NewFrame;
                    _videoCaptureDevice.Start();
                    GhiLog("Đã bật Webcam.");
                }

                // --- 2. KHỞI ĐỘNG SERVER ---
                _server = new TcpServer(9000);
                _server.OnLog += GhiLog;

                // --- 3. BẬT MICRO (AUDIO) ---
                if (WaveIn.DeviceCount > 0)
                {
                    _waveIn = new WaveInEvent();
                    _waveIn.DeviceNumber = 0;
                    _waveIn.WaveFormat = new WaveFormat(44100, 1);
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

        // Xử lý HÌNH ẢNH (Đã tắt Preview để có thể xóa PictureBox)
        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (_server == null) return;
            try
            {
                using (Bitmap originalFrame = (Bitmap)eventArgs.Frame.Clone())
                {
                    // A. HIỂN THỊ LÊN SERVER (PREVIEW)
                   
                    /*try
                    {
                        pbPreview.Invoke(new Action(() =>
                        {
                            if (pbPreview.Image != null) pbPreview.Image.Dispose();
                            pbPreview.Image = (Bitmap)originalFrame.Clone();
                        }));
                    }
                    catch { }*/
                    

                    // B. GỬI CHO CLIENT
                    // Resize về 640x480 để giảm dung lượng mạng
                    using (Bitmap resized = new Bitmap(originalFrame, new Size(640, 480)))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            resized.Save(ms, ImageFormat.Jpeg);
                            byte[] imgData = ms.ToArray();

                            // --- BẮT ĐẦU MÃ HÓA ---
                            byte[] encryptedData = SecurityHelper.Encrypt(imgData);
                            // ----------------------

                            // Gửi dữ liệu đã mã hóa đi
                            _server.BroadcastVideoFrame(encryptedData);
                        }
                    }
                }
            }
            catch { }
        }

        // Xử lý ÂM THANH
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