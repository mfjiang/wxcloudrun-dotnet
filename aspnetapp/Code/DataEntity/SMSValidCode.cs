using System;
using System.Collections.Generic;
using System.Text;

namespace aspnetapp.Code.Entity
{
    //author        江名峰
    //date          2019.03.04

    /// <summary>
    /// 表示短信验证码发送记录的数据实体类型
    /// </summary>
    public class SMSValidCode
    {
        /// <summary>
        /// 当前会话ID
        /// </summary>
        public string? session_id { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string? phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string? code { get; set; }

        /// <summary>
        /// 验证状态 0未验证，1已经验证，3过期
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string? ip { get; set; }

        /// <summary>
        /// 客户端ID
        /// </summary>
        public uint app_client_id { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public int product_id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime created { get; set; }
    }
}
