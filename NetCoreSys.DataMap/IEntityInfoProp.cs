namespace NetCoreSys.DataMap
{
    /// <summary>
    /// 表示拥有实体信息属性的接口
    /// </summary>
    public interface IEntityInfoProp
    {
        /// <summary>
        /// 获取或设置映射实体信息
        /// </summary>
        EntityInfo EntityInfo { get; set; }
    }
}
