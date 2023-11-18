using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using DotNetCoreConfiguration;
//using Newtonsoft.Json;

namespace NetCoreSys.Cache
{
    //Author    江名峰
    //Date      2019.04.02

    /// <summary>
    /// 表示Redis缓存处理服务的类
    /// </summary>
    public class RedisCacheService
    {
        #region 字段
        private ConnectionMultiplexer m_Conn;
        private IDatabase m_RedisDB;
        //只适用于redis链接字符串中含有端口和密码，如果需要指定数据库需要用到ConfigurationOptions
        private string m_ConnStr;
        private ConfigurationOptions options;
        private AppSettings m_AppSettings = DotNetCoreConfiguration.ConfigurationManager.GetAppConfig();
        #endregion

        /// <summary>
        /// 返回RedisCacheService实例
        /// </summary>
        /// <param name="connStr">redis连接串</param>
        public RedisCacheService(string connStr)
        {
            if (String.IsNullOrEmpty(connStr)) { throw new ArgumentNullException("connStr"); };
            m_ConnStr = connStr;
            //m_Conn = ConnectionMultiplexer.Connect(m_ConnStr);
            //m_RedisDB = m_Conn.GetDatabase();
        }
        private RedisConfiguration InitRedisConfiguration()
        {
            RedisConfiguration config = new RedisConfiguration();
            config.Redis_Port = m_AppSettings.Redis_Port;
            config.Redis_Host = m_AppSettings.Redis_Host;
            config.Redis_Password = m_AppSettings.Redis_Password;
            config.Redis_DataBase = m_AppSettings.Redis_DataBase;
            return config;
        }
        public RedisCacheService()
        {
            var config = InitRedisConfiguration();
            options = ConvertConfigToConfigurationOptions(config);
        }
        public RedisCacheService(RedisConfiguration config)
        {
            options = ConvertConfigToConfigurationOptions(config);
        }
        /// <summary>
        /// 对配置文件转换redis可读选择项连接实体
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private ConfigurationOptions ConvertConfigToConfigurationOptions(RedisConfiguration config)
        {
            if (config is null)
            {
                return null;
            }
            else
            {
                var fg = new ConfigurationOptions
                {
                    KeepAlive = 180,
                    Password = config.Redis_Password,
                    DefaultVersion = new Version("2.8.5"),// Needed for cache clear
                    DefaultDatabase = config.Redis_DataBase,
                    AllowAdmin = true
                };
                if (string.IsNullOrEmpty(config.Redis_Port))
                {
                    fg.EndPoints.Add(config.Redis_Host);
                }
                else
                    fg.EndPoints.Add(config.Redis_Host, int.Parse(config.Redis_Port));
                return fg;
            }
        }
        //public static string GetRedisDb(string host, string point, string pass, int db)
        //{
        //    ConfigurationOptions options = new ConfigurationOptions
        //    {
        //        EndPoints = { host, point },
        //        KeepAlive = 180,
        //        Password = pass,
        //        DefaultVersion = new Version("2.8.5"),// Needed for cache clear
        //        DefaultDatabase = db,
        //        AllowAdmin = true
        //    };
        //    var m = ConnectionMultiplexer.Connect(options);
        //    return m.GetDatabase().Database.ToString();
        //}

        /// <summary>
        /// 添加或覆盖缓存实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="entity">实体</param>
        /// <param name="expiry">有效时长</param>
        public void Set<T>(string key, T entity, TimeSpan? expiry)
        {
            if (!String.IsNullOrEmpty(key) && entity != null)
            {
                //string jsondata = JsonConvert.SerializeObject(entity);
                if (options != null)
                {
                    m_Conn = ConnectionMultiplexer.Connect(options);
                }
                else
                {
                    m_Conn = ConnectionMultiplexer.Connect(m_ConnStr);
                }

                m_RedisDB = m_Conn.GetDatabase();
                string jsondata = JsonSerializer.Serialize<T>(entity);
                m_RedisDB.StringSet(key, jsondata, expiry);
                m_Conn.Close();
            }
        }

        /// <summary>
        /// 添加或覆盖缓存文本数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">文本数据</param>
        /// <param name="expiry">有效时长</param>
        public void Set(string key, string data, TimeSpan? expiry)
        {
            if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(data))
            {
                if (options != null)
                {
                    m_Conn = ConnectionMultiplexer.Connect(options);
                }
                else
                {
                    m_Conn = ConnectionMultiplexer.Connect(m_ConnStr);
                }
                m_RedisDB = m_Conn.GetDatabase();
                m_RedisDB.StringSet(key, data, expiry);
                m_Conn.Close();
            }
        }

        /// <summary>
        /// 从缓存读取指定实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>缓存中的数据实体</returns>
        public object Get<T>(string key) where T : class
        {
            T obj = null;

            if (!String.IsNullOrEmpty(key))
            {
                if (options != null)
                {
                    m_Conn = ConnectionMultiplexer.Connect(options);
                }
                else
                {
                    m_Conn = ConnectionMultiplexer.Connect(m_ConnStr);
                }
                m_RedisDB = m_Conn.GetDatabase();
                if (m_RedisDB.KeyExists(key))
                {
                    string jsondata = m_RedisDB.StringGet(key);
                    if (!String.IsNullOrEmpty(jsondata))
                    {
                        obj = JsonSerializer.Deserialize<T>(jsondata, null);
                    }
                }
                m_Conn.Close();
            }

