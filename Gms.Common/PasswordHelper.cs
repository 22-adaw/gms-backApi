using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net; 

namespace Gms.Common
{
    public class PasswordHelper
    {
        /// <summary>
        /// 加密密码（注册用）
        /// </summary>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// 校验密码（登录用）
        /// </summary>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (hashedPassword.StartsWith("$2a$11$"))
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            else
            {
                return password.Equals(hashedPassword, StringComparison.Ordinal);
            }
        }
        /// <summary>
        /// 随机生成密码
        /// </summary>
        /// <returns></returns>
        public static string GeneratePassword()
        {
            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lower = "abcdefghijklmnopqrstuvwxyz";
            string number = "0123456789";
            string underLine = "_";
            string special = "!@#$%^&*()-=+[]{}";

            //全部合并
            string allChars = upper + lower + number + underLine + special;

            Random random = new Random();
            char[] password = new char[15];

            //生成15位密码
            for (int i = 0; i < 15; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            return new string(password);
        }
        /// <summary>
        /// 生成删除密码
        /// </summary>
        /// <returns></returns>
        public static string GenerateDeletePassword()
        {
            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lower = "abcdefghijklmnopqrstuvwxyz";
            string number = "0123456789";
            string underLine = "_";
            string special = "!@#$%^&*()-=+[]{}";
            string allChars = upper + lower + number + underLine + special;

            Random random = new Random();
            char[] password = new char[20];
            //生成20位密码
            for (int i = 0; i < 20; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            return new string(password);
        }
    }
}