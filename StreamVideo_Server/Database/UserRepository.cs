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
    }
}
