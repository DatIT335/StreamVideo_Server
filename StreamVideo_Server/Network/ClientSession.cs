using StreamVideo_Server.Common;
using StreamVideo_Server.Database;
using StreamVideo_Server.DTO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Net.Security;

namespace StreamVideo_Server.Network
{
    public class ClientSession
    {
        // Đã sửa: Dùng Property 'Client' viết hoa (Public)
        public TcpClient Client { get; private set; }
        private SslStream _sslStream;
        private BinaryReader _reader;
        private BinaryWriter _writer;

        public bool IsLoggedIn { get; private set; } = false;
        public string Username { get; private set; } = "";

        public ClientSession(TcpClient client, SslStream sslStream)
        {
            Client = client; // Gán vào Property Client
            _sslStream = sslStream;
            _reader = new BinaryReader(_sslStream);
            _writer = new BinaryWriter(_sslStream);
        }

        public async Task XuLyAsync()
        {
            try
            {
                while (Client.Connected)
                {
                    // 1. Đọc header
                    int length = _reader.ReadInt32();
                    byte type = _reader.ReadByte();

                    // 2. Đọc payload
                    byte[] payload = _reader.ReadBytes(length);

                    if (type == 1) // JSON Request
                    {
                        string json = Encoding.UTF8.GetString(payload);
                        ProcessJsonRequest(json);
                    }
                }
            }
            catch
            {
                // Khi ngắt kết nối đột ngột
                XuLyLogout();
            }
            finally
            {
                _sslStream.Close();
                Client.Close();
            }
        }

        private void ProcessJsonRequest(string json)
        {
            var request = JsonSerializer.Deserialize<BaseRequestDTO>(json);

            // Phân loại Request
            if (request.Type == RequestType.LOGIN)
            {
                XuLyLogin(request.Payload);
            }
            else if (request.Type == RequestType.LOGOUT) // <--- Thêm xử lý Logout
            {
                XuLyLogout();
            }
        }

        private void XuLyLogin(string payload)
        {
            var login = JsonSerializer.Deserialize<LoginRequestDTO>(payload);
            UserRepository repo = new UserRepository();

            bool hopLe = repo.KiemTraDangNhap(login.TenDangNhap, login.MatKhau);

            if (hopLe)
            {
                IsLoggedIn = true;
                Username = login.TenDangNhap;

                // --- SỬA LỖI Ở ĐÂY ---
                // Dùng 'Client' viết hoa thay vì '_client'
                string ipClient = Client.Client.RemoteEndPoint?.ToString();
                repo.GhiLichSu(Username, "Đăng nhập", ipClient);
            }

            var response = new LoginResponseDTO
            {
                ThanhCong = hopLe,
                ThongBao = hopLe ? "Đăng nhập thành công" : "Sai thông tin"
            };

            string jsonRes = JsonSerializer.Serialize(response);
            GuiDuLieu(1, Encoding.UTF8.GetBytes(jsonRes));
        }

        // --- BỔ SUNG HÀM LOGOUT ---
        private void XuLyLogout()
        {
            if (IsLoggedIn)
            {
                UserRepository repo = new UserRepository();
                // Dùng Client viết hoa
                string ipClient = Client.Client.RemoteEndPoint?.ToString();

                // Ghi log vào DB
                repo.GhiLichSu(Username, "Đăng xuất", ipClient);

                IsLoggedIn = false;
                Username = "";
                Console.WriteLine($"User {Username} đã đăng xuất.");
            }
        }

        public void GuiDuLieu(byte type, byte[] data)
        {
            try
            {
                lock (_writer)
                {
                    _writer.Write((int)data.Length);
                    _writer.Write(type);
                    _writer.Write(data);
                    _writer.Flush();
                }
            }
            catch { }
        }
    }
}