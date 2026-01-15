using StreamVideo_Server.Network;
using System;
using System.Windows.Forms;

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
    }
}
