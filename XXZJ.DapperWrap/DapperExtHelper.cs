using Dapper.Extension;
using DotNetCoreConfiguration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreSys.DapperWrap
{
    public class DapperExtHelper<T> where T : class
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
        static DapperExtHelper()
        {
            _dbConnection = new MySqlConnection();
        }

        private static readonly MySqlConnection _dbConnection;

        public List<T> GetAll(string DbNme)
        {
            _dbConnection.ConnectionString = GetConnStr(DbNme);
            return _dbConnection.GetAll<T>().ToList();
        }
        public async Task<List<T>> GetAllAsync(string DbNme)
        {
            _dbConnection.ConnectionString = GetConnStr(DbNme);
            return (await _dbConnection.GetAllAsync<T>()).ToList();
        }
        public T Get(int id, string DbNme)
        {
            _dbConnection.ConnectionString = GetConnStr(DbNme);
            return _dbConnection.Get<T>(id);
        }

        public bool Update(T entity, string DbNme)
        {
            _dbConnection.ConnectionString = GetConnStr(DbNme);
            return _dbConnection.Update(entity);
        }

        public async Task<bool> UpdateAsync(T entity, string DbNme)
        {
            _dbConnection.ConnectionString = GetConnStr(DbNme);
            return await _dbConnection.UpdateAsync(entity);
        }
        public long Insert(T entity, string DbNme)
        {
            _dbConnection.ConnectionString = GetConnStr(DbNme);
            return _dbConnection.Insert(entity);
        }
        public async Task<long> InsertAsync(T entity, string DbNme)
        {
            _dbConnection.ConnectionString = GetConnStr(DbNme);
            return await _dbConnection.InsertAsync(entity);
        }

        public bool Delete(T entity, string DbNme)
        {
            _dbConnection.ConnectionString = GetConnStr(DbNme);
            return _dbConnection.Delete(entity);
        }
        public async Task<bool> DeleteAsync(T entity, string DbNme)
        {
            _dbConnection.ConnectionString = GetConnStr(DbNme);
            return await _dbConnection.DeleteAsync(entity);
        }
        public bool DeleteAll(string DbNme)
        {
            _dbConnection.ConnectionString = GetConnStr(DbNme);
            return _dbConnection.DeleteAll<T>();
        }
        public async Task<bool> DeleteAllAsync(string DbNme)
        {
            _dbConnection.ConnectionString = GetConnStr(DbNme);
            return await _dbConnection.DeleteAllAsync<T>();
        }
    }
}
