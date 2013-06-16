using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace XChat2.Common.Hashes
{
    public class HashGenerator
    {
        private HashGenerator() { }

        public static byte[] GetMD5Hash(string s)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(s);

            MD5 md5 = MD5.Create();
            md5.Initialize();
            buffer = md5.ComputeHash(buffer);
            return buffer;
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ByteArrayToHexString(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
