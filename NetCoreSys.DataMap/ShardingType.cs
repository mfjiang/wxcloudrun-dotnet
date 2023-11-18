namespace NetCoreSys.DataMap
{
    /// <summary>
    /// 用于分片的主键类型
    /// </summary>
    public enum ShardingType : int
    {
        /// <summary>
        /// 不分片
        /// </summary>
        None = 0,
        /// <summary>
        /// 按自增ID分片，ID要为ULong类型（已实现）
        /// </summary>
        IDRange = 1,
        /// <summary>
        /// 按年月表分片（未实现）
        /// </summary>
        YearMonthTable = 2,
        /// <summary>
        /// 按键哈希值（未实现）
        /// </summary>
        KeyHash
    }
}
