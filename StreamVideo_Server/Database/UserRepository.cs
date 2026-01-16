using Microsoft.Data.SqlClient;
using StreamVideo_Server.Common;

namespace StreamVideo_Server.Database
{
    public class UserRepository
    {
        public bool KiemTraDangNhap(string tenDangNhap, string matKhau)
        {
            using SqlConnection conn =
                new SqlConnection(DbConfig.ConnectionString);

            conn.Open();

            string sql = @"
                SELECT COUNT(*) 
                FROM Users 
                WHERE TenDangNhap = @user AND MatKhau = @pass";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@user", tenDangNhap);
            cmd.Parameters.AddWithValue("@pass", matKhau);

            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
        public void GhiLichSu(string username, string hanhDong, string ip)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();
                string sql = "INSERT INTO UserHistory (TenDangNhap, HanhDong, IPAddress, ThoiGian) VALUES (@u, @h, @ip, GETDATE())";

                using SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@h", hanhDong);
                cmd.Parameters.AddWithValue("@ip", ip);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi ghi log: " + ex.Message);
            }
        }
    }
}
