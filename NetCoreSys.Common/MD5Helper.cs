using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NetCoreSys.Common
{
    //Author linfeng
    //Date 2012-07-16

    /// <summary>
    /// 表示Md5 帮助类
    /// </summary>
    public static class MD5Helper
    {
        /// <summary>
        /// 获取输入字符串的md5的hash值，字符串编码为UTF8
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <returns>32位小写的16进制的hash值</returns>
        public static string GetMd5Hash(string input)
        {
            string md5Hash = string.Empty;

            if (!string.IsNullOrEmpty(input))
            {
                byte[] encoder = Encoding.UTF8.GetBytes(input);
                md5Hash = GetMd5Hash(encoder);
            }

            return md5Hash;
        }

        /// <summary>
        /// 获取输入byte数组md5的hash值
        /// </summary>
        /// <param name="input">输入的byte数组</param>
        /// <returns>32位小写的16进制的hash值</returns>
        public static string GetMd5Hash(byte[] input)
        {
            string md5Hash = string.Empty;

            if (input != null && input.Length > 0)
            {
                StringBuilder sb = new StringBuilder();

                using (MD5 md5 = MD5.Create())
                {
                    byte[] data = md5.ComputeHash(input);
                    foreach (byte b in data)
                    {
                        //转换为16进制的hash码
                        sb.Append(b.ToString("x2"));
                    }
                }

                md5Hash = sb.ToString();
            }

            return md5Hash;
        }

        /// <summary>
        /// 获取IO流md5的hash值
        /// </summary>
        /// <param name="input">IO流</param>
        /// <returns>32位小写的16进制的hash值</returns>
        public static string GetMd5Hash(Stream input)
        {
            string md5Hash = string.Empty;

            if (input != null)
            {
                using (MD5 md5 = MD5.Create())
                {
                    StringBuilder sb = new StringBuilder();

                    byte[] data = md5.ComputeHash(input);

                    foreach (byte b in data)
                    {
                        //转换为16进制的hash码
                        sb.Append(b.ToString("x2"));
                    }

                    md5Hash = sb.ToString();
                }
            }

            return md5Hash;
        }

    }
}
