namespace LogMan
{
    //Author    江名峰
    //Date      2015.09.07

    /// <summary>
    /// 表示输出日志的级别
    /// </summary>
    public enum LogLevel : int
    {
        /// <summary>
        /// 未配置
        /// </summary>
        Unknown,
        /// <summary>
        /// 什么也不输出
        /// </summary>
        None,
        /// <summary>
        /// 一般消息
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 一般异常
        /// </summary>
        Error,
        /// <summary>
        /// 致命异常
        /// </summary>
        Fatal
    }
}
