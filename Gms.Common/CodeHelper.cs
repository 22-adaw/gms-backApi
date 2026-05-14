using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Common
{
    public class CodeHelper
    {
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public static string GenerateCode()
        {
            int length = 6;
            const string codeChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            var random = new Random();

            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = codeChars[random.Next(codeChars.Length)];
            }

            return new string(result);
        }
    }
}
