using System.Collections.Generic;

namespace DotNetCoreConfiguration
{
    //Author    江名峰
    //Date      2019.03.14

    /// <summary>
    /// 表示用于json配置文件的MYSQL集群配置信息类
    /// <para>分库是扩展存储容量</para>
    /// <para>从库是数据镜像和读分流</para>
    /// </summary>
    public class MySqlClusterSettings
    {
        private List<MysqlNode> m_Nodes;

        public MySqlClusterSettings()
        {
            m_Nodes = new List<MysqlNode>();
        }

        /// <summary>
        /// 配置中的节点清单
        /// </summary>
        public List<MysqlNode> Nodes { get => m_Nodes; set => m_Nodes = value; }

        /// <summary>
        /// 取主库
        /// </summary>
        /// <param name="dbname">数据库名称</param>
        /// <returns></returns>
        public MysqlNode GetMaster(string dbname)
        {
            var temp = m_Nodes.Find(n => n.IsSlave == false & n.DevideFromNodeID == 0 & n.DataBasesName.ToLower().Equals(dbname.ToLower()));
            return temp;
        }

        /// <summary>
        /// 取从库
        /// </summary>
        /// <param name="dbname">数据库名称</param>
        /// <returns></returns>
        public MysqlNode GetSlave(string dbname)
        {
            var temp = m_Nodes.Find(n => n.IsSlave == true & n.DevideFromNodeID == 0 & n.DataBasesName.ToLower().Equals(dbname.ToLower()));
            return temp;
        }

        /// <summary>
        /// 取分库
        /// </summary>
        /// <param name="dbname">数据库名称</param>
        /// <param name="devideFromMasterID">主库节点ID</param>
        /// <returns></returns>
        public MysqlNode GetDevide(string dbname, int devideFromMasterID)
        {
            var temp = m_Nodes.Find(n => n.DevideFromNodeID == devideFromMasterID & n.DataBasesName.ToLower().Equals(dbname.ToLower()));
            return temp;
        }
    }
}
