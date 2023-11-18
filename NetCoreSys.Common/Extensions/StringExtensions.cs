using System;
using System.Text;

namespace NetCoreSys.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToCondition(this string fieldName, string symbol = "=")
        {
            return $"`{fieldName}` {symbol} @{fieldName}";
        }

        public static bool IsEmpty(this string aims)
        {
            return string.IsNullOrEmpty(aims);
        }

        public static string ToBase64(this string aims, Encoding encoding = null)
        {
            var b = (encoding ?? Encoding.UTF8).GetBytes(aims);
            return Convert.ToBase64String(b);
        }

        public static string FromBase64(this string aims, Encoding encoding = null)
        {
            var c = Convert.FromBase64String(aims);
            return (encoding ?? Encoding.UTF8).GetString(c);
        }
    }
}