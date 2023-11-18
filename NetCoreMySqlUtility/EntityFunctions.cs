using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NetCoreMySqlUtility
{
    /// <summary>
    /// 实体处理方法
    /// </summary>
    public static class EntityFunctions
    {
        /// <summary>
        /// 将表格转换为行字典的List
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> TableToList(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        row.Add(dt.Columns[k].ColumnName, dt.Rows[i][k]);
                    }
                    list.Add(row);
                }
            }

            return list;
        }

        /// <summary>
        /// table 转实体 list
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<TEntity> TableToEntityList<TEntity>(DataTable dt) where TEntity : new()
        {
            List<TEntity> ls = new List<TEntity>();

            var ds = TableToList(dt);
            if (ds != null && ds.Count > 0)
            {
                for (int i = 0; i < ds.Count; i++)
                {
                    ls.Add(DicToObject<TEntity>(ds[i]));
                }
            }

            return ls;
        }

        /// <summary>
        /// 将行转换为字典
        /// </summary>
        /// <param name="row">数据行</param>
        /// <returns></returns>
        public static Dictionary<string, object> RowToDictionary(DataRow row)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            DataTable temp = row.Table;
            for (int i = 0; i < temp.Columns.Count; i++)
            {
                dic.Add(temp.Columns[i].ColumnName, row[i]);
            }

            return dic;
        }

        /// <summary>
        /// 字典转实体
        /// </summary>
        /// <typeparam name="T">实体模型</typeparam>
        /// <param name="dic">数据字典</param>
        /// <returns></returns>
        public static T DicToObject<T>(Dictionary<string, object> dic) where T : new()
        {
            var md = new T();
            //CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            // TextInfo textInfo = cultureInfo.TextInfo;
            foreach (var d in dic)
            {
                var filed = d.Key;
                var value = d.Value;
                var pinfo = md.GetType().GetProperty(filed);
                if (pinfo != null && value != null && !value.GetType().Equals(typeof(DBNull)))
                {
                    if (pinfo.PropertyType.Equals(typeof(bool)))
                    {
                        if (d.Value.GetType() != typeof(bool))
                        {
                            if (int.Parse(d.Value.ToString()) == 0)
                            {
                                value = false;
                            }
                            else
                            {
                                value = true;
                            }
                        }
                    }

                    //兼容mysql tinyint，当值为0或1时 .NET 会默认转成bool值，这里进行转换
                    if (pinfo.PropertyType.Equals(typeof(sbyte)) && value.GetType().Equals(typeof(bool)))
                    {
                        if (d.Value.ToString().ToLower().Equals("false"))
                        {
                            value = (sbyte)0;
                        }
                        else if (d.Value.ToString().ToLower().Equals("true"))
                        {
                            value = (sbyte)1;
                        }
                        else
                        {
                            sbyte tempValue = 0;
                            if (sbyte.TryParse(value.ToString(), out tempValue))
                            {
                                value = tempValue;
                            }
                        }
                    }

                    if (pinfo.PropertyType.Equals(typeof(byte)) && value.GetType().Equals(typeof(bool)))
                    {
                        if (d.Value.ToString().ToLower().Equals("false"))
                        {
                            value = (byte)0;
                        }
                        else if (d.Value.ToString().ToLower().Equals("true"))
                        {
                            value = (byte)1;
                        }
                        else
                        {
                            byte tempValue = 0;
                            if (byte.TryParse(value.ToString(), out tempValue))
                            {
                                value = tempValue;
                            }
                        }
                    }

                    if (pinfo.PropertyType.Equals(typeof(DateTime?)))
                    {
                        if (DateTime.TryParse(d.Value.ToString(), out DateTime dateTime))
                        {
                            value = dateTime;
                        }
                        else
                        {
                            value = null;
                        }
                    }

                    try
                    {
                        pinfo.SetValue(md, value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{md.GetType()}对象转换属性赋值失败,{pinfo.Name}", ex);
                    }
                }
            }
            return md;
        }

        /// <summary>
        /// 行转实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="row">数据行</param>
        /// <returns></returns>
        public static T RowToEntity<T>(DataRow row) where T : new()
        {
            T entity = default;
            if (row != null)
            {
                try
                {
                    Dictionary<string, object> dic = RowToDictionary(row);
                    entity = DicToObject<T>(dic);
                }
                catch (Exception ex)
                {
                    throw new Exception("转换数据实体失败", ex);
                }
            }
            return entity;
        }
    }
}
