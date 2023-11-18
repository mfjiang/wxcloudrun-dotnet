using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspnetapp.Code.Abstract
{
    //Author    江名峰
    //Date      2019.03.21

    /// <summary>
    /// 表示OAuth中的异常信息类
    /// </summary>
    public class OAuthException : Exception
    {
        /// <summary>
        /// 获取或设置错误码
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 返回OAuthException实例
        /// </summary>
        public OAuthException()
        {

        }

        /// <summary>
        ///  返回OAuthException实例
        /// </summary>
        /// <param name="message">异常消息</param>
        public OAuthException(string message) : base(message)
        {

        }

        /// <summary>
        /// 返回OAuthException实例
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public OAuthException(string message, Exception innerException) : base(message, innerException)
        {

        }

        /// <summary>
        /// 返回OAuthException实例
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="errorCode">异常码</param>
        /// <param name="innerException">内部异常</param>
        public OAuthException(string message, ServiceErrorCode errorCode, Exception? innerException) : base(message, innerException)
        {
            ErrorCode = (int)errorCode;
        }
    }
}
