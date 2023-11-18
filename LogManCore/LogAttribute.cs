using System;

namespace LogMan
{
    //Author    江名峰
    //Date      2015.09.07

    /// <summary>
    /// 表示自定义日志输出的属性类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = false)]
    public class LogAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置日志级别
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// 获取或设置日志名
        /// </summary>
        public string LogName { get; set; }

        /// <summary>
        /// 获取或设置日志文件后缀 如 .log
        /// </summary>
        public string FileSuffix { get; set; }

        /// <summary>
        /// 获取或设置自动清理日志的天数
        /// </summary>
        public int AutoCleanDays { get; set; }
    }
}
