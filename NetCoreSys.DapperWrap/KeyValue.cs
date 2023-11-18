using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreSys.DapperWrap
{
    /// <summary>
    /// 查看Dapper最终生成的SQL语句
    /// </summary>
    public class KeyValue
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public static string GetSqlStr(string sql, object? param = null)
        {
            try
            {
                string tempSql = sql;
                if (param is not null)
                {
                    var paramstr = !string.IsNullOrEmpty(param?.ToString()) ? param.ToString() : "";
                    if (paramstr is not null && paramstr.Contains("{") && paramstr.Contains("}"))//匿名类型
                    {
                        string[] arr = paramstr.Replace("{", "").Replace("}", "").Split(',');
                        List<KeyValue> paramList = new List<KeyValue>();
                        foreach (var item in arr)
                        {
                            KeyValue kv = new KeyValue();
                            string[] temp = item.Split('=');
                            kv.Key = temp[0].Trim();
                            kv.Value = temp[1].Trim();
                            paramList.Add(kv);
                        }
                        foreach (var par in paramList)
                        {
                            tempSql = tempSql.Replace("@" + par.Key, "'" + par.Value + "'");
                        }
                    }
                    else//自定义实体类型
                    {
                        Type type = (param ?? new object()).GetType();
                        foreach (PropertyInfo p in type.GetProperties())
                        {
                            var Key = p.Name;
                            var Value = p.GetValue(param);
                            tempSql = tempSql.Replace("@" + Key, "'" + Value + "'");
                        }
                    }
                }
                return tempSql;
            }
            catch
            {
                return sql;
            }
        }
    }
}
