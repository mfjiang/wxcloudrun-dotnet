using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreSys.Cache
{
    public class RedisConfiguration
    {
        /// <summary>
        /// ip地址或服务地址
        /// </summary>
        public string Redis_Host { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public string Redis_Port { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Redis_Password { get; set; }
        /// <summary>
        /// 指定数据库
        /// </summary>
        public int? Redis_DataBase { get; set; } = 0;
    }
}
