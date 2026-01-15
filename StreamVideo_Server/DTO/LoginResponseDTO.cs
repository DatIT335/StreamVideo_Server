using System;
using System.Collections.Generic;
using System.Text;

namespace StreamVideo_Server.DTO
{
    /// <summary>
    /// DTO server trả về sau khi xử lý login
    /// </summary>
    public class LoginResponseDTO
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; }
    }
}