            return obj;
        }

        /// <summary>
        /// 从缓存中取指定文本
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public string Get(string key)
        {
            string data = String.Empty;

            if (!String.IsNullOrEmpty(key))
            {
                if (options != null)
                {
                    m_Conn = ConnectionMultiplexer.Connect(options);
                }
                else
                {
                    m_Conn = ConnectionMultiplexer.Connect(m_ConnStr);
                }
                m_RedisDB = m_Conn.GetDatabase();
                if (m_RedisDB.KeyExists(key))
                {
                    data = m_RedisDB.StringGet(key);
                }
                m_Conn.Close();
            }

            return data;
        }

        /// <summary>
        /// 取KEY*匹配的值
        /// </summary>
        /// <param name="key">*号之前的值</param>
        /// <returns></returns>
        public string GetInKeyPattern(string key)
        {
            string data = String.Empty;
            string machKey = GetKeys($"{key}*", 1).FirstOrDefault();
            if (!String.IsNullOrEmpty(machKey))
            {
                data = Get(machKey);
            }

            return data;
        }

        /// <summary>
        /// 查找匹配的KEY
        /// </summary>
        /// <param name="keyPattern">KEY的匹配模式</param>
        /// <param name="limit">最大返回数</param>
        /// <returns></returns>
        public List<string> GetKeys(string keyPattern, int limit)
        {
            List<string> keys = new List<string>();

            if (!String.IsNullOrEmpty(keyPattern))
            {
                if (options != null)
                {
                    m_Conn = ConnectionMultiplexer.Connect(options);
                }
                else
                {
                    m_Conn = ConnectionMultiplexer.Connect(m_ConnStr);
                }
                m_RedisDB = m_Conn.GetDatabase();
                //var fs = m_Conn.GetServer(m_ConnStr).Features;
                var ks = (options != null ? m_Conn.GetServer(options.EndPoints[0]) : m_Conn.GetServer(m_ConnStr)).Keys(m_RedisDB.Database, keyPattern, limit * 2, CommandFlags.None).Take(limit);
                //var ks = m_Conn.GetServer(m_ConnStr).Keys(m_RedisDB.Database, keyPattern);
                //m_RedisDB.
                //r = m_RedisDB.Execute(String.Format("SCAN 0 MATCH {0} COUNT {1}]", keyPattern,limit),keyPattern,limit);W
                var em = ks.GetEnumerator();
                if (em != null&& em.Current.ToString()!="(null)")
                {

                    while (em.MoveNext())
                    {
                        keys.Add(em.Current.ToString());
                    }
                }
                m_Conn.Close();
            }
            return keys;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            bool done = false;
            if (options != null)
            {
                m_Conn = ConnectionMultiplexer.Connect(options);
            }
            else
            {
                m_Conn = ConnectionMultiplexer.Connect(m_ConnStr);
            }
            m_RedisDB = m_Conn.GetDatabase();
            if (!String.IsNullOrEmpty(key))
            {
                done = m_RedisDB.KeyDelete(key);
            }
            m_Conn.Close();

            return done;
        }

        /// <summary>
        /// 向一个命名的缓存队列尾部添加元素，如果队列不存在会自动创建
        /// </summary>
        /// <typeparam name="T">可序列化消息类型</typeparam>
        /// <param name="msgModel">消息模型</param>
        /// <param name="listKey">队列名(唯一)</param>
        public async void SetMQ<T>(T msgModel, string listKey)
        {
            if (msgModel != null && !String.IsNullOrEmpty(listKey))
            {
                if (options != null)
                {
                    m_Conn = ConnectionMultiplexer.Connect(options);
                }
                else
                {
                    m_Conn = ConnectionMultiplexer.Connect(m_ConnStr);
                }
                m_RedisDB = m_Conn.GetDatabase();
                //string jsondata = JsonConvert.SerializeObject(msgModel);
                string jsondata = JsonSerializer.Serialize(msgModel);
                //向队列尾部添加元素
                await m_RedisDB.ListRightPushAsync(jsondata, listKey);
                m_Conn.Close();
            }
        }

        /// <summary>
        /// 从一个命名的缓存队列头部取出元素
        /// </summary>
        /// <typeparam name="T">可序列化消息类型</typeparam>
        /// <param name="listKey">队列名(唯一)</param>
        /// <returns></returns>
        public async Task<object> ConsumeMQ<T>(string listKey)
        {
            object obj = null;

            if (!String.IsNullOrEmpty(listKey))
            {
                if (options != null)
                {
                    m_Conn = ConnectionMultiplexer.Connect(options);
                }
                else
                {
                    m_Conn = ConnectionMultiplexer.Connect(m_ConnStr);
                }
                m_RedisDB = m_Conn.GetDatabase();
                if (m_RedisDB.KeyExists(listKey))
                {
                    //从队列头部取出元素
                    string jsondata = await m_RedisDB.ListLeftPopAsync(listKey);
                    if (!String.IsNullOrEmpty(jsondata))
                    {
                        //obj = JsonConvert.DeserializeObject<T>(jsondata);
                        obj = JsonSerializer.Deserialize<T>(jsondata);
                    }
                }
                m_Conn.Close();
            }

            return obj;
        }
    }
}
