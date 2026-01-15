using StreamVideo_Server.Network;
using System;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace StreamVideo_Server.Winforms
{
    public partial class FormMain : Form
    {
        private TcpServer _server;

        public FormMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ghi log an toàn từ thread khác
        /// </summary>
        private void GhiLog(string noiDung)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(GhiLog), noiDung);
                return;
            }

            lstLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {noiDung}");
            lstLog.TopIndex = lstLog.Items.Count - 1;
        }

        /// <summary>
        /// Bấm Start Server
        /// </summary>
        private async void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                _server = new TcpServer(9000);
                _server.OnLog += GhiLog;

                await _server.BatDauAsync();

                lblStatus.Text = "Trạng thái: ĐANG CHẠY";
                lblStatus.ForeColor = System.Drawing.Color.Green;

                btnStart.Enabled = false;
                btnStop.Enabled = true;

                GhiLog("Server bắt đầu lắng nghe port 9000");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi start server: " + ex.Message);
            }
            // Bắt đầu timer stream (Giả sử bắt đầu stream ngay khi mở server, hoặc bạn làm nút riêng)
            timerStream.Start();
        }
        // Sự kiện Tick của Timer
        
        private Bitmap CaptureScreen()
        {
            // Chụp toàn màn hình chính
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            // Resize nhỏ lại chút cho nhẹ mạng LAN (Option)
            return new Bitmap(bmp, new Size(800, 450));
        }
        /// <summary>
        /// Bấm Stop Server
        /// </summary>
        private void btnStop_Click(object sender, EventArgs e)
        {
            _server?.Dung();

            lblStatus.Text = "Trạng thái: ĐÃ DỪNG";
            lblStatus.ForeColor = System.Drawing.Color.Red;

            btnStart.Enabled = true;
            btnStop.Enabled = false;

            GhiLog("Server đã dừng");
        }

        private void timerStream_Tick_1(object sender, EventArgs e)
        {
            if (_server == null) return;

            // 1. Chụp màn hình (hoặc lấy từ Camera)
            Bitmap bmp = CaptureScreen();

            // 2. Chuyển sang byte array (JPEG)
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Jpeg);
                byte[] imgData = ms.ToArray();

                // 3. Gọi Server gửi đi
                _server.BroadcastVideoFrame(imgData);
            }
            bmp.Dispose();
        }
    }
}
