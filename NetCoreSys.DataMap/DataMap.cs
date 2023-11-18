using System;
using System.Collections.Generic;

namespace NetCoreSys.DataMap
{
    //Author    江名峰
    //Date      2019.04.18

    /// <summary>
    /// 表示实现IDataMap接口的类
    /// </summary>
    public class DataMap : IDataMap
    {
        private readonly List<EntityInfo> m_Entities = new List<EntityInfo>();
        private readonly string m_DBName;

        public DataMap(string dbName)
        {
            m_DBName = dbName;
        }

        /// <summary>
        /// 添加IDataMap单例
        /// <para>IDataMap map = DataMap.AddSingleton(...).Config(m=>{m.MapEntity(...)....;m.Build();})</para>
        /// </summary>
        /// <param name="dbName">数据库名</param>
        /// <returns></returns>
        public static IDataMap AddSingleton(string dbName)
        {
            if (string.IsNullOrEmpty(dbName)) { throw new ArgumentNullException("dbName"); }
            IDataMap dataMap = new DataMap(dbName.ToLower());
            AppDomain.CurrentDomain.SetData(typeof(DataMap).FullName + "." + dbName, dataMap);
            return dataMap;
        }

        /// <summary>
        /// 取得IDataMap单例
        /// </summary>
        /// <param name="dbName">数据库名</param>
        /// <returns></returns>
        public static IDataMap Get(string dbName)
        {
            if (string.IsNullOrEmpty(dbName)) { throw new ArgumentNullException("dbName"); }
            IDataMap dataMap = null;
            dataMap = (IDataMap)AppDomain.CurrentDomain.GetData(typeof(DataMap).FullName + "." + dbName.ToLower());
            return dataMap;
        }

        string IDataMap.DBName => m_DBName;

        string IDataMap.DBConnStr { get; set; }

        List<EntityInfo> IDataMap.Entities => m_Entities;

        void IDataMap.MapEntity(Type entityType, Type dalType, string dataName, string firstKey, string secondKey, string shardingKey, ShardingType shardingKeyType)
        {
            m_Entities.Add(new EntityInfo() { EntityType = entityType, DalType = dalType, DataName = dataName, FirstKey = firstKey, SecondKey = secondKey, ShardingKey = shardingKey, ShardingKeyType = shardingKeyType });
        }

        void IDataMap.MapEntity(Type entityType, Type dalType, string dataName, string shardNameSpace, string firstKey, string secondKey, string shardingKey, ShardingType shardingKeyType, List<object> shardConnStrs, uint maxInShard)
        {
            m_Entities.Add(new EntityInfo() { EntityType = entityType, DalType = dalType, DataName = dataName, ShardNameSpace = shardNameSpace, FirstKey = firstKey, SecondKey = secondKey, ShardingKey = shardingKey, ShardingKeyType = shardingKeyType, ShardConnStrs = shardConnStrs, MaxInShard = maxInShard });
        }

        void IDataMap.Build()
        {
            AppDomain.CurrentDomain.SetData(typeof(DataMap).FullName + "." + m_DBName, this);
        }

        IDataRepo<TData> IDataMap.CreateRepo<TData>()
        {
            foreach (EntityInfo tinfo in m_Entities)
            {
                if (tinfo.EntityType.Equals(typeof(TData)))
                {
                    return new DataRepo<TData>(tinfo, ((IDataMap)this).DBConnStr);
                }
            }
            return null;
        }

        IDataRepo<TData, TDal> IDataMap.CreateRepo<TData, TDal>()
        {
            foreach (EntityInfo tinfo in m_Entities)
            {
                if (tinfo.EntityType.Equals(typeof(TData)))
                {
                    var repo = new DataRepo<TData, TDal>(tinfo, ((IDataMap)this).DBConnStr);
                    repo.Dal.EntityInfo = tinfo;
                    return repo;
                }
            }
            return null;
        }
    }
}
