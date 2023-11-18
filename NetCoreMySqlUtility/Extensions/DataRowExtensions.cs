using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace NetCoreMySqlUtility
{
    public static class DataRowExtensions
    {
        /// <summary>
        /// <see cref="EntityFunctions.RowToEntity"/> 的扩展方法。
        /// </summary>
        /// <remarks>参数和返回值详见 <see cref="EntityFunctions.RowToEntity"/> </remarks>
        public static T RowToEntity<T>(this DataRow row)
            where T : new()
        {
            return EntityFunctions.RowToEntity<T>(row);
        }
    }
}
