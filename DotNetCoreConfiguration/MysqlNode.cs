namespace DotNetCoreConfiguration
{
    //Author    江名峰
    //Date      2019.03.14

    /// <summary>
    /// 表示用于json配置文件的MYSQL集群配置节点信息类
    /// </summary>
    public class MysqlNode
    {
        public MysqlNode() { }

        /// <summary>
        /// 节点在配置中的ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 是否为从库节点
        /// </summary>
        public bool IsSlave { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DataBasesName { get; set; }

        /// <summary>
        /// 连接串
        /// </summary>
        public string ConnStr { get; set; }

        /// <summary>
        /// 分库的主库ID（0表示非分库）
        /// </summary>
        public int DevideFromNodeID { get; set; }

        /// <summary>
        /// 分库的分表设置例：table 1:hash key,table 2:hash key,table n:hash key
        /// </summary>
        public string DevideDataSet { get; set; }

        /// <summary>
        /// 需要自动迁移的大表例：table_name=t1,key_name=key1,date_field=created,data_hold_days=30,archive_node_id=5,schedule_time=23:00:00:00;
        /// </summary>
        public string AutoMoveDataSet { get; set; }
    }
}
