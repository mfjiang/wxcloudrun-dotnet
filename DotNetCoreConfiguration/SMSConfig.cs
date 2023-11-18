using System;

namespace DotNetCoreConfiguration
{
    //Author    江名峰
    //Date      2019.04.15

    /// <summary>
    /// 表示短信接口配置(适合短信宝接口)
    /// </summary>
    [Serializable]
    public class SMSConfig
    {
        /// <summary>
        /// 是否启用短信接口
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 短信接口用户名
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 短信密码
        /// </summary>
        public string PWD { get; set; }

        /// <summary>
        /// 接口地址（?之前）
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// 短信前置标题
        /// </summary>
        public string Tittle { get; set; }

    }
}
