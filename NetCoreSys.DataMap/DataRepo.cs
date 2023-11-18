using System;

namespace NetCoreSys.DataMap
{
    //Author    江名峰
    //Date      2019.04.18

    /// <summary>
    /// 实现IDataRepo接口的类
    /// </summary>
    /// <typeparam name="TData">数据实体类型</typeparam>
    public class DataRepo<TData> : IDataRepo<TData>
    {
        private readonly EntityInfo m_EntityInfo;
        private readonly object m_Dal;

        public DataRepo(EntityInfo entityInfo, string dbconn)
        {
            m_EntityInfo = entityInfo;
            m_Dal = Activator.CreateInstance(m_EntityInfo.DalType, dbconn);
        }

        public EntityInfo EntityInfo => m_EntityInfo;

        public object Dal => m_Dal;
    }

    /// <summary>
    /// 实现IDataRepo接口的类
    /// </summary>
    /// <typeparam name="TData">数据实体类型</typeparam>
    /// <typeparam name="TDal">指定的DAL类型</typeparam>
    public class DataRepo<TData, TDal> : IDataRepo<TData, TDal>
    {
        private readonly EntityInfo m_EntityInfo;
        private readonly TDal m_Dal;

        public DataRepo(EntityInfo entityInfo, string dbconn)
        {
            m_EntityInfo = entityInfo;
            m_Dal = (TDal)Activator.CreateInstance(m_EntityInfo.DalType, dbconn);
        }

        public EntityInfo EntityInfo => m_EntityInfo;

        public TDal Dal => m_Dal;
    }
}
