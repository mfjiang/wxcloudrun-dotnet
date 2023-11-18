using System;
using System.Text;
using System.Threading;

namespace JMF.CodeLibrary.Common
{
    //Author    江名峰
    //Date      2012-09-26

    /// <summary>
    /// 表示用来处理ASCII码相关的辅助类
    /// </summary>
    public static class ASCIICodeHelper
    {
        /// <summary>
        /// 生成指定长度的ASCII随机码字符串(数字及大小写字母混和)
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>ASCII随机码字符串</returns>
        public static string GenericRandomASCIIChars(int length)
        {
            StringBuilder code = new StringBuilder();

            //0-9:48-57,a-z:65-90,A-Z:97-122            
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();

            if (length > 0)
            {
                while (code.Length < length)
                {
                    Random rand = new Random(Guid.NewGuid().GetHashCode());
                    int number = rand.Next(48, 122);
                    if ((number >= 48 && number <= 57) || (number >= 65 && number <= 90) || (number >= 97 && number <= 122))
                    {
                        byte[] byteArray = new byte[] { (byte)number };
                        string chr = asciiEncoding.GetString(byteArray);
                        code.Append(chr);
                    }
                }
            }

            return code.ToString();
        }

        /// <summary>
        /// 随机生成ASCII数字串
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string GenericRandomASCIINumber(int length)
        {
            string code = string.Empty;

            if (length > 0)
            {
                while (code.Length < length)
                {
                    //重复概率在0.0015%~0.0026%
                    Random rand = new Random(ObjectIDHelper.GenericObjectID().GetHashCode());
                    //重复概率在0.005%~0.006%
                    //Random rand = new Random(Guid.NewGuid().GetHashCode());
                    int number = rand.Next(0, 99999);
                    code += number.ToString();
                }

                if (code.Length > length)
                {
                    code = code.Remove(length, (code.Length - length));
                }
            }

            //重复概率降到万分之零到万分之四
            Thread.Sleep(5);

            return code.ToString();
        }
    }
}
