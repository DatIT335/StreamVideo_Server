using StreamVideo_Server.Common;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace StreamVideo_Server.Network
{
    public class TcpServer
    {
        private TcpListener _listener;
        private X509Certificate2 _serverCert;
        private bool _dangChay = false;
        private ConcurrentBag<ClientSession> _clients = new ConcurrentBag<ClientSession>();

        public event Action<string>? OnLog;

        public TcpServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _serverCert = new X509Certificate2("server.pfx", "123456");
        }

        public async Task BatDauAsync()
        {
            _listener.Start();
            _dangChay = true;
            OnLog?.Invoke("Server đang lắng nghe...");

            try
            {
                while (_dangChay)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    OnLog?.Invoke($"Client kết nối: {client.Client.RemoteEndPoint}");

                    SslStream sslStream = new SslStream(client.GetStream(), false);
                    await sslStream.AuthenticateAsServerAsync(_serverCert, false, System.Security.Authentication.SslProtocols.Tls13, false);

                    ClientSession session = new ClientSession(client, sslStream);
                    _clients.Add(session);
                    _ = Task.Run(() => session.XuLyAsync());
                }
            }
            catch (Exception)
            {
                if (_dangChay) OnLog?.Invoke("Server lỗi dừng đột ngột.");
            }
        }

        public void BroadcastVideoFrame(byte[] imageBytes)
        {
            if (_clients.IsEmpty) return;
            // Mã hóa Video (Type 2)
            byte[] encryptedData = AesHelper.Encrypt(imageBytes);

            foreach (var session in _clients)
            {
                if (session.IsLoggedIn && session.Client.Connected)
                    session.GuiDuLieu(2, encryptedData);
            }
        }

        // --- HÀM MỚI: GỬI ÂM THANH ---
        public void BroadcastAudio(byte[] audioData)
        {
            if (_clients.IsEmpty) return;
            // Gửi Audio (Type 3) - Không mã hóa cho nhanh (hoặc tùy bạn)
            foreach (var session in _clients)
            {
                if (session.IsLoggedIn && session.Client.Connected)
                    session.GuiDuLieu(3, audioData);
            }
        }

        public void Dung()
        {
            _dangChay = false;
            try { _listener.Stop(); } catch { }
            OnLog?.Invoke("Server đã dừng");
        }
    }
}