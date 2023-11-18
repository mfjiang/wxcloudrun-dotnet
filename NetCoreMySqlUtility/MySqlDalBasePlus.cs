using MySqlConnector;
using NetCoreSys.DataMap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NetCoreMySqlUtility
{
    //Author    江名峰
    //Date      2019.04.18

    /// <summary>
    /// 对MySqlDalBase的扩展
    /// </summary>
    public static class MySqlDalBasePlus
    {
        /// <summary>
        /// 按主键取数据实体（不支持自动分片）
        /// </summary>
        /// <typeparam name="TData">数据实体类型</typeparam>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="dalBase"></param>
        /// <param name="key">主键值</param>
        /// <returns></returns>
        public static TData GetEntity<TData, TKey>(this MySqlDalBase dalBase, TKey key) where TData : new()
        {
            DataRow row = dalBase.GetRow(string.Format("where `{1}`=@{1}", dalBase.TableName, dalBase.FirstKeyName),
                new MySqlParameter("@" + dalBase.FirstKeyName, key));
            if (row != null)
            {
                return dalBase.RowToEntity<TData>(row);
            }
            else
            {
                var o = default(TData);
                return o;
            }
        }

        /// <summary>
        /// 按主副键取数据实体（不支持自动分片）
        /// </summary>
        /// <typeparam name="TData">数据实体类型</typeparam>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <typeparam name="TKey2">副键类型</typeparam>
        /// <param name="dalBase"></param>
        /// <param name="key">主键值</param>
        /// <param name="key2">副键值</param>
        /// <returns></returns>
        public static TData GetEntity<TData, TKey, TKey2>(this MySqlDalBase dalBase, TKey key, TKey2 key2) where TData : new()
        {
            if (string.IsNullOrEmpty(dalBase.SecondKeyName)) { throw new Exception("second key not found on current entity"); }

            DataRow row = dalBase.GetRow(string.Format("where `{1}`=@{1} and `{2}`=@{2}", dalBase.TableName, dalBase.FirstKeyName, dalBase.SecondKeyName),
                new MySqlParameter("@" + dalBase.FirstKeyName, key),
                new MySqlParameter("@" + dalBase.SecondKeyName, key2));
            if (row != null)
            {
                return dalBase.RowToEntity<TData>(row);
            }
            else
            {
                var o = default(TData);
                return o;
            }
        }

        /// <summary>
        /// 取实体清单（不支持自动分片）
        /// </summary>
        /// <typeparam name="TData">数据实体类型</typeparam>
        /// <param name="dalBase"></param>
        /// <param name="sqlQuery">where xx=@xx..</param>
        /// <param name="paramas">参数</param>
        /// <returns></returns>
        public static List<TData> GetEntityList<TData>(this MySqlDalBase dalBase, string sqlQuery, params MySqlParameter[] paramas) where TData : new()
        {
            List<TData> list = new List<TData>();
            DataTable table = dalBase.GetTable(sqlQuery, paramas);
            if (table != null)
            {
                var dicl = dalBase.TableToList(table);
                foreach (var dic in dicl)
                {
                    list.Add(dalBase.DicToObject<TData>(dic));
                }
            }

            return list;
        }

        #region 数据分片相关
        /// <summary>
        /// 设置实体映射信息（自动分片必要条件）
        /// </summary>        
        /// <param name="entityInfo">实体映射信息</param>
        public static void SetEntityInfo(this MySqlDalBase dalBase, EntityInfo entityInfo)
        {
            dalBase.EntityInfo = entityInfo;
        }

        /// <summary>
        /// 请求数据ID分片同时切换连接的数据库
        /// <para>适用于insert,select,delete,update 指定的ID</para>
        /// <para>ShardingType 必须是 ShardingType.IDRange</para>
        /// </summary>
        /// <param name="dalBase"></param>
        /// <param name="id">用于分片的主键</param>
        public static void ApplyDataSharding(this MySqlDalBase dalBase, ulong id)
        {
            dalBase.ApplyDataSharding(id);
        }

        /// <summary>
        /// 请求数据时间分表
        /// <para>适用于insert,select,delete,update 指定的时间</para>
        /// <para>ShardingType 必须是 ShardingType.YearMonthTable</para>
        /// </summary>
        /// <param name="dalBase"></param>
        /// <param name="dateTime">用于分表的时间</param>
        public static void ApplyDataSharding(this MySqlDalBase dalBase, DateTime dateTime)
        {
            dalBase.ApplyDataSharding(dateTime);
        }

        /// <summary>
        /// 跨数据分片查询
        /// <para>使用临时连接对象，不改变当前实例的数据库连接</para>
        /// </summary>
        /// <param name="dalBase"></param>
        /// <param name="limit">最大数据行数</param>
        /// <param name="query">查询语句(where 之后)</param>
        /// <param name="field">字段，全部 *</param>
        /// <param name="parameters">SQL参数</param>
        public static DataTable CrossQuery(this MySqlDalBase dalBase, string query, string field = "*", int limit = 1000, params MySqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            List<DataTable> tables = new List<DataTable>();
            if (dalBase.EntityInfo == null || dalBase.EntityInfo.ShardConnStrs.Count == 0)
            {
                throw new Exception("there is no data sharding info");
            }
            try
            {
                //取第一个连接的DB数据
                if (dalBase.EntityInfo != null && dalBase.EntityInfo.ShardConnStrs.Count > 0)
                {
                    string connstr0 = dalBase.EntityInfo.ShardConnStrs[0].ToString();
                    using (MySqlConnection conn = new MySqlConnection(connstr0))
                    {
                        var temp = dalBase.GetTable(query + " limit " + limit, field, conn, parameters);
                        if (temp != null) { table = temp; }
                        tables.Add(table);
                        limit -= table.Rows.Count;
                    }
                }

                //取余下的连接的DB数据
                if (dalBase.EntityInfo != null && dalBase.EntityInfo.ShardConnStrs.Count > 1)
                {
                    for (int i = 1; i < dalBase.EntityInfo.ShardConnStrs.Count; i++)
                    {
                        if (limit > 0)
                        {
                            //使用临时连接对象，不改变当前实例的数据库连接
                            string connstr = dalBase.EntityInfo.ShardConnStrs[i].ToString();
                            using (MySqlConnection conn = new MySqlConnection(connstr))
                            {
                                var tb = dalBase.GetTable(query + " limit " + limit, field, conn, parameters);
                                tables.Add(tb);
                                limit -= tb.Rows.Count;
                            }
                        }
                    }
                }

                //合并表
                if (tables.Count > 1)
                {
                    for (int m = 1; m < tables.Count; m++)
                    {
                        if (table.TableName.Equals(tables[m].TableName))
                        {
                            table.Merge(tables[m], true, MissingSchemaAction.AddWithKey);
                        }
                    }
                    tables.Clear();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("cross query failed,cmd text:" + query, ex);
            }

            return table;
        }

        /// <summary>
        /// 跨数据分片查询，返回强类型List
        /// <para>使用临时连接对象，不改变当前实例的数据库连接</para>
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="dalBase"></param>
        /// <param name="limit">最大数据行数</param>
        /// <param name="query">查询语句(where 之后)</param>
        /// <param name="field">字段，全部 *</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns></returns>
        public static List<TData> CrossQueryList<TData>(this MySqlDalBase dalBase, string query, string field = "*", int limit = 1000, params MySqlParameter[] parameters) where TData : new()
        {
            List<TData> list = new List<TData>();
            DataTable table = new DataTable();
            table = dalBase.CrossQuery(query, field, limit, parameters);
            if (table != null && table.Rows.Count > 0)
            {
                var dicl = dalBase.TableToList(table);
                foreach (var dic in dicl)
                {
                    list.Add(dalBase.DicToObject<TData>(dic));
                }
            }
            return list;
        }

        /// <summary>
        /// 跨数据分片计数
        /// <para>使用临时连接对象，不改变当前实例的数据库连接</para>
        /// </summary>
        /// <param name="dalBase"></param>
        /// <param name="query">查询语句(where 之后)</param>
        /// <param name="field">字段，全部 *</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns></returns>
        public static long CrossCount(this MySqlDalBase dalBase, string query, string field = "*", params MySqlParameter[] parameters)
        {
            long total = 0;
            string query_ = "where " + query;
            if (dalBase.EntityInfo == null || dalBase.EntityInfo.ShardConnStrs.Count == 0)
            {
                throw new Exception("there is no data sharding info");
            }

            try
            {
                //取第一个连接的DB数据
                if (dalBase.EntityInfo != null && dalBase.EntityInfo.ShardConnStrs.Count > 0)
                {
                    string connstr0 = dalBase.EntityInfo.ShardConnStrs[0].ToString();
                    using (MySqlConnection conn = new MySqlConnection(connstr0))
                    {
                        total += dalBase.Count(query_, conn, field, parameters);
                    }
                }

                //取余下的连接的DB数据
                if (dalBase.EntityInfo != null && dalBase.EntityInfo.ShardConnStrs.Count > 1)
                {
                    for (int i = 1; i < dalBase.EntityInfo.ShardConnStrs.Count; i++)
                    {
                        //使用临时连接对象，不改变当前实例的数据库连接
                        string connstr = dalBase.EntityInfo.ShardConnStrs[i].ToString();
                        using (MySqlConnection conn = new MySqlConnection(connstr))
                        {
                            total += dalBase.Count(query_, conn, field, parameters);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("cross count failed,cmd text:" + query_, ex);
            }

            return total;
        }

        /// <summary>
        /// 获取经过自动分片的ID
        /// </summary>
        /// <param name="dalBase"></param>
        /// <returns></returns>
        public static ulong GetID(this MySqlDalBase dalBase)
        {
            ulong id = DistributedIDGen.GetID(dalBase.EntityInfo.FullShardDataName);
            dalBase.ApplyDataSharding(id);
            return id;
        }

        /// <summary>
        /// 按主键取数据实体（自动分片）
        /// </summary>
        /// <typeparam name="TData">数据实体类型</typeparam>
        /// <param name="dalBase"></param>
        /// <param name="key">主键值</param>
        /// <returns></returns>
        public static TData GetEntity<TData>(this MySqlDalBase dalBase, ulong key) where TData : new()
        {
            dalBase.ApplyDataSharding(key);
            DataRow row = dalBase.GetRow(string.Format("where `{1}`=@{1}", dalBase.TableName, dalBase.FirstKeyName),
                new MySqlParameter("@" + dalBase.FirstKeyName, key));
            if (row != null)
            {
                return dalBase.RowToEntity<TData>(row);
            }
            else
            {
                var o = default(TData);
                return o;
            }
        }

        #region ID分片算法
        /// <summary>
        /// 按固定分片数取得主键分片索引
        /// </summary>
        /// <param name="key">用于分片的主键</param>
        /// <param name="shards">现有分片数</param>
        /// <param name="maxShards">最大分片数（必须是固定值，增减会导致主键分片计算前后不一致）</param>
        /// <returns></returns>
        public static int GetShardIndex(this MySqlDalBase dalBase, object key, List<object> shards, int maxShards)
        {
            int index = 0;

            string keystr = key.ToString();
            int keyhash = Encoding.UTF8.GetBytes(keystr).GetHashCode();

            index = (maxShards - 1) & keyhash;

            //如果索引超出现有分片数，使用第一个分片的索引
            if (index >= shards.Count)
            {
                index = 0;
            }

            return index;
        }

        /// <summary>
        /// 按固定数字范围取分片索引
        /// </summary>
        /// <param name="key">数字主键</param>
        /// <param name="shards">现有分片数</param>
        /// <param name="maxInRange">最大范围值，默认500万</param>
        /// <returns></returns>
        public static int GetShardIndexInRange(this MySqlDalBase dalBase, long key, List<object> shards, long maxInRange = 5000000)
        {
            int index = 0;
            if (key <= maxInRange)
            {
                index = 0;
            }
            else
            {
                if (shards.Count == 1)
                {
                    index = 0;
                }
                else
                {
                    //当前主键可以放到哪一片
                    long mod = key % maxInRange;
                    long temp = ((key / maxInRange) - 1);
                    index = int.Parse(temp.ToString());
                    //余数大于0，要往后移一个片
                    if (mod > 0)
                    {
                        index += 1;
                    }

                    //如果片索引超出现有片数范围，取最后一片的索引 
                    if (index >= shards.Count)
                    {
                        index = shards.Count - 1;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// 按固定数字范围取分片索引
        /// </summary>
        /// <param name="key">数字主键</param>
        /// <param name="shards">现有分片数</param>
        /// <param name="maxInRange">最大范围值，默认500万</param>
        /// <returns></returns>
        public static int GetShardIndexInRange(this MySqlDalBase dalBase, ulong key, List<object> shards, ulong maxInRange = 5000000)
        {
            int index = 0;
            if (key <= maxInRange)
            {
                index = 0;
            }
            else
            {
                if (shards.Count == 1)
                {
                    index = 0;
                }
                else
                {
                    //当前主键可以放到哪一片
                    ulong mod = key % maxInRange;
                    ulong temp = ((key / maxInRange) - 1);
                    index = int.Parse(temp.ToString());
                    //余数大于0，要往后移一个片
                    if (mod > 0)
                    {
                        index += 1;
                    }

                    //如果片索引超出现有片数范围，取最后一片的索引 
                    if (index >= shards.Count)
                    {
                        index = shards.Count - 1;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// 按固定数字范围取分片索引
        /// </summary>
        /// <param name="key">数字主键</param>
        /// <param name="shards">现有分片数</param>
        /// <param name="maxInRange">最大范围值，默认500万</param>
        /// <returns></returns>
        public static int GetShardIndexInRange(this MySqlDalBase dalBase, int key, List<object> shards, int maxInRange = 5000000)
        {
            int index = 0;
            if (key <= maxInRange)
            {
                index = 0;
            }
            else
            {
                if (shards.Count == 1)
                {
                    index = 0;
                }
                else
                {
                    //当前主键可以放到哪一片
                    int mod = key % maxInRange;
                    int temp = ((key / maxInRange) - 1);
                    index = int.Parse(temp.ToString());
                    //余数大于0，要往后移一个片
                    if (mod > 0)
                    {
                        index += 1;
                    }

                    //如果片索引超出现有片数范围，取最后一片的索引 
                    if (index >= shards.Count)
                    {
                        index = shards.Count - 1;
                    }
                }
            }

            return index;
        }
        #endregion

        #endregion
    }
}
