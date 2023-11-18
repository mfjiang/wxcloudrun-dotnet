using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreSys.Common
{
    public class TableStructureHelper<T> where T : class, new()
    {
        #region 将表结构信息查询到的数据转换成实体 ConverToTableStructure
        /// <summary>
        /// 将表结构信息查询到的数据转换成实体
        /// </summary>
        /// <param name="details"></param>
        /// <param name="tableStructures"></param>
        public static void ConverToTableStructure(IList<IDictionary<string, object>> details, List<T> tableStructures)
        {
            if (details != null && details.Count > 0)
            {
                foreach (var items in details)
                {
                    dynamic structureInfo = new T();
                    ulong tinyNum = 0;
                    foreach (var item in items)
                    {
                        switch (item.Key)
                        {
                            case "COLUMN_NAME":
                                structureInfo.column_name = item.Value.ToString();
                                break;
                            case "COLUMN_TYPE":
                                structureInfo.column_type = item.Value.ToString();
                                break;
                            case "CHARACTER_LEN":
                                structureInfo.char_len = ulong.Parse(item.Value.ToString());
                                break;
                            case "CHARACTER_OCTET_LEN":
                                structureInfo.char_octet_len = ulong.Parse(item.Value.ToString());
                                break;
                            case "NUMERIC_LEN":
                                structureInfo.number_len = ulong.Parse(item.Value.ToString());
                                break;
                            case "COLUMN_SCALE":
                                structureInfo.column_scale = ulong.Parse(item.Value.ToString());
                                break;
                            case "COLUMN_ISNULL":
                                structureInfo.column_isnull = item.Value.ToString() == "1" ? true : false;
                                break;
                            case "COLUMN_DEFAULT":
                                structureInfo.column_default = item.Value.ToString();
                                break;
                            case "COLUMN_PRI":
                                structureInfo.column_pri = item.Value.ToString() == "1" ? true : false;
                                break;
                            case "COLUMN_Index":
                                structureInfo.column_index = item.Value.ToString() == "1" ? true : false;
                                break;
                            case "COLUMN_Tin":
                                if (item.Value != null && !string.IsNullOrEmpty(item.Value.ToString()))
                                {
                                    var tinyData = item.Value.ToString().Replace("tinyint(", "").Replace(")", "");
                                    structureInfo.column_unsigned = tinyData.IndexOf("unsigned") >= 0 ? true : false;
                                    var numStr = tinyData.Replace("unsigned", "").Trim();
                                    tinyNum = ulong.Parse(numStr);
                                }
                                else
                                {
                                    structureInfo.column_unsigned = false;
                                }
                                break;
                        }
                    }
                    if (tinyNum != 0 && tinyNum < structureInfo.number_len)
                    {
                        structureInfo.number_len = tinyNum;
                    }
                    tableStructures.Add(structureInfo);
                }
            }
        }
        /// <summary>
        /// 将Desc查询到的表结构信息数据转换成实体
        /// </summary>
        /// <param name="details"></param>
        /// <param name="tableStructures"></param>
        public static void DescConverToTableStructure(IList<IDictionary<string, object>> details, List<T> tableStructures)
        {
            if (details != null && details.Count > 0)
            {
                foreach (var items in details)
                {
                    dynamic structureInfo = new T();
                    foreach (var item in items)
                    {
                        switch (item.Key)
                        {
                            case "Field":
                                structureInfo.column_name = item.Value.ToString();
                                break;
                            case "Type":
                                var str = item.Value.ToString();
                                var startIndex = str.IndexOf("(");
                                var dotIndex = str.IndexOf(",");
                                if (startIndex >= 0)
                                {
                                    structureInfo.column_type = str.Substring(0, startIndex);
                                    var len_scale = str.Replace(structureInfo.column_type, "").Replace("(", "").Replace(")", "");
                                    if (structureInfo.column_type == "char")
                                    {
                                        structureInfo.char_len = dotIndex >= 0 ? ulong.Parse(len_scale.Substring(0, len_scale.IndexOf(","))) : ulong.Parse(len_scale);
                                    }
                                    else
                                    {
                                        structureInfo.number_len = dotIndex >= 0 ? ulong.Parse(len_scale.Substring(0, len_scale.IndexOf(","))) : ulong.Parse(len_scale);
                                    }
                                    structureInfo.column_scale = dotIndex >= 0 ? ulong.Parse(str.Substring(dotIndex).Replace(",", "").Replace(")", "")) : 0;
                                }
                                else
                                {
                                    structureInfo.column_type = item.Value.ToString();
                                    structureInfo.number_len = 0;
                                    structureInfo.column_scale = 0;
                                }
                                break;
                            case "Null":
                                structureInfo.column_isnull = item.Value.ToString() == "YES" ? true : false;
                                break;
                            case "Key":
                                structureInfo.column_pri = item.Value != null && item.Value.ToString() == "PRI" ? true : false;
                                structureInfo.column_index = item.Value != null && item.Value.ToString() != "PRI" && item.Value.ToString() != "" ? true : false;
                                break;
                            case "Default":
                                structureInfo.column_default = item.Value != null ? item.Value.ToString() : "";
                                break;
                        }
                    }
                    tableStructures.Add(structureInfo);
                }
            }
        }
        #endregion

        #region 根据表得值中类型和名称转换成实体 CreateTableStructure
        public static T CreateTableStructure(string column_name, string column_type, string column_len)
        {
            dynamic structureInfo = new T();
            structureInfo.column_name = column_name;
            switch (column_type)
            {
                case "字符型":
                    structureInfo.column_type = "varchar";
                    structureInfo.char_len = string.IsNullOrEmpty(column_len) ? 0 : ulong.Parse(column_len);
                    structureInfo.char_octet_len = structureInfo.char_len * 3;
                    structureInfo.number_len = 0;
                    structureInfo.column_scale = 0;
                    structureInfo.column_isnull = true;
                    structureInfo.column_default = "";
                    structureInfo.column_pri = false;
                    structureInfo.column_index = false;
                    break;
                case "日期型":
                    structureInfo.column_type = "date";
                    structureInfo.char_len = 0;
                    structureInfo.char_octet_len = 0;
                    structureInfo.number_len = 0;
                    structureInfo.column_scale = 0;
                    structureInfo.column_isnull = true;
                    structureInfo.column_default = "";
                    structureInfo.column_pri = false;
                    structureInfo.column_index = false;
                    break;
                case "逻辑型":
                    structureInfo.column_type = "tinyint";
                    structureInfo.char_len = 1;
                    structureInfo.char_octet_len = 0;
                    structureInfo.number_len = 1;
                    structureInfo.column_scale = 0;
                    structureInfo.column_isnull = true;
                    structureInfo.column_default = "";
                    structureInfo.column_pri = false;
                    structureInfo.column_index = false;
                    break;
                case "数值型":
                    structureInfo.column_type = "decimal";
                    structureInfo.char_len = 0;
                    structureInfo.char_octet_len = 0;
                    structureInfo.number_len = string.IsNullOrEmpty(column_len) ? 0 : ulong.Parse(column_len) - 10 - 1;
                    structureInfo.column_scale = 10;
                    structureInfo.column_isnull = true;
                    structureInfo.column_default = "";
                    structureInfo.column_pri = false;
                    structureInfo.column_index = false;
                    break;
                default:
                    structureInfo.column_type = "varchar";
                    structureInfo.char_len = 200;
                    structureInfo.char_octet_len = 200 * 3;
                    structureInfo.number_len = 0;
                    structureInfo.column_scale = 0;
                    structureInfo.column_isnull = true;
                    structureInfo.column_default = "";
                    structureInfo.column_pri = false;
                    structureInfo.column_index = false;
                    break;
            }
            return structureInfo as T;
        }
        #endregion
    }
}
