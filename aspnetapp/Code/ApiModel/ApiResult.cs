using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspnetapp.Code.ApiModel
{
    /// <summary>
    /// 用于API返回值封装对象
    /// </summary>
    [Serializable]
    public class ApiResult<T>
    {
        /// <summary>
        /// 获取或设置是否异常
        /// </summary>
        public bool HasError { get; set; }

        public bool success { get { return !HasError; } }
        /// <summary>
        /// 获取或设置返回消息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 获取或设置异常代号
        /// </summary>
        public int code { get; set; } = 200;
        /// <summary>
        /// 获取或设置数据
        /// </summary>
        public T? Data { get; set; }
        /// <summary>
        /// 获取或设置数据
        /// </summary>
        public T? obj { get; set; }
        /// <summary>
        /// 获取或设置数据总行数，单一数据一直是1
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 返回一个异常结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="errorCode">错误码</param>
        /// <returns></returns>
        public void Error(string message, dynamic errorCode)
        {
            code = (int)errorCode;//().ToString();
            HasError = true;
            this.message = message;
        }

        /// <summary>
        /// 返回一个异常结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="errorCode">错误码</param>
        /// <returns></returns>
        public void Error(string message, int errorCode)
        {
            code = errorCode;//.ToString();
            HasError = true;
            this.message = message;
        }

        /// <summary>
        /// 返回一个正常结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">结果数据</param>
        /// <param name="count">结果计数</param>
        /// <returns></returns>
        public void Succeed(string message, T data, int count = 1)
        {
            HasError = false;
            code = 200;
            Data = data;
            TotalCount = count;
            this.message = message;
        }

        public static ApiResult<T> Success()
        {
            return new()
            {
                code = 200,// "200",
                Data = default
            };
        }
    }
}
