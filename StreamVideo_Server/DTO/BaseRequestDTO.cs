using StreamVideo_Server.DTO;

namespace StreamVideo_Server.DTO
{
    /// <summary>
    /// DTO gốc cho mọi request từ client gửi lên server
    /// </summary>
    public class BaseRequestDTO
    {
        // Loại request: LOGIN / STREAM / LOGOUT
        public RequestType Type { get; set; }

        // Dữ liệu cụ thể (JSON string)
        public string Payload { get; set; }

    }
}
