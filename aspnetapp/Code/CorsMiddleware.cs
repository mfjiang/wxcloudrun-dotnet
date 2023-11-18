using LogMan;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspnetapp.Code
{
    /// <summary>
    /// 跨域请求处理中间件
    /// </summary>
    [LogAttribute(FileSuffix = ".log", LogLevel = LogMan.LogLevel.Info, LogName = "CorsMiddleware", AutoCleanDays = 7)]
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 穿域
        /// </summary>
        /// <param name="next"></param>
        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext)
        {
            //AppSettings appSettings = ConfigurationManager.GetAppConfig();
            string ip = GetRemoteIPV4(httpContext);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"IP: {ip};");
            sb.AppendLine($"Method: {httpContext.Request.Method};");
            sb.AppendLine($"Path: {httpContext.Request.Path};");
            string querystr = httpContext.Request.QueryString.ToUriComponent();            
            sb.AppendLine($"Query: {querystr};");           

            if (httpContext.Request.Method == "OPTIONS")
            {
                if (httpContext.Request.Headers["Origin"] != "")
                {
                    //修改源标记
                    httpContext.Response.Headers.Add("Access-Control-Allow-Origin", httpContext.Request.Headers["Origin"]);
                    //Loger.Info(typeof(CorsMiddleware), $"IP: {ip};rewrite Access-Control-Allow-Origin:{httpContext.Request.Headers["Origin"]}");                
                    sb.AppendLine($"IP: {ip};OPTION header rewrite Access-Control-Allow-Origin:{httpContext.Request.Headers["Origin"]}");
                }
                else
                {
                    httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                }

                //httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                httpContext.Response.Headers.Add("Access-Control-Allow-Headers", httpContext.Request.Headers["Access-Control-Request-Headers"]);
                httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "*");
                httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                httpContext.Response.Headers.Add("Access-Control-Max-Age", "86400");//缓存一天
                httpContext.Response.StatusCode = 204;
                return httpContext.Response.WriteAsync("OK");
            }
            
            if (httpContext.Request.Headers["Origin"] != "")
            {                
                //修改源标记
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", httpContext.Request.Headers["Origin"]);
                //Loger.Info(typeof(CorsMiddleware), $"IP: {ip};rewrite Access-Control-Allow-Origin:{httpContext.Request.Headers["Origin"]}");                
                sb.AppendLine($"IP: {ip};rewrite Access-Control-Allow-Origin:{httpContext.Request.Headers["Origin"]}");
            }

            //Loger.Info(typeof(CorsMiddleware), sb.ToString());

            httpContext.Response.Headers.Add("Access-Control-Allow-Headers", httpContext.Request.Headers["Access-Control-Request-Headers"]);
            httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "*");
            //httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            if (!String.IsNullOrEmpty(httpContext.Request.Headers["Access-Control-Allow-Credentials"]))
            {
                httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", httpContext.Request.Headers["Access-Control-Allow-Credentials"]);
                sb.AppendLine($"IP: {ip};rewrite Access-Control-Allow-Credentials:{httpContext.Request.Headers["Access-Control-Allow-Credentials"]}");
            }
            else
            {
                httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            }
            httpContext.Response.Headers.Add("Access-Control-Max-Age", "86400");//缓存一天
            return _next.Invoke(httpContext);
        }

        /// <summary>
        /// 取真实请求IP
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private string GetRemoteIPV4(HttpContext httpContext)
        {
            string client_ip = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                client_ip = httpContext.Request.Headers["X-Forwarded-For"].ToString();
            }

            if (client_ip.Equals("::1") || client_ip.Equals("0.0.0.1"))
            {
                client_ip = "127.0.0.1";
            }
            return client_ip;
        }
    }
    
    /// <summary>
    ///Extension method used to add the middleware to the HTTP request pipeline. 
    /// </summary>
    public static class CorsMiddlewareExtensions
    {
        /// <summary>
        /// 使用自定义跨域
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsMiddleware>();
        }
    }
}
