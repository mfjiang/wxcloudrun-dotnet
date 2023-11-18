using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JMF.CodeLibrary.Common
{
    //Author    江名峰
    //Date      2012.10.09

    /// <summary>
    /// AES加解密助手
    /// </summary>
    public static class AESHelper
    {
        /// <summary>
        /// 加密数据以字节数据返回
        /// <para>向量IV自动取密钥的前16位字节</para>
        /// <para>密钥长度：32位字节，256位比特</para>
        /// <para>模式：ECB</para>
        /// <para>填充：ISO10126</para>
        /// </summary>
        /// <param name="data">要加密的数据</param>
        /// <param name="keyData">密钥</param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] data, byte[] keyData)
        {
            byte[] outBlock;

            //AES加密
            Aes aes = Aes.Create();
            //aes.GenerateIV();
            //取密钥的前16位字节作为向量
            aes.IV = keyData.Take(16).ToArray();
            aes.Key = keyData;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.ISO10126;
            ICryptoTransform ict = aes.CreateEncryptor();
            outBlock = ict.TransformFinalBlock(data, 0, data.Length);
            ict.Dispose();
            aes.Clear();
            return outBlock;
        }

        /// <summary>
        /// 解密
        /// <para>向量IV自动取密钥的前16位字节</para>
        /// <para>密钥长度：32位字节，256位比特</para>
        /// <para>模式：ECB</para>
        /// <para>填充：ISO10126</para>
        /// </summary>
        /// <param name="data">要解密的数据</param>
        /// <param name="keyData">密钥</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] data, byte[] keyData)
        {
            byte[] outBlock;

            //AES加密
            Aes aes = Aes.Create();
            //aes.GenerateIV();
            //取密钥的前16位比特作为向量
            aes.IV = keyData.Take(16).ToArray();
            aes.Key = keyData;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.ISO10126;
            ICryptoTransform ict = aes.CreateDecryptor();
            outBlock = ict.TransformFinalBlock(data, 0, data.Length);
            ict.Dispose();
            aes.Clear();
            return outBlock;
        }

        /// <summary>
        /// 加密数据以字节数据返回
        /// <para>向量IV自动取密钥的前16位字节</para>
        /// <para>密钥长度：32位字节，256位比特</para>
        /// <para>模式：ECB</para>
        /// <para>填充：PKCS7</para>
        /// </summary>
        /// <param name="data">要加密的数据</param>
        /// <param name="keyData">密钥</param>
        /// <returns></returns>
        public static byte[] Encrypt_PKCS7(byte[] data, byte[] keyData)
        {
            byte[] outBlock;

            //AES加密
            Aes aes = Aes.Create();
            //aes.GenerateIV();
            //取密钥的前16位字节作为向量
            //aes.IV = keyData.Take(16).ToArray();
            aes.Key = keyData;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = aes.CreateEncryptor();
            outBlock = ict.TransformFinalBlock(data, 0, data.Length);
            ict.Dispose();
            aes.Clear();
            return outBlock;
        }

        /// <summary>
        /// 解密
        /// <para>向量IV自动取密钥的前16位字节</para>
        /// <para>密钥长度：32位字节，256位比特</para>
        /// <para>模式：ECB</para>
        /// <para>填充：PKCS7</para>
        /// </summary>
        /// <param name="data">要解密的数据</param>
        /// <param name="keyData">密钥</param>
        /// <returns></returns>
        public static byte[] Decrypt_PKCS7(byte[] data, byte[] keyData)
        {
            byte[] outBlock;

            //AES加密
            Aes aes = Aes.Create();
            //aes.GenerateIV();
            //取密钥的前16位比特作为向量
            //aes.IV = keyData.Take(16).ToArray();
            aes.Key = keyData;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = aes.CreateDecryptor();
            outBlock = ict.TransformFinalBlock(data, 0, data.Length);
            ict.Dispose();
            aes.Clear();
            return outBlock;
        }

        /// <summary>
        /// 将数据加密并转成Base64字符串
        /// </summary>
        /// <param name="data">要解密的数据</param>
        /// <param name="keyData">密钥</param>
        /// <param name="isPKCS7">填充模式是否为PKCS7默认不是</param>
        /// <returns></returns>
        public static string EncryptToBase64(byte[] data, byte[] keyData, bool isPKCS7 = false)
        {
            string b64 = string.Empty;
            byte[] temp = isPKCS7 == true ? Encrypt_PKCS7(data, keyData) : Encrypt(data, keyData);
            b64 = Convert.ToBase64String(temp);

            return b64;
        }

        /// <summary>
        /// 从Base64字符串中解密
        /// </summary>
        /// <param name="b64Str">密文的Base64字符串</param>
        /// <param name="keyData">密钥</param>
        /// <returns></returns>
        public static string DecryptFromBase64(string b64Str, byte[] keyData, bool isPKCS7 = false)
        {
            string output = string.Empty;

            byte[] encryptedData = Convert.FromBase64String(b64Str);
            byte[] decryptedData = isPKCS7 == true ? Decrypt_PKCS7(encryptedData, keyData) : Decrypt(encryptedData, keyData);
            output = UTF8Encoding.UTF8.GetString(decryptedData);

            return output;
        }

        /// <summary>
        /// 有密码的AES加密，ECB,PKCS7
        /// </summary>
        /// <param name="text">待加密字符</param>
        /// <param name="key">密码</param>
        /// <returns></returns>
        public static string Encrypt(string toEncrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);

        }

        /// <summary>
        /// AES解密，ECB,PKCS7
        /// </summary>
        /// <param name="toDecrypt">待解密字符</param>
        /// <param name="key">密码</param>
        /// <returns></returns>
        public static string Decrypt(string toDecrypt, string key)
        {

            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            RijndaelManaged rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);

        }
        public static string AESDecrypt(string encryptedDatatxt, string AesKey, string AesIV)
        {
            try
            {
                //过滤特殊字符即可    
                //string dummyData = encryptedDatatxt.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                //if (dummyData.Length % 4 > 0)
                //{
                //    dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                //}
                //byte[] byteArray = Convert.FromBase64String(dummyData);
                byte[] encryptedData = Convert.FromBase64String(encryptedDatatxt);
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Key = Convert.FromBase64String(AesKey);
                rijndaelCipher.IV = Convert.FromBase64String(AesIV);
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.None;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                string result = Encoding.Default.GetString(plainText);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        /// <summary>
        /// AES解密：从小程序中 getPhoneNumber 返回值中，解析手机号码
        /// </summary>
        /// <param name="encryptedData">包括敏感数据在内的完整用户信息的加密数据，详细见加密数据解密算法</param>
        /// <param name="IV">加密算法的初始向量</param>
        /// <param name="Session_key"></param>
        /// <returns>手机号码</returns>
        public static string getPhoneNumber(string encryptedData, string IV, string Session_key)
        {
            try
            {
                //string dummyData = encryptedData.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                //if (dummyData.Length % 4 > 0)
                //{
                //    dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                //}
                //byte[] byteArray = Convert.FromBase64String(dummyData);
                //byte[] encryptedData = Convert.FromBase64String(dummyData);
                byte[] encryData = Convert.FromBase64String(encryptedData.Trim('\0'));  // strToToHexByte(text);
                var key  = Convert.FromBase64String(Session_key.Trim('\0')); // Encoding.UTF8.GetBytes(AesKey);
                var iv = Convert.FromBase64String(IV.Trim('\0'));// Encoding.UTF8.GetBytes(AesIV);                                          //创建aes对象
                var aes = Aes.Create();

                if (aes == null)
                {
                    throw new InvalidOperationException("未能获取Aes算法实例");
                }
                //设置模式为CBC
                aes.Mode = CipherMode.CBC;
                //设置Key大小
                aes.KeySize = 128;
                //设置填充
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;
                //创建解密器
                var de = aes.CreateDecryptor();
                //解密数据
                var decodeByteData = de.TransformFinalBlock(encryData, 0, encryData.Length);
                //转换为字符串
                var result = Encoding.UTF8.GetString(decodeByteData);
                return result;

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                throw ex;

            }
        }
    }
}
