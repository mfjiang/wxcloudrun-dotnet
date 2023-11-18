using DotNetCoreConfiguration;
using LogMan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aspnetapp.Entity;
using aspnetapp.Code.Abstract;
using aspnetapp.Code.OAuthModel;

namespace aspnetapp.Controllers
{
    /// <summary>
    /// 带有扩展功能的API控制器类型
    /// </summary>
    [Log(FileSuffix = ".log", LogLevel = LogMan.LogLevel.Info, LogName = "EnhancedApiController", AutoCleanDays = 7)]
    public class EnhancedApiController : ControllerBase
    {
        /// <summary>
        /// 记录API接口日志
        /// </summary>
        /// <param name="logName">日志名称</param>
        /// <param name="msg">消息</param>
        /// <param name="dataJson">JSON数据</param>
        /// <param name="result">结果</param>
        /// <param name="ex">异常对象</param>
        [NonAction]
        public void Log(string logName, string msg, string dataJson = "", Dictionary<string, object>? result = null, Exception? ex = null)
        {
            string ip = RemoteIP;
            string resultJsonStr = "";

            if (result != null)
            {
                resultJsonStr = JsonConvert.SerializeObject(result);
            }

            if (ex == null)//是否异常
            {
                if (result != null)
                {
                    LogMan.Loger.Info(typeof(EnhancedApiController), $"{logName}", $"处理成功 {ip} {msg} {dataJson} 返回 {resultJsonStr}", null);
                }
                else
                {
                    LogMan.Loger.Info(typeof(EnhancedApiController), $"{logName}", $"接收请求 {ip} {msg} {dataJson}", null);
                }
            }
            else
            {
                if (result != null)
                {
                    LogMan.Loger.Error(typeof(EnhancedApiController), $"{logName}", $"处理失败 {ip} {msg} {dataJson} 返回 {resultJsonStr}", ex);
                }
                else
                {
                    LogMan.Loger.Error(typeof(EnhancedApiController), $"{logName}", $"失败 {ip} {msg} {dataJson}", ex);
                }
            }
        }

        /// <summary>
        /// 记录API接口日志
        /// </summary>
        /// <param name="logName">日志名称</param>
        /// <param name="msg">消息</param>
        /// <param name="onAccess">是否请求受理阶段</param>
        /// <param name="result">结果</param>
        /// <param name="ex">异常对象</param>
        [NonAction]
        public void LogProcess(string logName, string msg, string result = "", bool onAccess = true, Exception? ex = null)
        {
            string ip = RemoteIP;
            string resultJsonStr = "";

            if (result != null)
            {
                resultJsonStr = JsonConvert.SerializeObject(result);
            }

            if (ex == null)//是否异常
            {
                if (onAccess)
                {
                    if (result != "")
                    {
                        LogMan.Loger.Info(typeof(EnhancedApiController), $"{logName}", $"接收请求 {ip} {msg} 参数 {result}", null);
                    }
                    else
                    {
                        LogMan.Loger.Info(typeof(EnhancedApiController), $"{logName}", $"接收请求 {ip} {msg}", null);
                    }
                }
                else
                {
                    if (result != "")
                    {
                        LogMan.Loger.Info(typeof(EnhancedApiController), $"{logName}", $"处理成功 {ip} {msg} 返回 {result}", null);
                    }
                    else
                    {
                        LogMan.Loger.Info(typeof(EnhancedApiController), $"{logName}", $"处理成功 {ip} {msg}", null);
                    }
                }
            }
            else
            {
                if (onAccess)
                {
                    if (result != "")
                    {
                        LogMan.Loger.Info(typeof(EnhancedApiController), $"{logName}", $"接收请求异常 {ip} {msg} 参数 {result}", null);
                    }
                    else
                    {
                        LogMan.Loger.Info(typeof(EnhancedApiController), $"{logName}", $"接收请求异常 {ip} {msg}", null);
                    }
                }
                else
                {
                    if (result != "")
                    {
                        LogMan.Loger.Error(typeof(EnhancedApiController), $"{logName}", $"处理失败 {ip} {msg}  返回 {resultJsonStr}", ex);
                    }
                    else
                    {
                        LogMan.Loger.Error(typeof(EnhancedApiController), $"{logName}", $"处理失败 {ip} {msg} ", ex);
                    }
                }
            }
        }

        /// <summary>
        /// 以文本格式读取请求中的Body内容
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> ReadBodyString(HttpRequest req)
        {
            string bodystr = "";
            req.Body.Position = 0;
            var reader = await req.BodyReader.ReadAsync();
            var buffer = reader.Buffer;
            bodystr = Encoding.UTF8.GetString(buffer.FirstSpan);
            return bodystr;
        }

        /// <summary>
        /// 按内部规则取一次请求的HASH值
        /// </summary>
        /// <param name="ip">来源IP</param>
        /// <param name="reqBody">请求body或queue串</param>
        /// <returns></returns>
        [NonAction]
        public string GetRequestHash(string ip, string reqBody)
        {
            string temp = $"{DateTime.Now.Ticks}.{ip}";
            return $"{temp.GetHashCode()}{reqBody.GetHashCode()}".Replace("-", "");
        }

        /// <summary>
        /// 提取请求中的路由部分
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public string ParseRouteString()
        {
            string routes = RouteData?.Values["controller"]?.ToString() + "/";
            if (RouteData != null && RouteData.Values.Keys.Count > 0)
            {
                var l = RouteData.Values.Keys.ToList();
                for (int i = 0; i < l.Count; i++)
                {
                    var c = l[i];
                    if (!c.Equals("controller"))
                    {
                        routes += RouteData.Values[l[i]] + "/";
                    }
                }
            }
            routes = routes.Remove(routes.LastIndexOf("/"));
            return routes;
        }

        /// <summary>
        /// 读取AppSettings配置
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public AppSettings GetAppSettings()
        {
            return DotNetCoreConfiguration.ConfigurationManager.GetAppConfig();
        }

        /// <summary>
        /// 取HTTP HEADER
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [NonAction]
        public StringValues GetHeader(string name)
        {
            this.Request.Headers.TryGetValue(name, out var headerValue);
            return headerValue;
        }

        /// <summary>
        /// 读取MySqlClusterSettings配置
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public MySqlClusterSettings GetMySqlClusterSettings()
        {
            return DotNetCoreConfiguration.ConfigurationManager.GetMySqlClusterSettings();
        }
       
        /// <summary>
        /// 取客户端的IP地址
        /// </summary>
        /// <returns></returns>        
        public string RemoteIP
        {
            get
            {
                if (HttpContext is null || HttpContext.Connection is null || HttpContext.Connection.RemoteIpAddress is null)
                {
                    return string.Empty;
                }
                string client_ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    client_ip = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
                }

                if (client_ip.Equals("::1") || client_ip.Equals("0.0.0.1"))
                {
                    client_ip = "127.0.0.1";
                }
                return client_ip;
            }
        }


        protected AppSettings m_AppSettings = DotNetCoreConfiguration.ConfigurationManager.GetAppConfig();

    }
}
