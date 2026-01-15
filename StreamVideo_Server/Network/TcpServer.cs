
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

        public TcpServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);

            // Load chứng chỉ SSL (tạo bằng makecert / openssl)
            _serverCert = new X509Certificate2("server.pfx", "123456");
        }

        public async Task BatDauAsync()
        {
            _listener.Start();
            Console.WriteLine("Server đang lắng nghe...");

            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();

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

                _ = Task.Run(() => session.XuLyAsync());
            }
        }
    }
}
