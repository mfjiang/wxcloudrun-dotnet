using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspnetapp.Code.Abstract
{
    //Author    江名峰
    //Date      2019.03.29

    /// <summary>
    /// 表示短信验证码会话
    /// </summary>
    public interface ISMSSession
    {
        /// <summary>
        /// 用于缓存的会话ID
        /// </summary>
        string Session { get; set; }

        /// <summary>
        /// 四字符验证码
        /// </summary>
        string Code { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        string Mobile { get; set; }

        /// <summary>
        /// UTC过期时间戳
        /// </summary>
        long ExpiresIn { get; set; }
    }
}
