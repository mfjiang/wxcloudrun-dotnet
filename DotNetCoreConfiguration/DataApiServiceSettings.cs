namespace DotNetCoreConfiguration
{
    /// <summary>
    /// 表示数据交换服务配置信息类
    /// </summary>
    public class DataApiServiceSettings : AppSettings
    {
        /// <summary>
        /// 是否启用数据分片
        /// </summary>
        public bool EnableDataSharding { get; set; }

        /// <summary>
        /// 数据分片配置文件位置
        /// </summary>
        public string DataShardingConfigPath { get; set; }
    }
}
