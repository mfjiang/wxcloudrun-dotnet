using MySqlConnector;
using NetCoreSys.DataMap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace NetCoreMySqlUtility
{
    //author        江名峰
    //date          2019.02.13

    //todo: 增加ID分片计算,自动切换分片数据库的功能

    /// <summary>
    /// 表示数据ID分片事件的处理委托
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="id">要分片的ID</param>
    public delegate void DataIDShardingEventHandler(MySqlDalBase caller, ulong id);
    /// <summary>
    /// 表示数据时间分片事件的处理委托
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="dateTime">要分片的时间</param>
    public delegate void DataDateTimeShardingEventHandler(MySqlDalBase caller, DateTime dateTime);

    /// <summary>
    /// 表示MYSQL DAL 类型的基类
    /// </summary>
    public class MySqlDalBase : IEntityInfoProp
    {
        #region 字段
        private string m_ConnStr;
        private MySqlConnection m_MySqlConnection;
        private EntityInfo m_EntityInfo;//分片需要
        private string m_TableName;
        private string m_FirstKeyName;
        private string m_SecondKeyName;
        #endregion

        #region 分片处理
        /// <summary>
        /// 数据ID分片事件
        /// </summary>
        private static event DataIDShardingEventHandler IDShardingEvent;
        /// <summary>
        /// 数据时间分片事件
        /// </summary>
        private static event DataDateTimeShardingEventHandler DateTimeShardingEvent;

        /// <summary>
        /// 静态事件才可以在静态构造器中进行订阅
        /// </summary>
        static MySqlDalBase()
        {
            IDShardingEvent += MySqlDalBase_IDShardingEvent;
            DateTimeShardingEvent += MySqlDalBase_DateTimeShardingEvent;
        }

        /// <summary>
        /// 时间分片处理
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="dateTime">要分片的时间</param>
        private static void MySqlDalBase_DateTimeShardingEvent(MySqlDalBase caller, DateTime dateTime)
        {
            //以时间计算要使用的分表
            if (caller.m_EntityInfo != null && caller.m_EntityInfo.ShardingKeyType == ShardingType.YearMonthTable)
            {
                if (caller.m_MySqlConnection == null)
                {
                    caller.m_MySqlConnection = new MySqlConnection(caller.ConnStr);
                    caller.m_MySqlConnection.Open();
                }

                if (caller.m_MySqlConnection.State != ConnectionState.Open)
                {
                    caller.m_MySqlConnection.Open();
                }

                //检测库中是否存在分表
                //计算当前年月要使用的分片空间名
                caller.m_EntityInfo.ShardNameSpace = dateTime.ToString("yyyyMM");
                //string cmd_auto_create_table = String.Format("create table if not exists `{1}` like `{0}`", caller.m_EntityInfo.DataName, caller.m_EntityInfo.FullShardDataName);
                try
                {
                    //此处实现的自动建分表导致每次读写表操作性能降低一半，因此改为DB上预建月分表
                    //if (caller.m_Cmd == null)
                    //{
                    //    caller.m_Cmd = caller.m_MySqlConnection.CreateCommand();
                    //}
                    //caller.m_Cmd.CommandText = cmd_auto_create_table;
                    //caller.m_Cmd.ExecuteNonQuery();
                    //caller.m_Cmd.CommandText = "";

                    //MySqlHelper.ExecuteNonQuery(caller.m_MySqlConnection, cmd_auto_create_table);
                    //原表作为模板，真实表名加上年月
                    caller.m_TableName = caller.m_EntityInfo.FullShardDataName;
                }
                catch (Exception ex)
                {
                    throw new Exception("auto create table failed", ex);
                }
                finally
                {
                    caller.m_MySqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// ID分片处理
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="id"></param>
        private static void MySqlDalBase_IDShardingEvent(MySqlDalBase caller, ulong id)
        {
            //以ID计算要使用的分片索引,DB节点大于1才有意义
            if (caller.m_EntityInfo.ShardConnStrs.Count > 1)
            {
                int idex = caller.GetShardIndexInRange(id, caller.m_EntityInfo.ShardConnStrs, caller.m_EntityInfo.MaxInShard);
                string connstr = caller.m_EntityInfo.ShardConnStrs[idex].ToString();
                if (caller.m_MySqlConnection == null || !caller.m_ConnStr.Equals(connstr))
                {
                    bool opened = caller.m_MySqlConnection.State == ConnectionState.Open;
                    //切换数据库连接
                    caller.m_ConnStr = connstr;
                    caller.m_MySqlConnection = new MySqlConnection(connstr);
                    if (opened)
                    {
                        caller.m_MySqlConnection.Open();
                    }
                }
            }
        }

        /// <summary>
        /// 请求ID分片处理（插入、修改、读取指定主键数据时引发）
        /// <para>ShardingType 必须是 ShardingType.IDRange</para>
        /// </summary>
        /// <param name="id"></param>
        internal void ApplyDataSharding(ulong id)
        {
            if (m_EntityInfo != null && m_EntityInfo.ShardingKeyType == ShardingType.IDRange)
            {
                IDShardingEvent?.Invoke(this, id);
            }
        }

        /// <summary>
        /// 请求时间分表处理（插入、修改、读取指定时间范围数据时引发）
        /// <para>ShardingType 必须是 ShardingType.YearMonthTable</para> 
        /// </summary>
        /// <param name="dateTime"></param>
        internal void ApplyDataSharding(DateTime dateTime)
        {
            if (m_EntityInfo != null && m_EntityInfo.ShardingKeyType == ShardingType.YearMonthTable)
            {
                DateTimeShardingEvent?.Invoke(this, dateTime);
            }
        }

        #endregion

        /// <summary>
        /// 返回MySqlDalBase实例
        /// </summary>
        /// <param name="connstr">mysql连接串</param>
        public MySqlDalBase(string connstr)
        {
            if (string.IsNullOrEmpty(connstr))
            {
                throw new ArgumentNullException("connstr");
            }
            m_TableName = string.Empty;
            m_ConnStr = connstr;
            //m_MySqlConnection = new MySqlConnection(m_ConnStr);
        }

        #region 属性
        /// <summary>
        /// 获取连接串
        /// </summary>
        public string ConnStr => m_ConnStr;

        /// <summary>
        /// 获取连接实例
        /// </summary>
        public MySqlConnection MySqlConnection => m_MySqlConnection;

        /// <summary>
        /// 使用内置连接串创建新的Connection实例
        /// </summary>
        /// <returns></returns>
        public MySqlConnection CreateNewMySqlConnection()
        {
            //get { return m_MySqlConnection ?? new MySqlConnection(m_ConnStr);   }
            m_MySqlConnection = new MySqlConnection(m_ConnStr);
            return m_MySqlConnection;
        }

        protected DbConnection CreateNewConnection() => new MySqlConnection(m_ConnStr);

        /// <summary>
        /// 获取或设置表名，调用此基类方法前要赋值
        /// </summary>
        public virtual string TableName
        {
            get => m_TableName;
            set => m_TableName = value;
        }

        /// <summary>
        /// 获取或设置主键名
        /// </summary>
        public virtual string FirstKeyName
        {
            get => m_FirstKeyName;
            set => m_FirstKeyName = value;
        }

        /// <summary>
        /// 获取或设置第二主键名
        /// </summary>
        public virtual string SecondKeyName
        {
            get => m_SecondKeyName;
            set => m_SecondKeyName = value;
        }

        /// <summary>
        /// 获取或设置映射实体信息(数据分片需要)
        /// </summary>
        public EntityInfo EntityInfo
        {
            get => m_EntityInfo;
            set => m_EntityInfo = value;
        }
        #endregion

        /// <summary>
        /// 将表格转换为行字典的List
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns></returns>
        public virtual List<Dictionary<string, object>> TableToList(DataTable dt)
        {           
            return EntityFunctions.TableToList(dt);
        }

        /// <summary>
        /// 将行转换为字典
        /// </summary>
        /// <param name="row">数据行</param>
        /// <returns></returns>
        public virtual Dictionary<string, object> RowToDictionary(DataRow row)
        {          
            return EntityFunctions.RowToDictionary(row);
        }

        /// <summary>
        /// 字典转实体
        /// </summary>
        /// <typeparam name="T">实体模型</typeparam>
        /// <param name="dic">数据字典</param>
        /// <returns></returns>
        public virtual T DicToObject<T>(Dictionary<string, object> dic) where T : new()
        {
            return EntityFunctions.DicToObject<T>(dic);
        }

        /// <summary>
        /// 行转实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="row">数据行</param>
        /// <returns></returns>
        public virtual T RowToEntity<T>(DataRow row) where T : new()
        {            
            return EntityFunctions.RowToEntity<T>(row);
        }

        /// <summary>
        /// 获取多行
        /// </summary>        
        /// <returns></returns>
        public virtual DataTable GetTable()
        {
            DataTable dt = null;

            string sql = string.Format("select * from {0}", TableName);
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                DataSet ds = MySqlHelperPlus.ExecuteDataset(conn, sql, new MySqlParameter[] { });
                if (ds != null && ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
                conn.Close();
            }

            return dt;
        }

        /// <summary>
        /// 获取多行        
        /// </summary>        
        /// <param name="parameters">参数</param>
        /// <param name="sqlQuery">查询条件</param>
        /// <returns></returns>
        public virtual DataTable GetTable(string sqlQuery, MySqlParameter[] parameters)
        {
            DataTable dt = null;

            string sql = string.Format("select * from {0} {1}", TableName, sqlQuery);
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                DataSet ds = MySqlHelperPlus.ExecuteDataset(conn, sql, parameters);
                if (ds != null && ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
                conn.Close();
            }

            return dt;
        }

        /// <summary>
        /// 获取多行        
        /// </summary>        
        /// <param name="parameters">参数</param>
        /// <param name="sqlQuery">查询条件，不带where</param>
        /// <returns></returns>
        public virtual DataTable GetTable(string sqlQuery, string field, MySqlParameter[] parameters)
        {
            DataTable dt = null;

            string sql = string.Format("select{2} from {0} where {1}", TableName, sqlQuery, field);
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                DataSet ds = MySqlHelperPlus.ExecuteDataset(conn, sql, parameters);
                if (ds != null && ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
                conn.Close();
            }

            return dt;
        }

        /// <summary>
        /// 获取多行，使用临时连接对象        
        /// </summary>        
        /// <param name="parameters">参数</param>
        /// <param name="sqlQuery">查询语句 where 之后</param>
        /// <param name="conn">临时连接对象</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public virtual DataTable GetTable(string sqlQuery, string field, MySqlConnection conn, MySqlParameter[] parameters)
        {
            DataTable dt = null;

            string sql = string.Format("select {2} from {0} where {1}", TableName, sqlQuery, field);
            if (conn.State == ConnectionState.Closed) { conn.Open(); }
            using (conn)
            {
                DataSet ds = MySqlHelperPlus.ExecuteDataset(conn, sql, parameters);
                if (ds != null && ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
                conn.Close();
            }
            return dt;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="columnStr">列名，以逗号分隔的字符串</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual int AddOne(string columnStr, params MySqlParameter[] parameters)
        {
            int r = 0;
            if (parameters.Length > 0)
            {
                string parastr = "";
                for (int i = 0; i < parameters.Length; i++)
                {
                    parastr += parameters[i].ParameterName;
                    if (!((i + 1) == parameters.Length))
                    {
                        parastr += ",";
                    }
                }
                string sql = string.Format("insert into {0} ({1}) values({2})", TableName, columnStr, parastr);

                using MySqlConnection conn = new MySqlConnection(ConnStr);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                r = MySqlHelperPlus.ExecuteNonQuery(conn, sql, parameters);
                conn.Close();
            }
            return r;
        }

        /// <summary>
        /// 在事务锁中插入行（并发插入行带有互斥条件时使用）
        /// </summary>
        /// <param name="columnStr">列名，以逗号分隔的字符串</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual int AddOneOnTransaction(string columnStr, params MySqlParameter[] parameters)
        {
            int r = 0;
            if (parameters.Length > 0)
            {
                string parastr = "";
                for (int i = 0; i < parameters.Length; i++)
                {
                    parastr += parameters[i].ParameterName;
                    if (!((i + 1) == parameters.Length))
                    {
                        parastr += ",";
                    }
                }
                //string sql = string.Format("START TRANSACTION;insert into {0} ({1}) values({2});COMMIT;", TableName, columnStr, parastr);
                string sql = string.Format("insert into {0} ({1}) values({2})", TableName, columnStr, parastr);
                using MySqlConnection conn = new MySqlConnection(ConnStr);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                //var mt = conn.BeginTransaction(IsolationLevel.Serializable);
                //r = MySqlHelperPlus.ExecuteTransactionNonQuery(conn, sql, parameters);
                r = MySqlHelperPlus.ExecuteNonQuery(conn, sql, parameters);
                //mt.Commit();
                conn.Close();
            }
            return r;
        }

        /// <summary>
        /// 获取数据页
        /// </summary>
        /// <param name="sqlQuery">sql 查询命令，要含 where 关键字</param>
        /// <param name="orderBy">排序命令，要含 order by 关键字</param>
        /// <param name="fields">字段，默认为*</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageNo">页号</param>        
        /// <returns></returns>
        public virtual DataSet GetDataPage(string sqlQuery, string orderBy, string fields = "*", int pageSize = 10, int pageNo = 1)
        {
            //IN p_table_name        VARCHAR(1024),        /*表名*/  
            //IN p_fields            VARCHAR(1024),        /*查询字段*/  
            //IN p_page_size         INT,                  /*每页记录数*/  
            //IN p_page_now          INT,                  /*当前页*/  
            //IN p_order_string      VARCHAR(128),         /*排序条件(包含ORDER关键字,可为空)*/    
            //IN p_where_string      VARCHAR(1024),        /*WHERE条件(包含WHERE关键字,可为空)*/      
            //OUT p_out_rows          INT

            var p1 = new MySqlParameter("@p_table_name", m_TableName)
            {
                Direction = ParameterDirection.Input
            };

            var p2 = new MySqlParameter("@p_fields", fields)
            {
                Direction = ParameterDirection.Input
            };

            var p3 = new MySqlParameter("@p_page_size", pageSize)
            {
                Direction = ParameterDirection.Input
            };

            var p4 = new MySqlParameter("@p_page_now", pageNo)
            {
                Direction = ParameterDirection.Input
            };

            var p5 = new MySqlParameter("@p_order_string", orderBy)
            {
                Direction = ParameterDirection.Input
            };

            var p6 = new MySqlParameter("@p_where_string", sqlQuery)
            {
                Direction = ParameterDirection.Input
            };

            var p7 = new MySqlParameter("@p_out_rows", MySqlDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                p1,p2,p3,p4,p5,p6,p7
            };
            DataSet ds = null;

            if (parameters.Length > 0)
            {
                string sql = string.Format("call get_data_page({0},{1},{2},{3},{4},{5},{6})", "@p_table_name", "@p_fields", "@p_page_size", "@p_page_now", "@p_order_string", "@p_where_string", "@p_out_rows");
                using MySqlConnection conn = new MySqlConnection(ConnStr);
                ds = MySqlHelperPlus.ExecuteDataset(conn, sql, parameters);
                conn.Close();
            }

            return ds;
        }

        /// <summary>
        /// 获取单行
        /// </summary>
        /// <param name="sqlQuery">sql 查询命令，要含 where 关键字</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual DataRow GetRow(string sqlQuery, params MySqlParameter[] parameters)
        {
            string sql = string.Format("select * from {0} {1}", TableName, sqlQuery);
            DataRow dr = null;
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                DataSet ds = MySqlHelperPlus.ExecuteDataset(conn, sql, parameters);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        dr = ds.Tables[0].Rows[0];
                    }
                }
                conn.Close();
            }

            return dr;
        }

        /// <summary>
        /// 取第一行
        /// </summary>
        /// <param name="sqlQuery">sql 查询命令，要含 where 关键字</param>
        /// <param name="parameters">参数</param>
        /// <param name="orderField">排序字段</param>
        /// <returns></returns>
        public virtual DataRow GetFirstRow(string sqlQuery,string orderField, params MySqlParameter[] parameters)
        {
            string sql = string.Format("select * from {0} {1} order by {2} asc", TableName, sqlQuery,orderField);
            DataRow dr = null;
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                DataSet ds = MySqlHelperPlus.ExecuteDataset(conn, sql, parameters);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        dr = ds.Tables[0].Rows[0];
                    }
                }
                conn.Close();
            }

            return dr;
        }

        /// <summary>
        /// 取最后一行
        /// </summary>
        /// <param name="sqlQuery">sql 查询命令，要含 where 关键字</param>
        /// <param name="parameters">参数</param>
        /// <param name="orderField">排序字段</param>
        /// <returns></returns>
        public virtual DataRow GetLastRow(string sqlQuery, string orderField, params MySqlParameter[] parameters)
        {
            string sql = string.Format("select * from {0} {1} order by {2} desc", TableName, sqlQuery, orderField);
            DataRow dr = null;
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                DataSet ds = MySqlHelperPlus.ExecuteDataset(conn, sql, parameters);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        dr = ds.Tables[0].Rows[0];
                    }
                }
                conn.Close();
            }

            return dr;
        }

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual object GetColumnValue(string columnName, object id)
        {
            object result = null;
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select {columnName} from {TableName} where {FirstKeyName}=@{FirstKeyName}";
                    MySqlParameter p1 = new MySqlParameter($"@{FirstKeyName}", id);
                    cmd.Parameters.Add(p1);
                    result = cmd.ExecuteScalar();
                }
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="sqlQuery">sql 查询命令，要含 where 关键字，可以为空</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual long Count(string sqlQuery, params MySqlParameter[] parameters)
        {

            string sql;
            if (!string.IsNullOrEmpty(sqlQuery))
            {
                if (!sqlQuery.ToLower().Contains("where")) { sqlQuery = "where " + sqlQuery; }
                sql = string.Format("select count(*) from {0} {1}", m_TableName, sqlQuery);
            }
            else
            {
                sql = string.Format("select count(*) from {0}", m_TableName);
            }

            long count = 0;
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                count = (long)MySqlHelperPlus.ExecuteScalar(conn, sql, parameters);
                conn.Close();
            }
            return count;
        }

        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="sqlQuery">sql 查询命令，要含 where 关键字，可以为空</param>
        /// <param name="field">字段，全部 *</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual long Count(string sqlQuery, string field = "*", params MySqlParameter[] parameters)
        {
            string sql;
            if (!string.IsNullOrEmpty(sqlQuery))
            {
                if (!sqlQuery.ToLower().Contains("where")) { sqlQuery = "where " + sqlQuery; }
                sql = string.Format("select count({0}) from {1} {2}", field, m_TableName, sqlQuery);
            }
            else
            {
                sql = string.Format("select count({0}) from {1}", field, m_TableName);
            }

            long count = 0;
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                count = (long)MySqlHelperPlus.ExecuteScalar(conn, sql, parameters);
                conn.Close();
            }
            return count;
        }

        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="sqlQuery">sql 查询命令，要含 where 关键字，可以为空</param>
        /// <param name="field">字段，全部 *</param>
        /// <param name="parameters">参数</param>
        /// <param name="conn">临时连接</param>
        /// <returns></returns>
        public virtual long Count(string sqlQuery, MySqlConnection conn, string field = "*", params MySqlParameter[] parameters)
        {
            string sql;
            if (!string.IsNullOrEmpty(sqlQuery))
            {
                if (!sqlQuery.ToLower().Contains("where")) { sqlQuery = "where " + sqlQuery; }
                sql = string.Format("select count({0}) from {1} {2}", field, m_TableName, sqlQuery);
            }
            else
            {
                sql = string.Format("select count({0}) from {1}", field, m_TableName);
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            long count = 0;
            using (conn)
            {
                count = (long)MySqlHelperPlus.ExecuteScalar(conn, sql, parameters);
                conn.Close();
            }
            return count;
        }

        /// <summary>
        /// 删除单个
        /// </summary>
        /// <param name="firstKey">唯一主键</param>
        /// <returns></returns>
        public virtual int DeleteOne(object firstKey)
        {
            int r = 0;

            if (firstKey != null)
            {
                string sql = string.Format("delete from {0} where {1}=@{1}", TableName, FirstKeyName);
                using MySqlConnection conn = new MySqlConnection(ConnStr);
                r = MySqlHelperPlus.ExecuteNonQuery(conn, sql, new MySqlParameter("@" + FirstKeyName, firstKey));
                conn.Close();
            }

            return r;
        }

        /// <summary>
        /// 删除单个
        /// </summary>
        /// <param name="firstKey">第一主键</param>
        /// <param name="secondKey">第二主键</param>
        /// <returns></returns>
        public virtual int DeleteOne(object firstKey, object secondKey)
        {
            int r = 0;

            if (firstKey != null && secondKey != null)
            {
                string sql = string.Format("delete from {0} where {1}=@{1} and {2}=@{2}", TableName, FirstKeyName, SecondKeyName);
                using MySqlConnection conn = new MySqlConnection(ConnStr);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                r = MySqlHelperPlus.ExecuteNonQuery(conn, sql, new MySqlParameter("@" + FirstKeyName, firstKey), new MySqlParameter("@" + SecondKeyName, secondKey));
                conn.Close();
            }

            return r;
        }

        /// <summary>
        /// 删除多个
        /// </summary>
        /// <param name="sqlQuery">条件命令，不要含where关键字</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual int DeleteMany(string sqlQuery, params MySqlParameter[] parameters)
        {
            int r = 0;

            if (!string.IsNullOrEmpty(sqlQuery))
            {
                string sql = string.Format("delete from {0} where {1}", TableName, sqlQuery);
                using MySqlConnection conn = new MySqlConnection(ConnStr);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                r = MySqlHelperPlus.ExecuteNonQuery(conn, sql, parameters);
                conn.Close();
            }

            return r;
        }

        /// <summary>
        /// 关闭并释放当前实例引用的Connection对象
        /// </summary>
        public virtual void CloseAndDisposeMySqlConnection()
        {
            if (MySqlConnection != null && MySqlConnection.State != ConnectionState.Closed)
            {
                try
                {
                    MySqlConnection.Close();
                    MySqlConnection.Dispose();
                }
                catch (Exception)
                {

                }
            }

        }

        /// <summary>
        /// 更新行
        /// </summary>
        /// <param name="setValues">set语句，包含set</param>
        /// <param name="queryStr">查询语句，不含where</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual int Update(string setValues, string queryStr, MySqlParameter[] parameters)
        {
            int r = 0;
            if (String.IsNullOrEmpty(setValues)) { throw new ArgumentNullException("setValues"); }
            if (String.IsNullOrEmpty(queryStr)) { throw new ArgumentNullException("queryStr"); }
            string sql = $"update {TableName} {setValues} where {queryStr}";
            using (MySqlConnection conn = new MySqlConnection(ConnStr))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                r = MySqlHelperPlus.ExecuteNonQuery(conn, sql, parameters);

                conn.Close();
            }

            return r;
        }
    }
}
