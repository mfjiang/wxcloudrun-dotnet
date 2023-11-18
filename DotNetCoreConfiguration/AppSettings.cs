namespace DotNetCoreConfiguration
{
    /// <summary>
    /// 对应appsettings.json中的AppSettings节
    /// </summary>
    public class AppSettings
    {
        public AppSettings() { }

        /// <summary>
        /// 日志路径
        /// </summary>
        public string LogManPath { get; set; }


        /// <summary>
        /// Redis连接串
        /// </summary>
        public string RedisConn { get; set; }
        /// <summary>
        /// Redis连接地址串
        /// </summary>
        public string Redis_Host { get; set; }
        /// <summary>
        /// Redis连接端口串
        /// </summary>
        public string Redis_Port { get; set; }
        /// <summary>
        /// Redis密码
        /// </summary>
        public string Redis_Password { get; set; }
        /// <summary>
        /// Redis连接数据库名
        /// </summary>
        public int? Redis_DataBase { get; set; }
        /// <summary>
        /// 代理服务器IP，多个用半角逗号分开
        /// </summary>
        public string Proxies { get; set; }

        /// <summary>
        /// 用户中心数据库名称
        /// </summary>
        public string OAuthCenterDBName { get; set; }
        ///// <summary>
        ///// 主数据库名称
        ///// </summary>
        //public string VMainDBName { get; set; }
        /// <summary>
        /// 分析数据库名称
        /// </summary>
        public string AnalysisDataDBName { get; set; }

        ///// <summary>
        ///// 文件中心数据库名称
        ///// </summary>
        //public string FileCenterDBName { get; set; }

        /// <summary>
        /// 分布式ID生成库名称
        /// </summary>
        public string DistributedIDDBName { get; set; }

        /// <summary>
        /// 账套数据库1#连接串,注意替换###为最终账套库名称
        /// </summary>
        public string DataBookConnStr_01 { get; set; }

        /// <summary>
        /// 是否启用短信接口
        /// </summary>
        public bool SMS_Enabled { get; set; } = false;

        ///// <summary>
        ///// 短信接口账号
        ///// </summary>
        //public string SMS_Account { get; set; }

        ///// <summary>
        ///// 短信接口账号密码
        ///// </summary>
        //public string SMS_PWD { get; set; }

        ///// <summary>
        ///// 短信接口地址(?之前)
        ///// </summary>
        //public string SMS_Uri { get; set; }

        ///// <summary>
        ///// 短信前置标题
        ///// </summary>
        //public string SMS_Tittle { get; set; }

        

        /// <summary>
        /// 当天限制发送短信次数
        /// </summary>
        public int SMS_SendTimes { get; set; }
        /// <summary>
        /// 短信过期时间间隔
        /// </summary>
        public int ExpireSmsTime { get; set; } = 5;
        /// <summary>
        /// token过期时间
        /// </summary>
        public int ExpireTokenTime { get; set; } = 10080;//默认7天
        

        /// <summary>
        /// HTTP监听URL 用于asp.net core 进程
        /// </summary>
        public string HttpListenUrl { get; set; }
        /// <summary>
        /// HTTPS监听URL  用于asp.net core 进程
        /// </summary>
        public string HttpsListenUrl { get; set; }

        /// <summary>
        /// 是否启用测试模式，不启用则不显示Swager在线文档、不执行api/test入口
        /// </summary>
        public bool EnableTestModel { get; set; }

        /// <summary>
        /// 微信小程序开放平台APP ID
        /// </summary>
        public string WeichatOpenAppID { get; set; }

        /// <summary>
        /// 微信小程序开放平台APP Secret
        /// </summary>
        public string WeichatOpenAppSecret { get; set; }
        /// <summary>
        ///  微信开放平台APP ID
        /// </summary>
        public string WeixiAppID { get; set; }

        /// <summary>
        /// 微信开放平台APP Secret
        /// </summary>
        public string WeixiAppSecret { get; set; }
        /// <summary>
        /// 微信api连接地址
        /// </summary>

        public string WeichatApiUrl { get; set; }

        /// <summary>
        /// 是否启用队列服务
        /// </summary>
        public bool EnableKafka { get; set; } = false;

        /// <summary>
        /// 队列主题
        /// </summary>
        public string KafkaTopic { get; set; }

        /// <summary>
        /// 队列服务器like 192.168.1.250:9092
        /// </summary>
        public string KafkaBrokerServers { get; set; }

        /// <summary>
        /// MQTT服务TCP地址
        /// </summary>
        public string MQTTBrokerTcpAddress { get; set; }
        /// <summary>
        /// MQTT服务端口
        /// </summary>
        public int MQTTPort { get; set; }
        /// <summary>
        /// MQTT客户端ID
        /// </summary>
        public string MQTTClientID { get; set; }
        /// <summary>
        /// MQTT用户名
        /// </summary>
        public string MQTTUserName { get; set; }
        /// <summary>
        /// MQTT用户密码
        /// </summary>
        public string MQTTUserPwd { get; set; }
        /// <summary>
        /// 订阅主题，多个用半角逗号分开
        /// </summary>
        public string MQTTTopics { get; set; }
        /// <summary>
        /// 默认头像地址
        /// </summary>

        public string DefaultHeaderUrl { get; set; }

        /// <summary>
        /// 返回短信配置实体
        /// </summary>
        /// <returns></returns>
        public SMSConfig GetSMSConfig()
        {
            return new SMSConfig()
            {
                //Account = this.SMS_Account,
                //Uri = this.SMS_Uri,
                Enabled = this.SMS_Enabled,
                //PWD = this.SMS_PWD,
                //Tittle = this.SMS_Tittle
            };
        }
    }
}
