using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace DotNetCoreConfiguration
{
    /// <summary>
    /// 实现.net core环境的配置文件操作方法，以替代.net framework 中的同名类
    /// 默认使用当前目录下的appsettings.json文件
    /// </summary>
    public static class ConfigurationManager
    {
        #region 私有字段
        private static readonly IConfigurationBuilder m_ConfigBuilder;
        private static readonly IConfigurationRoot m_ConfigRoot;
        #endregion

        /// <summary>
        /// 获取JsonConfig对象
        /// </summary>
        public static IConfigurationRoot JsonConfig => m_ConfigRoot;

        static ConfigurationManager()
        {
            string path = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var jsonFileName = $"appsettings{(environment is null ? "" : $".{environment}")}.json";

            if (!File.Exists(Path.Combine(path, jsonFileName)))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string loc = assembly.Location;
                FileInfo ff = new FileInfo(loc);
                path = ff.DirectoryName;
            }

            m_ConfigBuilder = new ConfigurationBuilder().SetBasePath(path).AddJsonFile(jsonFileName);
            m_ConfigRoot = m_ConfigBuilder.Build();
        }

        /// <summary>
        ///以实体的形式读取AppSettings节点下的值  net core 3.0 及以下版本有效
        /// </summary>
        public static AppSettings GetAppConfig()
        {
            AppSettings cfg = new AppSettings();

            m_ConfigRoot.GetSection("AppSettings").Bind(cfg);

            return cfg;
        }


        /// <summary>
        /// 以实体的形式读取AppSettings节点下的值 net core 3.0 及以下版本有效
        /// </summary>
        /// <returns></returns>
        public static DataApiServiceSettings GetDataApiServiceSettings()
        {
            DataApiServiceSettings cfg = new DataApiServiceSettings();
            m_ConfigRoot.GetSection("AppSettings").Bind(cfg);
            return cfg;
        }


        /// <summary>
        /// 读取AppSettings节点下的指定KEY的值
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string GetAppConfig(string key)
        {
            string cfg = string.Empty;

            cfg = m_ConfigRoot.GetSection("AppSettings").GetValue<string>(key);

            return cfg;
        }

        /// <summary>
        /// 以实体的形式读取MySqlClusterSettings节点下的值
        /// </summary>
        /// <returns></returns>
        public static MySqlClusterSettings GetMySqlClusterSettings()
        {
            MySqlClusterSettings cfg = new MySqlClusterSettings();
            var ie = m_ConfigRoot.GetSection("MySqlClusterSettings:Nodes").GetChildren().GetEnumerator();
            int i = 0;
            while (ie.MoveNext())
            {
                string path = ie.Current.Path + ":MysqlNode";
                MysqlNode node = new MysqlNode();
                m_ConfigRoot.GetSection(path).Bind(node);
                cfg.Nodes.Add(node);
                i += 1;
            }
            return cfg;
        }

        /// <summary>
        /// 指定节点和键名取值
        /// </summary>
        /// <param name="sectionName">节点</param>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string GetValue(string sectionName, string key)
        {
            string cfg = string.Empty;

            var section = m_ConfigRoot.GetSection(sectionName);
            if (section != null)
            {
                cfg = section.GetValue<string>(key);
            }
            return cfg;
        }
    }
}
