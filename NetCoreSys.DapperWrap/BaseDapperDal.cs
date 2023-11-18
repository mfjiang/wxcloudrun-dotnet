using Dapper.Extension.AspNetCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreSys.DapperWrap
{
    public class BaseDapperDal<TEntity> where TEntity : class, new()
    {
        #region 字段
        /// <summary>
        /// 数据表名/对象名
        /// </summary>
        public string m_ObjectName = string.Empty;
        /// <summary>
        /// 主键名
        /// </summary>
        public string m_FirstKeyName = string.Empty;
        /// <summary>
        /// 联合主键名
        /// </summary>
        public string m_SecondKeyName = string.Empty;
        #endregion
        ///// <summary>
        ///// netcore dapper
        ///// </summary>
        //private IDapper _dapper;
        /// <summary>
        /// 要查询的数据库
        /// </summary>

        private string DbNme = string.Empty;

        ///// <summary>
        ///// core 调用dapper构造函数
        ///// </summary>
        ///// <param name="dapper"></param>
        //public BaseDapperDal(IDapper dapper, string dbname)
        //{
        //    _dapper = dapper;
        //    DbNme = dbname;

        //}
        /// <summary>
        /// 通用构造函数
        /// </summary>
        /// <param name="dbname"></param>
        public BaseDapperDal(string dbname)
        {
            DbNme = dbname;
        }


        #region 根据实体直接执行增加删除，查询更新（适合.net 5\6, .net core 3.1）
        /// <summary>
        /// 返回值是对应表添加的对应记录的主键值 （适合.net 5\6）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long AddWithTableName(TEntity model, bool isautoid = true)
        {
            var res = DapperHelper<TEntity>.INSERT(DbNme, m_ObjectName, model, isautoid);
            return res;

        }
        // <summary>
        /// 返回值是对应表添加的对应记录的主键值 （适合.net 5\6）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long Add(TEntity model)
        {
            var res = DapperHelper<TEntity>.INSERT(DbNme, model);
            return res;

        }
        /// <summary>
        /// 删除对应实体数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Del(TEntity model)
        {
            return DapperHelper<TEntity>.DELETE(DbNme, model);
        }
        /// <summary>
        /// 分页获取实体列表数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<TEntity> GetList(int pageIndex, int pageSize)
        {
            var result = DapperHelper<TEntity>.GetDataAll(DbNme, m_ObjectName, pageIndex, pageSize);
            return result is null ? new List<TEntity>() : result;
        }
        /// <summary>
        /// 调用分页存储过程获取列表数据和总数
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderStr"></param>
        /// <param name="where_string"></param>
        /// <returns></returns>
        public (int, List<TEntity>) GetPageListProc(string fields, int pageIndex, int pageSize, string orderStr = "", string where_string = "")
        {
           
            return GetPageListProc(fields,DbNme, pageIndex, pageSize, orderStr, where_string);
        }
        public (int, List<TEntity>) GetPageListProc(string fields,string tablename, int pageIndex, int pageSize, string orderStr, string where_string)
        {
            var result = DapperHelper<TEntity>.StoredProcedurePageMultiple(tablename, "get_data_pager", new
            {
                p_table_name = m_ObjectName,
                p_fields = fields,
                p_page_now = pageIndex,
                p_page_size = pageSize,
                p_order_string = orderStr,
                p_where_string = where_string
            });
            return result;
        }
        public (int, List<OtherEntity>) GetPageListProc<OtherEntity>(string fields, string tablename, int pageIndex, int pageSize, string orderStr, string where_string)
        {
            var result = DapperHelper<TEntity>.StoredProcedurePageMultiple<OtherEntity>(DbNme, "get_data_pager", new
            {
                p_table_name = tablename,
                p_fields = fields,
                p_page_now = pageIndex,
                p_page_size = pageSize,
                p_order_string = orderStr,
                p_where_string = where_string
            });
            return result;
        }
        /// <summary>
        /// 更新对应实体数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(TEntity model)
        {
            return DapperHelper<TEntity>.UPDATE(DbNme, model);
        }

        #endregion

        #region 根据sql操作
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="dbNme">查询的数据库名称</param>
        /// <param name="sql">查询的sql</param>
        /// <param name="param">替换参数</param>
        /// <returns></returns>
        public List<TEntity>? QueryToList(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.QueryToList(DbNme, sql, param);
        }
        /// <summary>
        /// 根据查询sql语句查询数据集
        /// </summary>
        /// <param name="dbNme"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<TEntity> SELECT(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.SELECT(DbNme, sql, param) ?? new List<TEntity>();
        }
        public List<OtherTEntity> SelectDtos<OtherTEntity>(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.SelectDtos<OtherTEntity>(DbNme, sql, param);
        }
        /// <summary>
        /// 返回datatable数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public DataTable QueryTable(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.QueryTable(DbNme, sql, param);
        }
        /// <summary>
        /// 根据sql查询返回第一条数据
        /// </summary>
        /// <param name="dbNme"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public TEntity? QueryFirst(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.QueryFirst(DbNme, sql, param);
        }
        /// <summary>
        /// 根据sql查询返回第一条数据
        /// </summary>
        /// <param name="dbNme"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public TEntity? QueryFirstOrDefault(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.QueryFirstOrDefault(DbNme, sql, param);
        }
        /// <summary>
        /// 根据sql查询返回第一条数据
        /// </summary>
        /// <param name="dbNme"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public TEntity? TOP_1(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.TOP_1(DbNme, sql, param);
        }
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        public TEntity? QuerySingle(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.QuerySingle(DbNme, sql, param);
        }
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        public TEntity? QuerySingleOrDefault(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.QuerySingleOrDefault(DbNme, sql, param);
        }
        /// <summary>
        /// 返回其他结果数据集
        /// </summary>
        /// <typeparam name="OtherTEntity">其他结果数据集</typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public OtherTEntity? QuerySingleOrDefault<OtherTEntity>(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.QuerySingleOrDefault<OtherTEntity>(DbNme, sql, param);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        public bool INSERTBySql(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.INSERT(DbNme, sql, param);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="dbNme"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool UpdateBySql(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.UPDATE(DbNme, sql, param);
        }
        /// <summary>
        /// 删除所有数据
        /// </summary>
        /// <param name="DbNme"></param>
        /// <returns></returns>
        public bool DeleteAllBySql()
        {
            return DapperHelper<TEntity>.DeleteAll(DbNme);
        }
        /// <summary>
        /// 返回单结果实体
        /// </summary>
        /// <param name="dbNme"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public TEntity? ExecuteScalarReturnEntity(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.ExecuteScalarReturnEntity(DbNme, sql, param);
        }
        /// <summary>
        /// 返回但结果Object对象
        /// </summary>
        /// <param name="dbNme"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object? ExecuteScalarReturnObject(string sql, object? param = null)
        {
            return DapperHelper<TEntity>.ExecuteScalarReturnObject(DbNme, sql, param);
        }
        /// <summary>
        /// 带参数的存储过程
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<TEntity>? StoredProcedure(string proc, object? param = null)
        {
            return DapperHelper<TEntity>.StoredProcedure(DbNme, proc, param);
        }
        /// <summary>
        /// 查询分页存储过程
        /// </summary>
        /// <param name="dbNme"></param>
        /// <param name="proc"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public (int, List<TEntity>) StoredProcedurePageMultiple(string proc, object? param = null)
        {
            return DapperHelper<TEntity>.StoredProcedurePageMultiple(DbNme, proc, param);
        }
        /// <summary>
        /// 执行事务过程
        /// </summary>
        /// <param name="dbNme"></param>
        /// <param name="sqlarr"></param>
        /// <returns></returns>
        public int ExecuteTransaction(string[] sqlarr)
        {
            return DapperHelper<TEntity>.ExecuteTransaction(DbNme, sqlarr);
        }
        /// <summary>
        /// 执行事务带参数
        /// </summary>
        /// <param name="dbNme"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public int ExecuteTransaction(Dictionary<string, object> dic)
        {
            return DapperHelper<TEntity>.ExecuteTransaction(DbNme, dic);
        }
        #endregion

        #region 异步操作
        public async Task<List<TEntity>> GetAllAsync()
        {
            return (await DapperHelper<TEntity>.GetAllAsync(DbNme)).ToList();
        }

        public Task<bool> UpdateAsync(TEntity entity)
        {
            return DapperHelper<TEntity>.UpdateAsync(entity, DbNme);
        }
        public Task<long> InsertAsync(TEntity entity)
        {
            return DapperHelper<TEntity>.InsertAsync(entity, DbNme);
        }
        public Task<bool> DeleteAsync(TEntity entity)
        {
            return DapperHelper<TEntity>.DeleteAsync(entity, DbNme);
        }
        public Task<bool> DeleteAllAsync()
        {
            return DapperHelper<TEntity>.DeleteAllAsync(DbNme);
        }
        #endregion
    }
}
