using MySqlConnector;
using System.Data;

namespace NetCoreMySqlUtility
{
    /// <summary>
    /// 为社区版MysqlConnector程序集的同名类增加扩展方法，以兼容Oracle的MySqlConnect/net驱动
    /// </summary>
    public static class MySqlHelperPlus
    {
        /// <summary>
        /// 替换Oracle驱动的同名方法
        /// </summary>
        /// <param name="conn">MySqlConnection</param>
        /// <param name="sql">sql text</param>
        /// <param name="parameters">parameters</param>
        /// <returns>返回DataSet</returns>
        public static DataSet ExecuteDataset(MySqlConnection conn, string sql, params MySqlParameter[] parameters)
        {
            DataSet ds = new DataSet();

            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(parameters);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }
            }

            return ds;
        }

        /// <summary>
        /// 替换Oracle驱动的同名方法
        /// </summary>
        /// <param name="conn">MySqlConnection</param>
        /// <param name="sql">sql text</param>
        /// <param name="parameters">parameters</param>
        /// <returns>返回影响行数</returns>
        public static int ExecuteNonQuery(MySqlConnection conn, string sql, params MySqlParameter[] parameters)
        {
            int r = 0;
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(parameters);
                r = cmd.ExecuteNonQuery();
            }
            return r;
        }

        /// <summary>
        /// 执行带有事务命令的SQL语句
        /// </summary>
        /// <param name="conn">MySqlConnection</param>
        /// <param name="sql">sql text</param>
        /// <param name="parameters">parameters</param>
        /// <returns></returns>
        public static int ExecuteTransactionNonQuery(MySqlConnection conn, string sql, params MySqlParameter[] parameters)
        {
            int r = 0;
            var mt = conn.BeginTransaction(IsolationLevel.Serializable);
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(parameters);
                r = cmd.ExecuteNonQuery();
            }
            mt.Commit();
            return r;
        }

        /// <summary>
        /// 替换Oracle驱动的同名方法
        /// </summary>
        /// <param name="conn">MySqlConnection</param>
        /// <param name="sql">sql text</param>
        /// <param name="parameters">parameters</param>
        /// <returns>返回Scalar</returns>
        public static object ExecuteScalar(MySqlConnection conn, string sql, params MySqlParameter[] parameters)
        {
            object r = null;

            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(parameters);
                r = cmd.ExecuteScalar();
            }

            return r;
        }

        /// <summary>
        /// 返回Reader
        /// </summary>
        /// <param name="conn">MySqlConnection</param>
        /// <param name="sql">sql text</param>
        /// <param name="parameters">parameters</param>
        /// <returns></returns>
        public static MySqlDataReader ExecuteReader(MySqlConnection conn, string sql, params MySqlParameter[] parameters)
        {
            MySqlDataReader reader = null;

            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                reader = cmd.ExecuteReader();
            }

            return reader;
        }
    }
}
