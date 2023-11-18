using DotNetCoreConfiguration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace LogMan
{
    //Author    江名峰
    //Date      2018.11.27  添加自动清理日志功能
    //Date      2015.09.07

    /// <summary>
    /// 表示简易日志输出器
    /// </summary>
    public static class Loger
    {
        #region 静态字段
        private static string m_LogName;
        private static string m_LogSuffix;
        private static string m_LogPath;
        private static readonly LogLevel m_LogLevel;
        private static readonly int m_AutoCleanDays_Cfg;
        private static int m_AutoCleanDays_Code;
        private static readonly long m_MaxFileSize;
        #endregion

        static Loger()
        {
            m_LogName = "LogMan";
            m_LogSuffix = ".log";
            m_LogPath = @"C:\LogMan\";
            m_LogLevel = LogLevel.Unknown;
            //默认清理周期
            m_AutoCleanDays_Code = 7;
            //此值为-1说明没有配置，按m_AutoCleanDays_Code的值执行
            m_AutoCleanDays_Cfg = -1;
            //不大于50MB (5242880*10 byte)   
            m_MaxFileSize = 5242880 * 10;

            try
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.GetAppConfig("LogManPath")))
                {
                    m_LogPath = ConfigurationManager.GetAppConfig("LogManPath");
                }

                if (!Directory.Exists(m_LogPath))
                {
                    Directory.CreateDirectory(m_LogPath);
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.GetAppConfig("LogManLevel")))
                {
                    m_LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), ConfigurationManager.GetAppConfig("LogManLevel"));
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.GetAppConfig("AutoCleanDays")))
                {
                    int.TryParse(ConfigurationManager.GetAppConfig("AutoCleanDays"), out m_AutoCleanDays_Cfg);
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.GetAppConfig("LogManMaxFileSize")))
                {
                    long.TryParse(ConfigurationManager.GetAppConfig("LogManMaxFileSize"), out m_MaxFileSize);
                }
            }
            catch (Exception)
            {
                ;
            }
        }

        /// <summary>
        /// 初始化日志配置
        /// </summary>
        /// <param name="logName">日志名</param>
        /// <param name="logSuffix">日志后缀</param>
        /// <param name="logPath">日志路径</param>
        /// <param name="autoCleanDays">自动清理天数</param>
        public static void InitLog(string logName = null, string logSuffix = null, string logPath = null, int autoCleanDays = 7)
        {
            if (!string.IsNullOrEmpty(logName))
            {
                m_LogName = logName;
            }

            if (!string.IsNullOrEmpty(logSuffix))
            {
                m_LogSuffix = logSuffix;
            }

            if (!string.IsNullOrEmpty(logPath))
            {
                m_LogPath = logPath;
            }

            m_AutoCleanDays_Code = autoCleanDays;
        }

        /// <summary>
        /// 输出消息日志 LogLevel = Info
        /// </summary>
        /// <param name="ot">要记录的类型</param>
        /// <param name="message">消息</param>
        /// <param name="ex">异常</param>
        public static void Info(Type ot, string message, Exception ex = null)
        {
            //从类型获取日志配置
            IEnumerable<object> ie = ot.GetCustomAttributes(typeof(LogAttribute), true);
            if (ie != null && ie.Count() > 0)
            {
                foreach (Attribute ca in ie)
                {
                    LogAttribute lma = (LogAttribute)ca;
                    //配置文件没有明确配置日志级别，使用代码中定义的默认级别
                    if (m_LogLevel == LogLevel.Unknown)
                    {
                        if (lma.LogLevel == LogLevel.Info)
                        {
                            InitLog(lma.LogName, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Info), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb);
                            break;
                        }
                    }
                    else
                    {
                        //使用配置中的日志级别
                        if (m_LogLevel == LogLevel.Info)
                        {
                            InitLog(lma.LogName, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Info), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 输出消息日志 LogLevel = Info
        /// </summary>
        /// <param name="ot">要记录的类型</param>
        /// <param name="logName">自定义日志名称</param>
        /// <param name="message">消息</param>
        /// <param name="ex">异常</param>
        public static void Info(Type ot, string logName, string message, Exception ex = null)
        {
            //从类型获取日志配置
            IEnumerable<object> ie = ot.GetCustomAttributes(typeof(LogAttribute), true);
            if (ie != null && ie.Count() > 0)
            {
                foreach (Attribute ca in ie)
                {
                    LogAttribute lma = (LogAttribute)ca;
                    //配置文件没有明确配置日志级别，使用代码中定义的默认级别
                    if (m_LogLevel == LogLevel.Unknown)
                    {
                        if (lma.LogLevel == LogLevel.Info)
                        {
                            InitLog(null, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Info), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb, logName);
                            break;
                        }
                    }
                    else
                    {
                        //使用配置中的日志级别
                        if (m_LogLevel == LogLevel.Info)
                        {
                            InitLog(null, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Info), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb, logName);
                            break;
                        }
                    }
                }
            }
            else//处理不存在LogAttribute的情况
            {
                //使用配置中的日志级别
                if (m_LogLevel == LogLevel.Info)
                {
                    InitLog(null, m_LogSuffix, null, m_AutoCleanDays_Cfg);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Info), DateTime.Now));
                    if (!string.IsNullOrEmpty(message))
                    {
                        sb.AppendLine(message);
                    }

                    if (ex != null)
                    {
                        sb.AppendLine(ex.ToString());
                    }

                    WriteLogFile(sb, logName);
                }
            }
        }

        /// <summary>
        /// 输出警告日志 LogLevel = Info,Warn
        /// </summary>
        /// <param name="obj">要记录的类型</param>
        /// <param name="message">消息</param>
        /// <param name="ex">异常</param>
        public static void Warn(Type ot, string message, Exception ex = null)
        {
            //从类型获取日志配置
            IEnumerable<object> ie = ot.GetCustomAttributes(typeof(LogAttribute), true);
            if (ie != null && ie.Count() > 0)
            {
                foreach (object ca in ie)
                {
                    LogAttribute lma = (LogAttribute)ca;
                    //配置文件没有明确配置日志级别，使用代码中定义的默认级别
                    if (m_LogLevel == LogLevel.Unknown)
                    {
                        if (lma.LogLevel <= LogLevel.Warn && lma.LogLevel > LogLevel.None)
                        {
                            InitLog(lma.LogName, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Warn), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb);
                            break;
                        }
                    }
                    else
                    {
                        //使用配置中的日志级别
                        if (m_LogLevel <= LogLevel.Warn && m_LogLevel > LogLevel.None)
                        {
                            InitLog(lma.LogName, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Warn), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 输出警告日志 LogLevel = Info,Warn
        /// </summary>
        /// <param name="ot">要记录的类型</param>
        /// <param name="logName">自定义日志名称</param>
        /// <param name="message">消息</param>
        /// <param name="ex">异常</param>
        public static void Warn(Type ot, string logName, string message, Exception ex = null)
        {
            //从类型获取日志配置
            IEnumerable<object> ie = ot.GetCustomAttributes(typeof(LogAttribute), true);
            if (ie != null && ie.Count() > 0)
            {
                foreach (object ca in ie)
                {
                    LogAttribute lma = (LogAttribute)ca;
                    //配置文件没有明确配置日志级别，使用代码中定义的默认级别
                    if (m_LogLevel == LogLevel.Unknown)
                    {
                        if (lma.LogLevel <= LogLevel.Warn && lma.LogLevel > LogLevel.None)
                        {
                            InitLog(null, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Warn), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb, logName);
                            break;
                        }
                    }
                    else
                    {
                        //使用配置中的日志级别
                        if (m_LogLevel <= LogLevel.Warn && m_LogLevel > LogLevel.None)
                        {
                            InitLog(logName, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Warn), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb, logName);
                            break;
                        }
                    }
                }
            }
            else//处理不存在LogAttribute的情况
            {
                //使用配置中的日志级别
                if (m_LogLevel <= LogLevel.Warn && m_LogLevel > LogLevel.None)
                {
                    InitLog(null, m_LogSuffix, null, m_AutoCleanDays_Cfg);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Info), DateTime.Now));
                    if (!string.IsNullOrEmpty(message))
                    {
                        sb.AppendLine(message);
                    }

                    if (ex != null)
                    {
                        sb.AppendLine(ex.ToString());
                    }

                    WriteLogFile(sb, logName);
                }
            }
        }

        /// <summary>
        /// 输出一般异常日志 LogLevel = Info,Warn,Error
        /// </summary>
        /// <param name="obj">要记录的类型</param>
        /// <param name="message">消息</param>
        /// <param name="ex">异常</param>
        public static void Error(Type ot, string message, Exception ex = null)
        {
            //从类型获取日志配置
            IEnumerable<object> ie = ot.GetCustomAttributes(typeof(LogAttribute), true);
            if (ie != null && ie.Count() > 0)
            {
                foreach (object ca in ie)
                {
                    LogAttribute lma = (LogAttribute)ca;
                    //配置文件没有明确配置日志级别，使用代码中定义的默认级别
                    if (m_LogLevel == LogLevel.Unknown)
                    {
                        if (lma.LogLevel <= LogLevel.Error && lma.LogLevel > LogLevel.None)
                        {
                            InitLog(lma.LogName, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Error), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb);
                            break;
                        }
                    }
                    else
                    {
                        //使用配置中的日志级别
                        if (m_LogLevel <= LogLevel.Error && m_LogLevel > LogLevel.None)
                        {
                            InitLog(lma.LogName, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Error), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb);
                            break;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 输出一般异常日志 LogLevel = Info,Warn,Error
        /// </summary>
        /// <param name="ot">要记录的类型</param>
        /// <param name="logName">自定义日志名称</param>
        /// <param name="message">消息</param>
        /// <param name="ex">异常</param>
        public static void Error(Type ot, string logName, string message, Exception ex = null)
        {
            //从类型获取日志配置
            IEnumerable<object> ie = ot.GetCustomAttributes(typeof(LogAttribute), true);
            if (ie != null && ie.Count() > 0)
            {
                foreach (object ca in ie)
                {
                    LogAttribute lma = (LogAttribute)ca;
                    //配置文件没有明确配置日志级别，使用代码中定义的默认级别
                    if (m_LogLevel == LogLevel.Unknown)
                    {
                        if (lma.LogLevel <= LogLevel.Error && lma.LogLevel > LogLevel.None)
                        {
                            InitLog(null, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Error), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb, logName);
                            break;
                        }
                    }
                    else
                    {
                        //使用配置中的日志级别
                        if (m_LogLevel <= LogLevel.Error && m_LogLevel > LogLevel.None)
                        {
                            InitLog(null, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Error), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.ToString());
                            }

                            WriteLogFile(sb, logName);
                            break;
                        }
                    }

                }
            }
            else//处理不存在LogAttribute的情况
            {
                //使用配置中的日志级别
                if (m_LogLevel <= LogLevel.Error && m_LogLevel > LogLevel.None)
                {
                    InitLog(null, m_LogSuffix, null, m_AutoCleanDays_Cfg);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Info), DateTime.Now));
                    if (!string.IsNullOrEmpty(message))
                    {
                        sb.AppendLine(message);
                    }

                    if (ex != null)
                    {
                        sb.AppendLine(ex.ToString());
                    }

                    WriteLogFile(sb, logName);
                }
            }
        }

        /// <summary>
        /// 输出致命异常日志 LogLevel = Info,Warn,Error,Fatal
        /// </summary>
        /// <param name="ot">要记录的类型</param>
        /// <param name="message">消息</param>
        /// <param name="ex">异常</param>
        public static void Fatal(Type ot, string message, Exception ex = null)
        {
            _ = new List<LogLevel>() { LogLevel.Info, LogLevel.Warn, LogLevel.Error, LogLevel.Fatal };
            //从类型获取日志配置
            IEnumerable<object> ie = ot.GetCustomAttributes(typeof(LogAttribute), true);
            if (ie != null && ie.Count() > 0)
            {
                foreach (object ca in ie)
                {
                    LogAttribute lma = (LogAttribute)ca;
                    //配置文件没有明确配置日志级别，使用代码中定义的默认级别
                    if (m_LogLevel == LogLevel.Unknown)
                    {
                        if (lma.LogLevel <= LogLevel.Fatal && lma.LogLevel > LogLevel.None)
                        {
                            InitLog(lma.LogName, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("======= {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Fatal), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.Source);
                                sb.AppendLine(ex.ToString());
                                sb.AppendLine(ex.StackTrace);
                                if (ex.TargetSite != null)
                                {
                                    sb.AppendLine(ex.TargetSite.ToString());
                                }
                            }

                            WriteLogFile(sb);
                            break;
                        }
                    }
                    else
                    {
                        //使用配置中的日志级别
                        if (m_LogLevel <= LogLevel.Fatal && m_LogLevel > LogLevel.None)
                        {
                            InitLog(lma.LogName, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("======= {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Fatal), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.Source);
                                sb.AppendLine(ex.ToString());
                                sb.AppendLine(ex.StackTrace);
                                if (ex.TargetSite != null)
                                {
                                    sb.AppendLine(ex.TargetSite.ToString());
                                }
                            }

                            WriteLogFile(sb);
                            break;
                        }
                    }
                }
            }
        }

        // <summary>
        /// 输出致命异常日志 LogLevel = Info,Warn,Error,Fatal
        /// </summary>
        /// <param name="ot">要记录的类型</param>
        /// <param name="message">消息</param>
        /// <param name="logName">自定义日志名称</param>
        /// <param name="ex">异常</param>
        public static void Fatal(Type ot, string logName, string message, Exception ex = null)
        {
            _ = new List<LogLevel>() { LogLevel.Info, LogLevel.Warn, LogLevel.Error, LogLevel.Fatal };
            //从类型获取日志配置
            IEnumerable<object> ie = ot.GetCustomAttributes(typeof(LogAttribute), true);
            if (ie != null && ie.Count() > 0)
            {
                foreach (object ca in ie)
                {
                    LogAttribute lma = (LogAttribute)ca;
                    //配置文件没有明确配置日志级别，使用代码中定义的默认级别
                    if (m_LogLevel == LogLevel.Unknown)
                    {
                        if (lma.LogLevel <= LogLevel.Fatal && lma.LogLevel > LogLevel.None)
                        {
                            InitLog(null, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("======= {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Fatal), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.Source);
                                sb.AppendLine(ex.ToString());
                                sb.AppendLine(ex.StackTrace);
                                if (ex.TargetSite != null)
                                {
                                    sb.AppendLine(ex.TargetSite.ToString());
                                }
                            }

                            WriteLogFile(sb, logName);
                            break;
                        }
                    }
                    else
                    {
                        //使用配置中的日志级别
                        if (m_LogLevel <= LogLevel.Fatal && m_LogLevel > LogLevel.None)
                        {
                            InitLog(null, lma.FileSuffix, null, lma.AutoCleanDays);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("======= {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Fatal), DateTime.Now));
                            if (!string.IsNullOrEmpty(message))
                            {
                                sb.AppendLine(message);
                            }

                            if (ex != null)
                            {
                                sb.AppendLine(ex.Source);
                                sb.AppendLine(ex.ToString());
                                sb.AppendLine(ex.StackTrace);
                                if (ex.TargetSite != null)
                                {
                                    sb.AppendLine(ex.TargetSite.ToString());
                                }
                            }

                            WriteLogFile(sb, logName);
                            break;
                        }
                    }
                }
            }
            else//处理不存在LogAttribute的情况
            {
                //使用配置中的日志级别
                if (m_LogLevel <= LogLevel.Fatal && m_LogLevel > LogLevel.None)
                {
                    InitLog(null, m_LogSuffix, null, m_AutoCleanDays_Cfg);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Format("====== {0} {1} {2} ======", ot.FullName, Enum.GetName(typeof(LogLevel), LogLevel.Info), DateTime.Now));
                    if (!string.IsNullOrEmpty(message))
                    {
                        sb.AppendLine(message);
                    }

                    if (ex != null)
                    {
                        sb.AppendLine(ex.ToString());
                    }

                    WriteLogFile(sb, logName);
                }
            }
        }

        /// <summary>
        /// 写日志到文件
        /// </summary>
        /// <param name="log"></param>
        private static void WriteLogFile(StringBuilder log)
        {
            string dateField = DateTime.Now.ToString("yyyy-MM-dd");
            int pid = Process.GetCurrentProcess().Id;
            string logFileName = string.Format("{0}-{1}-pid{2}{3}", m_LogName, dateField, pid, m_LogSuffix);

            try
            {
                object locker = new object();

                lock (locker)
                {
                    //FileStream logFile = File.Open(m_LogPath + "\\" + logFileName, FileMode.Append);这个写法导致linux上多了\
                    DirectoryInfo logdir = new DirectoryInfo(m_LogPath);
                    //FileStream logFile = File.Open(logdir.FullName + @"\" + logFileName, FileMode.Append);
                    FileStream logFile = File.Open(logdir.FullName + logFileName, FileMode.Append);
                    //检查文件大小，每个日志文件不大于5MB (5242880 byte)   
                    if (logFile.Length < m_MaxFileSize)
                    {
                        if (log != null)
                        {
                            byte[] logData = Encoding.UTF8.GetBytes(log.ToString());
                            MemoryStream logStream = new MemoryStream(logData);
                            for (int i = 0; i < logData.Length; i++)
                            {
                                logFile.Write(logData, i, 1);
                            }
                        }
                    }
                    else
                    {
                        logFile.Flush();
                        logFile.Close();

                        int partitionedLogFileNumber = 1;
                        //把当天日志拆成多个，每个不大于5MB
                        WriteLogFile(log, partitionedLogFileNumber);
                    }

                    logFile.Flush();
                    logFile.Close();

                }

                //每周一、三、六的1、9、12、15、18、23点的1、5、10、15、30、50分清理文件
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Monday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) &&
                    (DateTime.Now.Hour == 1 || DateTime.Now.Hour == 9 || DateTime.Now.Hour == 12 || DateTime.Now.Hour == 15 || DateTime.Now.Hour == 18 || DateTime.Now.Hour == 23) &&
                    (DateTime.Now.Minute == 1 || DateTime.Now.Minute == 5 || DateTime.Now.Minute == 10 || DateTime.Now.Minute == 15 || DateTime.Now.Minute == 30 || DateTime.Now.Minute == 50))
                {
                    CleanFiles();
                }

            }
            catch (Exception)
            {
                ;
            }
        }

        /// <summary>
        /// 写日志到文件
        /// </summary>
        /// <param name="log"></param>
        /// <param name="logName">指定日志名</param>
        private static void WriteLogFile(StringBuilder log, string logName)
        {
            string dateField = DateTime.Now.ToString("yyyy-MM-dd");
            int pid = Process.GetCurrentProcess().Id;
            string logFileName = string.Format("{0}-{1}-pid{2}{3}", logName, dateField, pid, m_LogSuffix);

            try
            {
                object locker = new object();

                lock (locker)
                {
                    //FileStream logFile = File.Open(m_LogPath + "\\" + logFileName, FileMode.Append);这个写法导致linux上多了\
                    DirectoryInfo logdir = new DirectoryInfo(m_LogPath);
                    //FileStream logFile = File.Open(logdir.FullName + @"\" + logFileName, FileMode.Append);
                    FileStream logFile = File.Open(logdir.FullName + logFileName, FileMode.Append);
                    //检查文件大小，每个日志文件不大于5MB (5242880 byte)   
                    if (logFile.Length < m_MaxFileSize)
                    {
                        if (log != null)
                        {
                            byte[] logData = Encoding.UTF8.GetBytes(log.ToString());
                            MemoryStream logStream = new MemoryStream(logData);
                            for (int i = 0; i < logData.Length; i++)
                            {
                                logFile.Write(logData, i, 1);
                            }
                        }
                    }
                    else
                    {
                        logFile.Flush();
                        logFile.Close();

                        int partitionedLogFileNumber = 1;
                        //把当天日志拆成多个，每个不大于5MB
                        WriteLogFile(log, logName, partitionedLogFileNumber);
                    }

                    logFile.Flush();
                    logFile.Close();

                }

                //每周一、三、六的1、9、12、15、18、23点的1、5、10、15、30、50分清理文件
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Monday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) &&
                    (DateTime.Now.Hour == 1 || DateTime.Now.Hour == 9 || DateTime.Now.Hour == 12 || DateTime.Now.Hour == 15 || DateTime.Now.Hour == 18 || DateTime.Now.Hour == 23) &&
                    (DateTime.Now.Minute == 1 || DateTime.Now.Minute == 5 || DateTime.Now.Minute == 10 || DateTime.Now.Minute == 15 || DateTime.Now.Minute == 30 || DateTime.Now.Minute == 50))
                {
                    CleanFiles();
                }

            }
            catch (Exception ex)
            {
                ;
            }
        }

        /// <summary>
        /// 把当天日志拆成多个，每个不大于50MB
        /// </summary>
        /// <param name="log"></param>
        /// <param name="partitionedLogFileNumber">每次递归加1</param>
        private static void WriteLogFile(StringBuilder log, int partitionedLogFileNumber)
        {
            string dateField = DateTime.Now.ToString("yyyy-MM-dd");
            int pid = Process.GetCurrentProcess().Id;
            string logFileName = string.Format("{0}-{1}-pid{2}-part{3}{4}", m_LogName, dateField, pid, partitionedLogFileNumber, m_LogSuffix);

            try
            {
                object locker = new object();

                lock (locker)
                {

                    //FileStream logFile = File.Open(m_LogPath + "\\" + logFileName, FileMode.Append);这个写法导致linux上多了\
                    DirectoryInfo logdir = new DirectoryInfo(m_LogPath);
                    //FileStream logFile = File.Open(logdir.FullName + @"\" + logFileName, FileMode.Append);
                    FileStream logFile = File.Open(logdir.FullName + logFileName, FileMode.Append);
                    //检查文件大小，每个日志文件不大于MaxFileSize  
                    if (logFile.Length < m_MaxFileSize)
                    {
                        if (log != null)
                        {
                            byte[] logData = Encoding.UTF8.GetBytes(log.ToString());
                            MemoryStream logStream = new MemoryStream(logData);
                            for (int i = 0; i < logData.Length; i++)
                            {
                                logFile.Write(logData, i, 1);
                            }
                        }
                    }
                    else
                    {
                        logFile.Flush();
                        logFile.Close();
                        //递归处理，每次日志编号加1
                        WriteLogFile(log, partitionedLogFileNumber + 1);
                    }

                    logFile.Flush();
                    logFile.Close();

                }

                //每周一、三、六的1、9、12、15、18、23点的1、5、10、15、30、50分清理文件
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Monday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) &&
                    (DateTime.Now.Hour == 1 || DateTime.Now.Hour == 9 || DateTime.Now.Hour == 12 || DateTime.Now.Hour == 15 || DateTime.Now.Hour == 18 || DateTime.Now.Hour == 23) &&
                    (DateTime.Now.Minute == 1 || DateTime.Now.Minute == 5 || DateTime.Now.Minute == 10 || DateTime.Now.Minute == 15 || DateTime.Now.Minute == 30 || DateTime.Now.Minute == 50))
                {
                    CleanFiles();
                }
            }
            catch (Exception)
            {
                ;
            }
        }

        /// <summary>
        /// 把当天日志拆成多个，每个不大于5MB
        /// </summary>
        /// <param name="log"></param>
        /// <param name="logName">指定日志名</param>
        /// <param name="partitionedLogFileNumber">每次递归加1</param>
        private static void WriteLogFile(StringBuilder log, string logName, int partitionedLogFileNumber)
        {
            string dateField = DateTime.Now.ToString("yyyy-MM-dd");
            int pid = Process.GetCurrentProcess().Id;
            string logFileName = string.Format("{0}-{1}-pid{2}-part{3}{4}", logName, dateField, pid, partitionedLogFileNumber, m_LogSuffix);

            try
            {
                object locker = new object();

                lock (locker)
                {
                    //FileStream logFile = File.Open(m_LogPath + "\\" + logFileName, FileMode.Append);这个写法导致linux上多了\
                    DirectoryInfo logdir = new DirectoryInfo(m_LogPath);
                    //FileStream logFile = File.Open(logdir.FullName + @"\" + logFileName, FileMode.Append);
                    FileStream logFile = File.Open(logdir.FullName + logFileName, FileMode.Append);
                    //检查文件大小，每个日志文件不大于50MB (5242880*10 byte)   
                    if (logFile.Length < m_MaxFileSize)
                    {
                        if (log != null)
                        {
                            byte[] logData = Encoding.UTF8.GetBytes(log.ToString());
                            MemoryStream logStream = new MemoryStream(logData);
                            for (int i = 0; i < logData.Length; i++)
                            {
                                logFile.Write(logData, i, 1);
                            }
                        }
                    }
                    else
                    {
                        logFile.Flush();
                        logFile.Close();
                        //递归处理，每次日志编号加1
                        WriteLogFile(log, logName, partitionedLogFileNumber + 1);
                    }

                    logFile.Flush();
                    logFile.Close();

                }

                //每周一、三、六的1、9、12、15、18、23点的1、5、10、15、30、50分清理文件
                if ((DateTime.Now.DayOfWeek == DayOfWeek.Monday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) &&
                    (DateTime.Now.Hour == 1 || DateTime.Now.Hour == 9 || DateTime.Now.Hour == 12 || DateTime.Now.Hour == 15 || DateTime.Now.Hour == 18 || DateTime.Now.Hour == 23) &&
                    (DateTime.Now.Minute == 1 || DateTime.Now.Minute == 5 || DateTime.Now.Minute == 10 || DateTime.Now.Minute == 15 || DateTime.Now.Minute == 30 || DateTime.Now.Minute == 50))
                {
                    CleanFiles();
                }
            }
            catch (Exception)
            {
                ;
            }
        }

        /// <summary>
        /// 清理过时日志文件
        /// </summary>
        private static void CleanFiles()
        {
            int days = m_AutoCleanDays_Code;
            if (m_AutoCleanDays_Cfg > -1)
            {
                days = m_AutoCleanDays_Cfg;
            }

            DirectoryInfo logdir = new DirectoryInfo(m_LogPath);
            //FileStream logFile = File.Open(logdir.FullName + @"\" + logFileName, FileMode.Append);
            FileStream logFile = File.Open(logdir.FullName + "AutoCleanLog.log", FileMode.Append);
            StringBuilder log = new StringBuilder();
            log.AppendLine(string.Format("{0},auto clean days:{1},path:{2}", DateTime.Now.ToLongDateString(), days, m_LogPath));

            //0值表示不清理
            if (days > 0)
            {
                object locker = new object();
                lock (locker)
                {
                    try
                    {
                        string[] files = Directory.GetFiles(m_LogPath, "*" + m_LogSuffix);
                        log.AppendLine(string.Format("{0},{1} log files found,path:{2}", DateTime.Now.ToLongDateString(), files.Length, m_LogPath));
                        //仅余1个日志时不清理
                        if (files.Length > 1)
                        {
                            for (int i = 1; i < files.Length; i++)
                            {
                                FileInfo f = new FileInfo(files[i]);
                                if (!f.Name.Equals("AutoCleanLog.log"))
                                {
                                    if ((DateTime.Now - f.CreationTime) >= new TimeSpan(days, 0, 0, 0))
                                    {
                                        log.AppendLine(f.CreationTime.ToLongDateString() + " deleted " + files[i]);
                                        f.Delete();
                                    }
                                }
                            }
                        }

                        byte[] logData = Encoding.UTF8.GetBytes(log.ToString());
                        MemoryStream logStream = new MemoryStream(logData);
                        for (int i = 0; i < logData.Length; i++)
                        {
                            logFile.Write(logData, i, 1);
                        }
                        logFile.Flush();
                        logFile.Close();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }
}
