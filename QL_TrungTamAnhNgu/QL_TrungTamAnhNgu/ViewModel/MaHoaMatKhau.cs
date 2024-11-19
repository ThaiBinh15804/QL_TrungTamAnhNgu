using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace QL_TrungTamAnhNgu.ViewModel
{
    public class MaHoaMatKhau
    {
        private const string SecretKey = "thuankhanhbinhd@g1@####/+"; // Thay bằng một chuỗi bí mật mạnh và không thay đổi

        public static string HashPasswordSHA256(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Kết hợp mật khẩu với SecretKey
                string passwordWithKey = password + SecretKey;
                // Chuyển chuỗi kết hợp thành mảng byte
                byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordWithKey);
                // Tính toán hash
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                // Chuyển hash sang chuỗi hex để lưu trữ
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // Chuyển mỗi byte thành 2 ký tự hex
                }
                return sb.ToString();
            }
        }
    }
}