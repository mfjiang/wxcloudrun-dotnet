using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreMySqlUtility
{
    public static class DbValueExtensions
    {
        /// <summary>
        /// 列表不为 <c>null</c> 或 <see cref="DBNull.Value"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotNullOrDbNull(this object value) 
            => value != null && value != DBNull.Value;
    }
}
