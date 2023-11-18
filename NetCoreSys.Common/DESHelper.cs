using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NetCoreSys.Common
{
    public class DESHelper
    {
        private static string encryptKey = "@2O^2*zj";//密钥(长度必须8位)   

        /// <summary>
        /// DES加密
        /// 2020-12-11 Jess、
        /// </summary>
        /// <param name="pToEncrypt">加密字符串</param>
        /// <returns></returns>
        public static string EncryptString(string pToEncrypt)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
            des.Key = UTF8Encoding.UTF8.GetBytes(encryptKey);
            des.IV = UTF8Encoding.UTF8.GetBytes(encryptKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// DES解密
        /// 2020-12-11 Jess、
        /// </summary>
        /// <param name="pToDecrypt">解密字符串</param>
        /// <returns></returns>
        public static string DecryptString(string pToDecrypt)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = UTF8Encoding.UTF8.GetBytes(encryptKey);
            des.IV = UTF8Encoding.UTF8.GetBytes(encryptKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
