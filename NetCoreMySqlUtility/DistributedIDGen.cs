using MySqlConnector;
using System;

namespace NetCoreMySqlUtility
{
    //Author    江名峰
    //Date      2019.04.18

    /// <summary>
    /// 分布式ID生成器
    /// </summary>
    public static class DistributedIDGen
    {
        private static MySqlConnection m_Conn;
        private static MySqlCommand m_Cmd;
        private static bool m_TableCreated;
        private static string m_ConnStr;

        private static readonly string m_cmd_create_table = @"SET FOREIGN_KEY_CHECKS=0;
create table if not exists `distributed_id` (  
  `data_name` varchar(255) NOT NULL,
  `id` bigint(20) unsigned NOT NULL DEFAULT(1),
  PRIMARY KEY (`data_name`),
  KEY `idx_name_id` (`id`,`data_name`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;";

        static DistributedIDGen()
        {
            m_TableCreated = false;
        }

        /// <summary>
        /// 在应用进程初始化时调用，建立到分布式ID数据库的连接
        /// </summary>
        /// <param name="conn">ID数据库的连接串</param>
        public static void SetupOnAppStartUp(string connstr)
        {
            try
            {
                m_ConnStr = connstr;
                AutoCreateTable();
            }
            catch (Exception ex)
            {
                throw new Exception("can't setup a connection to distributed id db", ex);
            }
        }

        /// <summary>
        /// 设置ID种子（只在第一次设置有效）
        /// </summary>
        /// <param name="dataName">格式：分片空间名.数据表名,比如 oauth_center.user</param>
        /// <param name="seed">ID起始值</param>
        public static ulong SetupSeed(string dataName, ulong seed)
        {
            if (string.IsNullOrEmpty(m_ConnStr)) { throw new Exception("is not connect a distributed id db"); }
            if (m_Conn == null) { m_Conn = new MySqlConnection(m_ConnStr); }
            if (m_Cmd == null) { m_Cmd = m_Conn.CreateCommand(); }
            m_Cmd.CommandType = System.Data.CommandType.Text;
            string commandText = string.Format("insert into `distributed_id` (`data_name`,`id`) values('{0}',{1}) ON DUPLICATE KEY UPDATE `id`=`id`;select `id` from `distributed_id` where `data_name`='{0}';", dataName, seed);
            m_Cmd.CommandText = commandText;
            ulong id;
            try
            {
                id = (ulong)m_Cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("generic seed id failed,data name:" + dataName, ex);
            }

            return id;
        }

        /// <summary>
        /// 取分布式ID（25~50个ID/秒，取决于硬件和网络性能）
        /// </summary>
        /// <param name="dataName">格式：分片空间名.数据表名 比如 oauth_center.user</param>
        /// <returns></returns>
        public static ulong GetID(string dataName)
        {
            if (string.IsNullOrEmpty(m_ConnStr)) { throw new Exception("is not connect a distributed id db"); }
            if (m_Conn == null) { m_Conn = new MySqlConnection(m_ConnStr); m_Conn.Open(); }
            if (m_Cmd == null) { m_Cmd = m_Conn.CreateCommand(); }
            m_Cmd.CommandType = System.Data.CommandType.Text;
            string commandText = string.Format("insert into `distributed_id` (`data_name`) values('{0}') ON DUPLICATE KEY UPDATE `id`=`id`+1;select `id` from `distributed_id` where `data_name`='{0}';", dataName);
            m_Cmd.CommandText = commandText;
            ulong id;
            try
            {
                if (m_Conn.State != System.Data.ConnectionState.Open) { m_Conn.Open(); }
                id = (ulong)m_Cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("generic new id failed", ex);
            }
            finally
            {
                m_Conn.Close();
            }
            return id;
        }

        /// <summary>
        /// 自动建表
        /// </summary>
        private static void AutoCreateTable()
        {
            if (m_Conn == null)
            {
                m_Conn = new MySqlConnection(m_ConnStr);
                m_Conn.Open();
            }

            //如果没建表自动建表
            if (!m_TableCreated)
            {
                m_Cmd = m_Conn.CreateCommand();
                m_Cmd.CommandText = m_cmd_create_table;
                m_Cmd.CommandType = System.Data.CommandType.Text;
                m_Cmd.ExecuteNonQuery();
                m_TableCreated = true;
                m_Cmd.CommandText = "";
            }
        }
    }
}
