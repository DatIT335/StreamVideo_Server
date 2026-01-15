using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using StreamVideo_Server.DTO;
using StreamVideo_Server.Database;


namespace StreamVideo_Server.Network

{
    /// <summary>
    /// Đại diện cho 1 client đang kết nối
    /// </summary>
    public class ClientSession
    {
        private TcpClient _client;
        private SslStream _sslStream;

        public ClientSession(TcpClient client, SslStream sslStream)
        {
            _client = client;
            _sslStream = sslStream;
        }

        /// <summary>
        /// Hàm xử lý client
        /// </summary>
        public async Task XuLyAsync()
        {
            try
            {
                byte[] buffer = new byte[4096];

                while (_client.Connected)
                {
                    int soByte = await _sslStream.ReadAsync(buffer, 0, buffer.Length);
                    if (soByte == 0)
                        break;

                    string json = Encoding.UTF8.GetString(buffer, 0, soByte);

                    // Parse JSON thành BaseRequestDTO
                    BaseRequestDTO request =
                        JsonSerializer.Deserialize<BaseRequestDTO>(json);

                    // Phân loại request
                    switch (request.Type)
                    {
                        case RequestType.LOGIN:
                            XuLyLogin(request.Payload);
                            break;

                        case RequestType.STREAM:
                            XuLyStream(request.Payload);
                            break;

                        case RequestType.LOGOUT:
                            XuLyLogout();
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi ClientSession: " + ex.Message);
            }
            finally
            {
                _sslStream.Close();
                _client.Close();
            }
        }
        private async void XuLyLogin(string payload)
        {
            LoginRequestDTO login =
                JsonSerializer.Deserialize<LoginRequestDTO>(payload);

            UserRepository repo = new UserRepository();

            bool hopLe = repo.KiemTraDangNhap(
                login.TenDangNhap,
                login.MatKhau
            );

            LoginResponseDTO response = new LoginResponseDTO
            {
                ThanhCong = hopLe,
                ThongBao = hopLe
                    ? "Đăng nhập thành công (SQL)"
                    : "Sai tài khoản hoặc mật khẩu"
            };

            string json = JsonSerializer.Serialize(response);
            byte[] data = Encoding.UTF8.GetBytes(json);

            await _sslStream.WriteAsync(data);
        }



        private void XuLyStream(string payload)
        {
            Console.WriteLine("Client gửi yêu cầu STREAM");
            Console.WriteLine("Payload: " + payload);

            // TODO:
            // - Kiểm tra đã login chưa
            // - Bắt đầu gửi dữ liệu stream
        }

        private void XuLyLogout()
        {
            Console.WriteLine("Client LOGOUT");
        }

    }
}

