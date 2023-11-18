using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Dapper;
using Dapper.Extension;
using DotNetCoreConfiguration;
using LogMan;
using MySql.Data.MySqlClient;

namespace NetCoreSys.DapperWrap
{
    /// <summary>
    /// Dapper常用方法
    /// </summary>
    [Log(FileSuffix = ".log", LogLevel = LogMan.LogLevel.Info, LogName = nameof(DapperHelper), AutoCleanDays = 7)]
    public class DapperHelper
    {
        public static string DbNme { get; set; } = string.Empty;
        //从配置文件主动读取MYSQL群集配置
        public static string GetConnStr()
        {
            var m_MySqlClusterSettings = DotNetCoreConfiguration.ConfigurationManager.GetMySqlClusterSettings();
            //var m_AppSettings = DotNetCoreConfiguration.ConfigurationManager.GetAppConfig();

            return m_MySqlClusterSettings.GetMaster(DbNme).ConnStr;//m_AppSettings.OAuthCenterDBName
        }
        private static readonly string connectionString = GetConnStr();
        /// <summary>
        /// Dapper查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static DataTable? SELECT(string sql, object? param = null)
        {
            return ExecuteReader(sql, param);
        }
        private static DataTable? ExecuteReader(string sql, object? param = null)
        {
            string sqlStr = KeyValue.GetSqlStr(sql, param);//仅供调试用的SQL语句
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                try
                {
                    DataTable dt = new DataTable();
                    var reader = con.ExecuteReader(sql, param);
                    dt.Load(reader);
                    return dt;
                }
                catch(Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return default;
                }
            }
        }
        /// <summary>
        /// 增加
        /// </summary>
        public static bool INSERT(string sql, object? param = null)
        {
            return Execute(sql, param);
        }
        /// <summary>
        /// 修改
        /// </summary>
        public static bool UPDATE(string sql, object? param = null)
        {
            return Execute(sql, param);
        }
        /// <summary>
        /// 删除
        /// </summary>
        public static bool DELETE(string sql, object? param = null)
        {
            return Execute(sql, param);
        }
        /// <summary>
        /// Dapper增删改操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        private static bool Execute(string sql, object? param = null)
        {
            string sqlStr = KeyValue.GetSqlStr(sql, param);//仅供调试用的SQL语句
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Execute(sql, param);
                    return true;
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return false;
                }
            }
        }
    }
    /// <summary>
    /// Dapper<泛型>,返回List<T>
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>

    [Log(FileSuffix = ".log", LogLevel = LogMan.LogLevel.Info, LogName = nameof(DapperHelper), AutoCleanDays = 7)]
    public class DapperHelper<T> where T : class, new()
    {

        private static MySqlClusterSettings m_MySqlClusterSettings = DotNetCoreConfiguration.ConfigurationManager.GetMySqlClusterSettings();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string GetConnStr(string DbNme)
        {
            //var m_AppSettings = DotNetCoreConfiguration.ConfigurationManager.GetAppConfig();

            return m_MySqlClusterSettings.GetMaster(DbNme).ConnStr;//m_AppSettings.OAuthCenterDBName
        }
        //private static readonly string connectionString = GetConnStr();

        #region 根据sql和参数查询
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="dbNme">查询的数据库名称</param>
        /// <param name="sql">查询的sql</param>
        /// <param name="param">替换参数</param>
        /// <returns></returns>
        public static List<T>? QueryToList(string dbNme, string sql, object? param = null)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    return con.Query<T>(sql, param).ToList();
                }
                catch(Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return null;
                }
            }
        }
        public static List<TT>? QueryToDtosList<TT>(string dbNme, string sql, object? param = null)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    return con.Query<TT>(sql, param).ToList();
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return null;
                }
            }
        }
        public static List<T> SELECT(string dbNme, string sql, object? param = null)
        {
            return QueryToList(dbNme, sql, param) ?? new List<T>();
        }
        public static List<TT> SelectDtos<TT>(string dbNme, string sql, object? param = null)
        {
            return QueryToDtosList<TT>(dbNme, sql, param) ?? new List<TT>();
        }
        public static DataTable QueryTable(string dbNme, string sql, object? param = null)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    DataTable dt = new DataTable();
                    var reader = con.ExecuteReader(sql, param);
                    dt.Load(reader);
                    return dt;
                }
                catch(Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return null;
                }
            }
        }
        /// <summary>
        /// 查询第一个数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns>返回序列中的第一个元素,如果源序列为空，则抛异常。</returns>
        public static T? QueryFirst(string dbNme, string sql, object? param = null)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    return con.Query<T>(sql, param).ToList().First();
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return default(T);
                }
            }
        }
        /// <summary>
        /// 查询第一个数据没有返回默认值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns>返回序列中的第一个元素；如果序列中不包含任何元素，则返回默认值。</returns>
        public static T? QueryFirstOrDefault(string dbNme, string sql, object? param = null)
        {
            string sqlStr = KeyValue.GetSqlStr(sql, param);//仅供调试用的SQL语句
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    return con.Query<T>(sql, param).ToList().FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return default(T);
                }
            }
        }
        /// <summary>
        /// 获取单条记录 返回实体<T>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T? TOP_1(string dbNme, string sql, object? param = null)
        {
            return QueryFirstOrDefault(dbNme, sql, param);
        }
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns>返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。</returns>
        public static T? QuerySingle(string dbNme, string sql, object? param = null)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    return con.Query<T>(sql, param).ToList().Single();
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return default(T);
                }
            }
        }
        /// <summary>
        /// 查询单条数据,没有则返回默认值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns>返回序列中满足指定条件的唯一元素；如果这类元素不存在，则返回默认值；如果有多个元素满足该条件，此方法将引发异常。</returns>
        public static T? QuerySingleOrDefault(string dbNme, string sql, object? param = null)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    return con.Query<T>(sql, param).ToList().SingleOrDefault();
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex?.Message, ex);
                    return default(T);
                }
            }
        }
        public static TT? QuerySingleOrDefault<TT>(string dbNme, string sql, object? param = null)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    return con.Query<TT>(sql, param).ToList().SingleOrDefault();
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex?.Message, ex);
                    return default(TT);
                }
            }
        }
        /// <summary>
        /// 增加
        /// </summary>
        public static bool INSERT(string dbNme, string sql, object? param = null)
        {
            return Execute(dbNme, sql, param);
        }
        /// <summary>
        /// 修改
        /// </summary>
        public static bool UPDATE(string dbNme, string sql, object? param = null)
        {
            return Execute(dbNme, sql, param);
        }
        /// <summary>
        /// 删除
        /// </summary>
        public static bool DELETE(string dbNme, string sql, object? param = null)
        {
            return Execute(dbNme, sql, param);
        }
        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns>成功：true；失败：false；操作0行数据：true；</returns>
        public static bool Execute(string dbNme, string sql, object? param = null)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    con.Execute(sql, param);
                    return true;
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    var error = ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// Reader获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IDataReader? ExecuteReader(string dbNme, string sql, object param)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    return con.ExecuteReader(sql, param);
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return null;
                }
            }
        }
        /// <summary>
        /// Scalar获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object? ExecuteScalarReturnObject(string dbNme, string sql, object? param)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    return con.ExecuteScalar(sql, param);
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return null;
                }
            }
        }
        /// <summary>
        /// Scalar获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T? ExecuteScalarReturnEntity(string dbNme, string sql, object? param)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    return con.ExecuteScalar<T>(sql, param);
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return default(T);
                }
            }
        }
        /// <summary>
        /// 带参数的存储过程
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static (int, List<T>) StoredProcedurePageMultiple(string dbNme, string proc, object? param = null)
        {
            (int, List<T>) dict;
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    var reads = con.QueryMultiple(proc, param, null, null, CommandType.StoredProcedure);
                    var count = reads.Read<int>().FirstOrDefault();
                    var lists = reads.Read<T>().ToList();
                    dict = (count, lists);
                    return dict;
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return (0, new List<T>());
                }
            }
        }
        public static (int, List<OT>) StoredProcedurePageMultiple<OT>(string dbNme, string proc, object? param = null)
        {
            (int, List<OT>) dict;
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    var reads = con.QueryMultiple(proc, param, null, null, CommandType.StoredProcedure);
                    var count = reads.Read<int>().FirstOrDefault();
                    var lists = reads.Read<OT>().ToList();
                    dict = (count, lists);
                    return dict;
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return (0, new List<OT>());
                }
            }
        }
        /// <summary>
        /// 带参数的存储过程
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<T>? StoredProcedure(string dbNme, string proc, object? param = null)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    con.Open();
                    List<T> list = con.Query<T>(proc, param, null, true, null, CommandType.StoredProcedure).ToList();
                    return list;
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return null;
                }
            }
        }
        /// <summary>
        /// 事务1 - 多条SQL语句数组
        /// </summary>
        /// <param name="sqlarr">多条SQL</param>
        /// <param name="param">param</param>
        /// <returns></returns>
        public static int ExecuteTransaction(string dbNme, string[] sqlarr)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        int result = 0;
                        foreach (var sql in sqlarr)
                        {
                            result += con.Execute(sql, null, transaction);
                        }
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Loger.Error(typeof(DapperHelper), ex?.Message, ex);
                        return -1;
                    }
                }
            }
        }
        /// <summary>
        /// 事务2 - 声明参数
        ///demo:
        ///dic.Add("Insert into Users values (@UserName, @Email, @Address)",
        ///        new { UserName = "jack", Email = "380234234@qq.com", Address = "上海" });
        /// </summary>
        /// <param name="Key">多条SQL</param>
        /// <param name="Value">param</param>
        /// <returns></returns>
        public static int ExecuteTransaction(string dbNme, Dictionary<string, object> dic)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        int result = 0;
                        foreach (var sql in dic)
                        {
                            result += con.Execute(sql.Key, sql.Value, transaction);
                        }
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Loger.Error(typeof(DapperHelper), ex?.Message, ex);
                        return 0;
                    }
                }
            }
        }

        #endregion

        #region 根据实体查询
        /// <summary>
        /// 查全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T>? GetDataAll(string dbNme, string table_name, int pageIndex, int pageSize)
        {
            Type type = typeof(T);
            List<T> Data = new List<T>();
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                //用type.Name.ToLower()代替表名
                string sql = String.Empty;
                if (pageIndex < pageSize)
                {

                    sql = $"select * from {table_name} limit {pageIndex - 1},{(pageIndex) * pageSize}";
                }
                else
                {
                    sql = $"select * from {table_name}";
                }
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    object? obj = Activator.CreateInstance(type);
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                        {
                            if (property.PropertyType.FullName == "System.Boolean")
                            {
                                property.SetValue(obj, reader[property.Name].ToString() == "1" ? true : false);
                            }
                            else
                            {
                                property.SetValue(obj, reader[property.Name]);
                            }
                        }
                    }
                    if (obj is not null)
                        Data.Add((T)obj);
                }
                reader.Close();
            }
            return Data;
        }
        /// <summary>
        /// 按id查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T? GetDataById(string dbNme, string table_name, int id)
        {
            Type type = typeof(T);
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                string sql = $"select * from {table_name} where id={id}";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                object? obj = Activator.CreateInstance(type);
                while (reader.Read())
                {
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                        {
                            if (property.PropertyType.FullName == "System.Boolean")
                            {
                                property.SetValue(obj, reader[property.Name].ToString() == "1" ? true : false);
                            }
                            else
                            {
                                property.SetValue(obj, reader[property.Name]);
                            }
                        }
                    }
                }
                reader.Close();
                return obj is not null ? (T)obj : default(T);
            }
        }

        /// <summary>
        /// 单条添加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int INSERT(string dbNme, string table_name, T t, bool isautoid = true)
        {
            Type type = typeof(T);
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                Func<PropertyInfo, object> f = (x) =>
                {
                    var val = x.GetValue(t);
                    if (val is null)
                    {
                        return $"null";
                    }
                    else if (val.GetType().Equals(typeof(string)) || val.GetType().Equals(typeof(DateTime)))
                    {
                        if (val.GetType().Equals(typeof(DateTime))&&val!=null)
                        {
                            DateTime.TryParse($"{val}", out var dt);
                            return $"'{dt}'";
                        }
                        return $"'{val}'";
                    }
                    else
                    {
                        return val;
                    }
                };
                string sql = $"insert into {table_name} " +
                            (isautoid?
                            $"({string.Join(",", type.GetProperties().Where(n => !n.Name.Equals("id")).Select(n => $"`{n.Name}`"))}) "
                            : $"({string.Join(",", type.GetProperties().Select(n => $"`{n.Name}`"))}) "
                            )  +
                            (isautoid ?
                            $"values({string.Join(",", type.GetProperties().Where(n => !n.Name.Equals("id")).Select(n => $"{f(n)}"))})" 
                            : $"values({string.Join(",", type.GetProperties().Select(n => $"{f(n)}"))})"
                            );
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int result = cmd.ExecuteNonQuery();
                return result;
            }
        }
        /// <summary>
        /// 单条添加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static long INSERT(string dbNme, T entity)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                return conn.Insert(entity);
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="dbNme"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool UPDATE(string dbNme, T t)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                return conn.Update<T>(t);
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        public static bool DELETE(string dbNme, T t)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                return conn.Delete<T>(t);
            }
        }
        /// <summary>
        /// 删除所有数据
        /// </summary>
        /// <param name="dbNme"></param>
        /// <returns></returns>
        public static bool DeleteAll(string dbNme)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                return conn.DeleteAll<T>();
            }
        }
        #endregion

        #region 异步操作
        /// <summary>
        /// 异步获取所有数据
        /// </summary>
        /// <param name="dbNme"></param>
        /// <returns></returns>
        public static async Task<List<T>> GetAllAsync(string dbNme)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                return (await conn.GetAllAsync<T>()).ToList();
            }
        }
        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dbNme"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateAsync(T entity, string dbNme)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                return await conn.UpdateAsync(entity);
            }
        }
        /// <summary>
        /// 异步插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dbNme"></param>
        /// <returns></returns>
        public static async Task<long> InsertAsync(T entity, string dbNme)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                return await conn.InsertAsync(entity);
            }
        }
        /// <summary>
        /// 删除指定数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dbNme"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteAsync(T entity, string dbNme)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                return await conn.DeleteAsync(entity);
            }
        }
        /// <summary>
        /// 删除所有数据
        /// </summary>
        /// <param name="dbNme"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteAllAsync(string dbNme)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnStr(dbNme)))
            {
                conn.Open();
                return await conn.DeleteAllAsync<T>();
            }
        }
        /// <summary>
        /// 带参数的存储过程
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<(int, List<T>)> StoredProcedurePageMultipleAsync(string dbNme, string proc, object? param = null)
        {
            (int, List<T>) dict;
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    var reads = await con.QueryMultipleAsync(proc, param, null, null, CommandType.StoredProcedure);
                    var count = reads.Read<int>().FirstOrDefault();
                    var lists = reads.Read<T>().ToList();
                    dict = (count, lists);
                    return dict;
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return (0, new List<T>());
                }
            }
        }
        /// <summary>
        /// 带参数的存储过程
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<List<T>?> StoredProcedureAsync(string dbNme, string proc, object? param = null)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnStr(dbNme)))
            {
                try
                {
                    List<T> list = (await con.QueryAsync<T>(proc, param, null, null, CommandType.StoredProcedure)).ToList();
                    return list;
                }
                catch (Exception ex)
                {
                    Loger.Error(typeof(DapperHelper), ex.Message, ex);
                    return null;
                }
            }
        }
        #endregion
    }

}