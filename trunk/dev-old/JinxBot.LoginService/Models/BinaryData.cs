using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace JinxBot.LoginService.Models
{
    public static class BinaryData
    {
        public static byte[] ToBinaryData(this string str)
        {
            byte[] buffer = new byte[str.Length / 2];
            for (int i = 0, j = 0; i < str.Length; i += 2, j++)
            {
                buffer[j] = byte.Parse(str.Substring(i, 2), NumberStyles.HexNumber);
            }

            return buffer;
        }

        public static string ToHexString(this byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 2);
            for (int i = 0; i < data.Length; i++)
            {
                sb.AppendFormat("{0:x2}", data[i]);
            }

            return sb.ToString();
        }
    }
}