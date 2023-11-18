using System;
using System.Text;

namespace JMF.CodeLibrary.Common
{
    //Author    江名峰
    //Date      2014.05.14

    /// <summary>
    /// 表示账户名生成器(前缀+6个随机数字)
    /// </summary>
    public static class AccountNameHelper
    {
        /// <summary>
        /// 生成随机账户名(相同前缀下约有1,679,998,320个账号可用，重复概率约万分之零到万分之四)
        /// </summary>
        /// <param name="prefix">账户前缀（可空）</param>
        /// <returns></returns>
        public static string Generic(string prefix)
        {
            //增加天数和秒数的前置码
            if (DateTime.Now.Day < 10)
            {
                prefix += string.Format("0{0}", DateTime.Now.Day);
            }
            else
            {
                prefix += DateTime.Now.Day.ToString();
            }

            int second = DateTime.Now.Second;
            if (second < 10)
            {
                prefix += string.Format("0{0}", second);
            }
            else
            {
                prefix += second.ToString();
            }

            //前缀+随机数字
            string name = prefix + ASCIICodeHelper.GenericRandomASCIINumber(6);

            return name;
        }

        /// <summary>
        /// 过滤掉特殊表情字符
        /// </summary>
        /// <param name="emoj"></param>
        /// <returns></returns>
        public static string ReplaceEmoj(this string emoj)
        {
            if (string.IsNullOrEmpty(emoj))
            {
                return string.Empty;
            }

            foreach (var a in emoj)
            {
                byte[] bts = Encoding.UTF32.GetBytes(a.ToString());

                if (bts[0].ToString() == "253" && bts[1].ToString() == "255")
                {
                    emoj = emoj.Replace(a.ToString(), "?");
                }

            }
            return emoj;
        }
    }
}
