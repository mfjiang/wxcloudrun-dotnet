using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NetCoreSys.Common
{
    public class Md5Util
    {
        public static String md5(String input)
        {
            try
            {
                byte[] encoder = Encoding.UTF8.GetBytes(input);
                byte[] md5 = GetMd5Hash(encoder);
                return toHexString(md5);
            }
            catch (Exception e)
            {
                
            }
            return "";
        }
        public static byte[] GetMd5Hash(byte[] input)
        {
            try
            {
                //MessageDigest mdInst = MessageDigest.getInstance("MD5");
                //md.update(data);
                //return md.digest();
                using (MD5 md5 = MD5.Create())
                {
                    byte[] data = md5.ComputeHash(input);
                    return data;
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return new byte[] { };
        }

        private static String toHexString(byte[] md5)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in md5)
            {
                sb.Append(b.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        private static String leftPad(String hex, char c, int size)
        {
            char[] cs = new char[size];
            Array.Fill(cs, c);
            Array.Copy(hex.ToCharArray(), 0, cs, cs.Length - hex.Length, hex.Length);
            return new String(cs);
        }
    }
}
