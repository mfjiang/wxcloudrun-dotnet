using System;
using System.Collections.Generic;

namespace NetCoreSys.DataMap
{
    /// <summary>
    /// 表示映射实体信息
    /// </summary>
    public class EntityInfo
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; set; }
        /// <summary>
        /// DAL类型
        /// </summary>
        public Type DalType { get; set; }
        /// <summary>
        /// 用于分片的空间名
        /// <para>ID分片时 例： oauth_center.user</para>
        /// <para>时间分表时 例：201901</para>
        /// </summary>
        public string ShardNameSpace { get; set; }
        /// <summary>
        /// 数据名
        /// </summary>
        public string DataName { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string FirstKey { get; set; }
        /// <summary>
        /// 副键
        /// </summary>
        public string SecondKey { get; set; }
        /// <summary>
        /// 分片键
        /// </summary>
        public string ShardingKey { get; set; }
        /// <summary>
        /// 分片键类型
        /// </summary>
        public ShardingType ShardingKeyType { get; set; }
        /// <summary>
        /// 用于数据分片的数据库连接串，每片一个
        /// </summary>
        public List<object> ShardConnStrs { get; set; }

        /// <summary>
        /// 单片记录大小
        /// </summary>
        public uint MaxInShard { get; set; }

        /// <summary>
        /// 获取带分片命名空间的数据名
        /// </summary>
        public string FullShardDataName
        {
            get
            {
                if (this.ShardingKeyType == ShardingType.IDRange)
                {
                    return this.ShardNameSpace + "." + this.DataName;
                }
                else
                {
                    return this.DataName + "_" + this.ShardNameSpace;
                }
            }
        }
    }
}
