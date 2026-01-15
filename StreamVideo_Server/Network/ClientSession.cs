using StreamVideo_Server.Common; // Nhớ using AES
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
        public TcpClient Client { get; private set; } // Public để Server quản lý list
        private SslStream _sslStream;
        private BinaryReader _reader;
        private BinaryWriter _writer;

        public bool IsLoggedIn { get; private set; } = false;
        public string Username { get; private set; } = "";

        public ClientSession(TcpClient client, SslStream sslStream)
        {
            Client = client;
            _sslStream = sslStream;
            _reader = new BinaryReader(_sslStream); // Dùng BinaryReader để đọc chính xác byte
            _writer = new BinaryWriter(_sslStream);
        }

        public async Task XuLyAsync()
        {
            try
            {
                while (Client.Connected)
                {
                    // 1. Đọc độ dài gói tin (4 byte int)
                    // Cần try-catch vì ReadInt32 sẽ throw nếu ngắt kết nối
                    int length = _reader.ReadInt32();

                    // 2. Đọc loại gói tin (1 byte)
                    byte type = _reader.ReadByte();

                    // 3. Đọc dữ liệu payload
                    byte[] payload = _reader.ReadBytes(length);

                    if (type == 1) // Loại 1: JSON Request (Login...)
                    {
                        string json = Encoding.UTF8.GetString(payload);
                        ProcessJsonRequest(json);
                    }
                    // Nếu là loại 2 (Binary) thì server hiện tại chưa cần xử lý chiều lên từ client (trừ khi client stream ngược lại)
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Client ngắt kết nối.");
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
            if (request.Type == RequestType.LOGIN)
            {
                XuLyLogin(request.Payload);
            }
        }

        private void XuLyLogin(string payload)
        {
            var login = JsonSerializer.Deserialize<LoginRequestDTO>(payload);
            UserRepository repo = new UserRepository();

            // Kiểm tra DB
            bool hopLe = repo.KiemTraDangNhap(login.TenDangNhap, login.MatKhau);

            if (hopLe)
            {
                IsLoggedIn = true;
                Username = login.TenDangNhap;
            }

            var response = new LoginResponseDTO
            {
                ThanhCong = hopLe,
                ThongBao = hopLe ? "Đăng nhập thành công" : "Sai thông tin"
            };

            // Gửi phản hồi về Client (Gói tin loại 1 - JSON)
            string jsonRes = JsonSerializer.Serialize(response);
            GuiDuLieu(1, Encoding.UTF8.GetBytes(jsonRes));
        }

        // Hàm gửi dữ liệu chung (Thread-safe đơn giản)
        public void GuiDuLieu(byte type, byte[] data)
        {
            try
            {
                lock (_writer) // Tránh tranh chấp tài nguyên khi gửi từ nhiều luồng
                {
                    _writer.Write((int)data.Length); // 4 byte độ dài
                    _writer.Write(type);             // 1 byte loại
                    _writer.Write(data);             // Payload
                    _writer.Flush();
                }
            }
            catch { /* Client có thể đã out */ }
        }
    }
}