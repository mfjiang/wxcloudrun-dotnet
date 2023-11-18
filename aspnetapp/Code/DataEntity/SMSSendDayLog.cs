namespace aspnetapp.Code.Entity
{
    //author        江名峰
    //date          2019.03.04

    /// <summary>
    ///发送给手机号的短信日志数据的实体类型
    /// </summary>
    public class SMSSendDayLog
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public ulong id { get; set; }

        /// <summary>
        /// 客户端ID
        /// </summary>
        public uint app_client_id { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public int product_id { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string? phone { get; set; }

        /// <summary>
        /// 年月日组成的数值 20190101
        /// </summary>
        public int the_day { get; set; }

        /// <summary>
        /// 当天发送次数
        /// </summary>
        public int send_times { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime last_update { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime created { get; set; }
    }
}
