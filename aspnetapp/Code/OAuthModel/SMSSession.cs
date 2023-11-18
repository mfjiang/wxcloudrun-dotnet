using aspnetapp.Code.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspnetapp.Code.OAuthModel
{
    //Author    江名峰
    //Date      2019.03.29

    /// <summary>
    /// 表示短信会话缓存内容
    /// </summary>
    public class SMSSession : ISMSSession
    {
        /// <summary>
        /// 用于缓存的会话ID
        /// </summary>
        public string Session { get; set; }

        /// <summary>
        /// 四字符验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 新密码（明文）
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// UTC过期时间戳
        /// </summary>
        public long ExpiresIn { get; set; }
    }
}
