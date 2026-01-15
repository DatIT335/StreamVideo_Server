using StreamVideo_Server.Common;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace StreamVideo_Server.Network
{
    /// <summary>
    /// Lớp TCP Server – chỉ làm nhiệm vụ:
    /// - Lắng nghe kết nối
    /// - Tạo SSL
    /// - Tạo ClientSession
    /// </summary>
    public class TcpServer
    {
        private TcpListener _listener;
        private X509Certificate2 _serverCert;
        private bool _dangChay = false;
        // Thêm danh sách client (Thread-safe list)
        private ConcurrentBag<ClientSession> _clients = new ConcurrentBag<ClientSession>();

        /// <summary>
        /// Event dùng để gửi log lên UI (FormMain)
        /// </summary>
        public event Action<string>? OnLog;

        public TcpServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);

            // Load chứng chỉ SSL (tạo bằng makecert / openssl)
            _serverCert = new X509Certificate2("server.pfx", "123456");
        }

        /// <summary>
        /// Bắt đầu server
        /// </summary>
        public async Task BatDauAsync()
        {
            _listener.Start();
            _dangChay = true;

            OnLog?.Invoke("Server đang lắng nghe...");

            while (_dangChay)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();

                OnLog?.Invoke($"Client kết nối: {client.Client.RemoteEndPoint}");

                // Tạo SSL Stream
                SslStream sslStream = new SslStream(
                    client.GetStream(),
                    false
                );

                await sslStream.AuthenticateAsServerAsync(
                    _serverCert,
                    false,
                    System.Security.Authentication.SslProtocols.Tls13,
                    false
                );

                // Tạo session cho client
                ClientSession session = new ClientSession(client, sslStream);
                _clients.Add(session);
                _ = Task.Run(() => session.XuLyAsync());
            }
        }

        /// <summary>
        /// Hàm Stream Video: Gửi 1 khung hình (ảnh) tới tất cả client đã đăng nhập
        /// </summary>
        public void BroadcastVideoFrame(byte[] imageBytes)
        {
            // 1. Mã hóa AES dữ liệu ảnh trước khi gửi
            byte[] encryptedData = AesHelper.Encrypt(imageBytes);

            // 2. Gửi cho tất cả client
            foreach (var session in _clients)
            {
                if (session.IsLoggedIn && session.Client.Connected)
                {
                    // Gửi gói tin loại 2 (Binary Video)
                    session.GuiDuLieu(2, encryptedData);
                }
            }
        }

        /// <summary>
        /// Dừng server
        /// </summary>
        public void Dung()
        {
            _dangChay = false;

            try
            {
                _listener.Stop();
            }
            catch { }

            OnLog?.Invoke("Server đã dừng");
        }
    }
}
