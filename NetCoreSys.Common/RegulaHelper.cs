using System.Text.RegularExpressions;

namespace NetCoreSys.Common
{
    //Author    江名峰
    //Date      2019.04.01

    /// <summary>
    /// 常用正则表达式
    /// </summary>
    public static class RegulaHelper
    {
        /// <summary>
        /// 验证手机号格式
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns></returns>
        public static bool IsMobile(string mobile)
        {
            return Regex.IsMatch(mobile, @"^[1]+[1,2,3,4,5,6,7,8,9]+\d{9}");
        }

        /// <summary>
        /// 验证数字
        /// </summary>
        /// <param name="str_number"></param>
        /// <returns></returns>
        public static bool IsNumber(string str_number)
        {
            return Regex.IsMatch(str_number, @"^[0-9]*$");
        }

        /// <summary>
        /// 验证中国邮政编码
        /// </summary>
        /// <param name="postalcode">中国邮政编码</param>
        /// <returns></returns>
        public static bool IsPostalcode(string postalcode)
        {
            return Regex.IsMatch(postalcode, @"^\d{6}$");
        }

        /// <summary>
        /// 验证EMAIL地址
        /// </summary>
        /// <param name="email">EMAIL地址</param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            return Regex.IsMatch(email, @"\\w{1,}@\\w{1,}\\.\\w{1,}");
        }

        /// <summary>
        /// 验证身份证号码
        /// </summary>
        /// <param name="idcard"></param>
        /// <returns></returns>
        public static bool IsIDcard(string idcard)
        {
            return Regex.IsMatch(idcard, @"(^\d{18}$)|(^\d{15}$)");
        }

        /// <summary>
        /// 过滤SQL敏感字符，并做安全转换
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string SqlDefense(string inputString)
        {
            string safeString = "";

            string SqlStr = @"and|or|exec|execute|insert|select|delete|update|alter|create|drop|count|\*|chr|char|asc|mid|substring|master|truncate|declare|xp_cmdshell|restore|backup|net +user|net +localgroup +administrators";
            string strRegex = @"\b(" + SqlStr + @")\b";

            Regex regex = new Regex(strRegex, RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(inputString);

            for (int i = 0; i < matches.Count; i++)
            {
                inputString = inputString.Replace(matches[i].Value, "[" + matches[i].Value + "]");
            }
            return safeString;
        }
    }
}
