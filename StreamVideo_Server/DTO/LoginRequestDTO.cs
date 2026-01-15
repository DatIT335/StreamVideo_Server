using System;
using System.Collections.Generic;
using System.Text;

namespace StreamVideo_Server.DTO
{
    /// <summary>
    /// DTO client gửi lên khi đăng nhập
    /// </summary>
    public class LoginRequestDTO
    {
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
    }
}

