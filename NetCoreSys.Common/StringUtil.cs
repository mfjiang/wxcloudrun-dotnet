using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreSys.Common
{
    public class StringUtil
    {
        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="num">个数</param>
        /// <returns></returns>
        public static String getRandom(int num)
        {
            Random random = new Random();
            String result = "";
            for (int i = 0; i < num; i++)
            {
                result += random.Next(10);
            }
            return result;
        }
    }
}
