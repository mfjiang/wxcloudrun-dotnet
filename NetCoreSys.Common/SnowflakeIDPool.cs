using JMF.CodeLibrary.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreSys.Common
{
    //author    JMF
    //date      2020-07-03

    /// <summary>
    /// 在内存中维持一个ID池
    /// </summary>
    public static class SnowflakeIDPool
    {
        private static readonly Dictionary<string, SnowflakeIDCache> m_Pools;
        private static readonly object m_Locker;

        static SnowflakeIDPool()
        {
            m_Pools = new Dictionary<string, SnowflakeIDCache>();
            m_Locker = new object();
        }

        /// <summary>
        /// 初始化ID池
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="dataCenterID">数据中心ID 30以内的数</param>
        /// <param name="machineID">服务器ID 30以内的数</param>
        /// <param name="poolSize">池大小,1秒内大约2万个，超过2万可能导致生成期间阻塞明显</param>
        /// <param name="jumpNumber">跳数，每个服务实例应该不同，在多实例场景避免池内ID重复</param>
        public static void InitPool(string topic = "default", long machineID = 1, long dataCenterID = 1, int poolSize = 20000, int jumpNumber = 3)
        {
            var instance = Snowflake.Instance(machineID, dataCenterID);
            if (!m_Pools.ContainsKey(topic.ToLower()))
            {
                Queue<long> queue = new Queue<long>();
                for (int i = 0; i < poolSize; i++)
                {
                CreateID:
                    var id = instance.GetIdPlus() + jumpNumber;
                    if (queue.Contains(id))
                    {
                        goto CreateID;
                    }
                    queue.Enqueue(id);
                }
                //小到大重新排序
                queue = new Queue<long>(queue.OrderBy(o => o));
                SnowflakeIDCache cache = new SnowflakeIDCache
                {
                    MachineID = machineID,
                    DataCenterID = dataCenterID,
                    PoolSize = poolSize,
                    Queue = queue,
                    JumpNumber = jumpNumber
                };
                m_Pools.Add(topic.ToLower(), cache);
            }
        }

        /// <summary>
        /// 指定主题的ID池是否存在
        /// </summary>
        /// <param name="topic">主题</param>
        /// <returns></returns>
        public static bool HasPool(string topic = "default")
        {
            return m_Pools.ContainsKey(topic.ToLower());
        }

        /// <summary>
        /// 从ID池中取用ID
        /// </summary>
        /// <param name="topic">主题</param>
        /// <returns></returns>
        public static long GetID(string topic = "default")
        {
            long id = 0;

            if (m_Pools.ContainsKey(topic.ToLower()))
            {
                id = m_Pools[topic.ToLower()].Queue.Dequeue();
                FillPool(topic);
            }

            return id;
        }

        /// <summary>
        /// 显示缓存中的ID池状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> ShowPools()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            var ie = m_Pools.GetEnumerator();
            while (ie.MoveNext())
            {
                dic.Add(ie.Current.Key, $"{ie.Current.Value.Queue.Count}/{ie.Current.Value.PoolSize}");
            }

            return dic;
        }

        /// <summary>
        /// 按pool池大小填充不满的池
        /// </summary>
        /// <param name="topic">主题</param>
        private static void FillPool(string topic)
        {
            if (m_Pools.ContainsKey(topic.ToLower()))
            {
                var cache = m_Pools[topic.ToLower()];
                var queue = cache.Queue;
                var instance = Snowflake.Instance(cache.MachineID, cache.DataCenterID);
                //当ID消耗一半时进行填充
                if (queue.Count <= (cache.PoolSize / 2))
                {
                    lock (m_Locker)//填充池时短暂锁定
                    {
                        try
                        {
                            while (cache.Queue.Count < cache.PoolSize)
                            {
                            CreateID:
                                var id = instance.GetIdPlus() + cache.JumpNumber;
                                if (queue.Contains(id))
                                {
                                    goto CreateID;
                                }
                                queue.Enqueue(id);
                            }

                            //替换队列
                            //小到大重新排序
                            queue = new Queue<long>(queue.OrderBy(o => o));
                            cache.Queue = queue;
                            m_Pools[topic.ToLower()] = cache;
                        }
                        catch (Exception)
                        {
                            ;//任何异常释放锁
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// 表示ID缓存对象
    /// </summary>
    public class SnowflakeIDCache
    {
        public long MachineID { get; set; }

        public long DataCenterID { get; set; }

        public int PoolSize { get; set; }

        public int JumpNumber { get; set; }

        public Queue<long> Queue { get; set; }
    }
}
