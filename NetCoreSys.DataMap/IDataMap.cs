using System;
using System.Collections.Generic;

namespace NetCoreSys.DataMap
{
    //Author    江名峰
    //Date      2019.04.18

    /// <summary>
    /// 表示数据实体映射集合
    /// </summary>
    public interface IDataMap
    {
        /// <summary>
        /// 数据库名
        /// </summary>
        string DBName { get; }
        /// <summary>
        /// 数据连接串
        /// </summary>
        string DBConnStr { get; set; }
        /// <summary>
        /// 映射实体信息清单
        /// </summary>
        List<EntityInfo> Entities { get; }
        /// <summary>
        /// 映射指定实体
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="dalType">DAL类型</param>
        /// <param name="dataName">数据名（表名）</param>
        /// <param name="firstKey">主键名</param>
        /// <param name="secondKey">第二主键名</param>
        /// <param name="shardingKey">分片键</param>
        /// <param name="shardingKeyType">分片键类型</param>
        void MapEntity(Type entityType, Type dalType, string dataName, string firstKey, string secondKey, string shardingKey, ShardingType shardingKeyType);

        /// <summary>
        /// 映射指定实体,配置分片信息
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="dalType">DAL类型</param>
        /// <param name="dataName">数据名（表名）</param>
        /// <param name="shardNameSpace">分片空间名</param>
        /// <param name="firstKey">主键名</param>
        /// <param name="secondKey">第二主键名</param>
        /// <param name="shardingKey">分片键</param>
        /// <param name="shardingKeyType">分片键类型</param>
        /// <param name="shardConnStrs">每个分片一个DB连接串</param>
        /// <param name="maxInShard">每片的记录数大小</param>
        void MapEntity(Type entityType, Type dalType, string dataName, string shardNameSpace, string firstKey, string secondKey, string shardingKey, ShardingType shardingKeyType, List<object> shardConnStrs, uint maxInShard);

        /// <summary>
        /// 创建对应数据实体的数据库操作类(不支持自动数据分片)
        /// </summary>
        /// <typeparam name="TData">实体类型</typeparam>
        /// <returns></returns>
        IDataRepo<TData> CreateRepo<TData>();

        /// <summary>
        ///  创建对应数据实体的数据库操作类(支持自动数据分片)
        /// </summary>
        /// <typeparam name="TData">实体类型</typeparam>
        /// <typeparam name="TDal">指定的DAL类型</typeparam>
        /// <returns></returns>
        IDataRepo<TData, TDal> CreateRepo<TData, TDal>() where TDal : class, IEntityInfoProp;

        /// <summary>
        /// 在当前应用域创建一个进程内的单一实例
        /// </summary>
        void Build();
    }
}
