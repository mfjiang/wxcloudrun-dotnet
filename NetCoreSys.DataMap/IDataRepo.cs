namespace NetCoreSys.DataMap
{
    //Author    江名峰
    //Date      2019.04.18

    /// <summary>
    /// 一组数据实体存取操作方法
    /// </summary>
    public interface IDataRepo<TData>
    {
        /// <summary>
        /// 获取映射实体信息
        /// </summary>
        EntityInfo EntityInfo { get; }

        /// <summary>
        /// 获取SQL处理层
        /// </summary>
        object Dal { get; }
    }

    /// <summary>
    /// 一组数据实体存取操作方法，带有指定类型的DAL实例
    /// </summary>
    public interface IDataRepo<TData, TDal>
    {
        /// <summary>
        /// 获取映射实体信息
        /// </summary>
        EntityInfo EntityInfo { get; }

        /// <summary>
        /// 获取SQL处理层
        /// </summary>
        TDal Dal { get; }
    }
}
