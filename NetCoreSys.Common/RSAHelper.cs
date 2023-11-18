using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using RSAExtensions;
using XC.RSAUtil;

namespace NetCoreSys.Common
{
    //Author    江名峰
    //Date      2021-04-08

    /// <summary>
    /// 提供RSA对称加密处理的封装
    /// </summary>
    public static class RSAHelper
    {
        /// <summary>
        /// 通过导入私钥创建RSA实例
        /// </summary>
        /// <param name="keyType">RSA密钥类型</param>
        /// <param name="privateKey"></param>
        /// <param name="isPem">密钥类型 为 PKCS#1 and PKCS#8 时有效</param>
        /// <returns></returns>
        public static RSA CreateInstanceViaPrivateKey(RSAKeyType keyType, string privateKey, bool isPem = false)
        {
            var rsa = RSA.Create();
            rsa.ImportPrivateKey(keyType, privateKey, isPem);
            return rsa;
        }

        /// <summary>
        /// 通过导入公钥创建RSA实例
        /// </summary>
        /// <param name="keyType">RSA密钥类型</param>
        /// <param name="pubKey"></param>
        /// <param name="isPem">密钥类型 为 PKCS#1 and PKCS#8 时有效</param>
        /// <returns></returns>
        public static RSA CreateInstanceViaPublicKey(RSAKeyType keyType, string pubKey, bool isPem = false)
        {
            var rsa = RSA.Create();
            rsa.ImportPublicKey(keyType, pubKey, isPem);
            return rsa;
        }

        /// <summary>
        /// 加密，密钥取决于RSA实例使用的密钥
        /// </summary>
        /// <param name="rsa">RSA实例</param>
        /// <param name="content">待加密内容</param>
        /// <param name="padding">填充方式</param>
        /// <returns></returns>
        public static byte[] Encrypt(ref RSA rsa, byte[] content, RSAEncryptionPadding padding)
        {
            byte[] finalData = new byte[] { };
            finalData = rsa.Encrypt(content, padding);
            return finalData;
        }

        /// <summary>
        /// 解密，密钥取决于RSA实例使用的密钥
        /// </summary>
        /// <param name="rsa">RSA实例</param>
        /// <param name="encyptedContent">待解密内容</param>
        /// <param name="padding">填充方式</param>
        /// <returns></returns>
        public static byte[] Decypt(ref RSA rsa, byte[] encyptedContent, RSAEncryptionPadding padding)
        {
            byte[] finalData = new byte[] { };
            finalData = rsa.Decrypt(encyptedContent, padding);
            return finalData;
        }

        /// <summary>
        /// 创建PKC8工具对象,公钥加密时需要公钥，私钥解密时需要私钥
        /// </summary>
        /// <param name="publicKey">RSA公钥</param>
        /// <param name="privateKey">RSA私钥</param>
        /// <param name="encoding">字符串编码</param>
        /// <returns></returns>
        public static RsaPkcs8Util CreatePKC8Util(Encoding encoding, string publicKey = null, string privateKey = null)
        {
            RsaPkcs8Util pkc8 = null;
            if (publicKey == null && privateKey == null) { throw new ArgumentNullException("公钥和私钥不能同时为空，公钥加密时需要公钥，私钥解密时需要私钥"); }
            pkc8 = new RsaPkcs8Util(encoding, publicKey, privateKey);

            return pkc8;
        }
        /// <summary>
        /// RsaPkcs8Util 私钥解密
        /// </summary>
        /// <param name="rsa">RsaPkcs8Util对象</param>
        /// <param name="encyptedContent">加密内容</param>
        /// <param name="padding">填充方式，RSAEncryptionPadding.Pkcs1 与java的RSA默认填充兼容</param>
        /// <returns></returns>
        public static string DecyptViaRsaPkcs8Util(ref RsaPkcs8Util rsa, string encyptedContent, RSAEncryptionPadding padding)
        {
            string finalContent = String.Empty;
            try
            {
                finalContent = rsa.RsaDecrypt(encyptedContent, padding);
            }
            catch (Exception ex)
            {
                throw new Exception("RsaPkcs8Util 解密失败，请检查密钥及填充方式是否与密文加密形式对应", ex);
            }
            return finalContent;
        }

        /// <summary>
        /// RsaPkcs8Util 公钥加密
        /// </summary>
        /// <param name="rsa">RsaPkcs8Util对象</param>
        /// <param name="content">明文</param>
        /// <param name="padding">填充方式，RSAEncryptionPadding.Pkcs1 与java的RSA默认填充兼容</param>
        /// <returns></returns>
        public static string EncyptViaRsaPkcs8Util(ref RsaPkcs8Util rsa, string content, RSAEncryptionPadding padding)
        {
            string finalContent = String.Empty;

            try
            {
                finalContent = rsa.RsaEncrypt(content, padding);
            }
            catch (Exception ex)
            {
                throw new Exception("RsaPkcs8Util 加密失败，请检查密钥及填充方式", ex);
            }

            return finalContent;
        }

        /// <summary>
        /// App默认RSA加密
        /// </summary>
        /// <param name="aims">待加密文本</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="padding">填充方式，默认Pkcs1</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string aims, string publicKey, RSAEncryptionPadding padding = null)
        {
            var rsa = new RsaPkcs8Util(Encoding.UTF8, publicKey);
            return rsa.RsaEncrypt(aims, padding ?? RSAEncryptionPadding.Pkcs1);
        }

        /// <summary>
        /// App默认RSA解密
        /// </summary>
        /// <param name="aims">待解密文本</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="padding">填充方式，默认Pkcs1</param>
        /// <returns>解密后的字符串</returns>
        public static string Decrypt(string aims, string privateKey, RSAEncryptionPadding padding = null)
        {
            var rsa = new RsaPkcs8Util(Encoding.UTF8, null, privateKey);
            return rsa.RsaDecrypt(aims, padding ?? RSAEncryptionPadding.Pkcs1);
        }
    }
}
