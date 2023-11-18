using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspnetapp.Code.OAuthModel
{
    public class UserSession
    {
        /// <summary>
        /// 返回token值
        /// </summary>
        public string? token { get; set; }
        public string? user_name { get { return userInfo?.name; } }
        public string? user_mobile { get { return userInfo?.phone; } }
        public string? user_id { get { return (userInfo?.id).ToString() == "0" ? "" : (userInfo?.id).ToString(); } }
        public int type { get; set; }
        public string client_id { get; set; } = "0";
        public dynamic? userInfo { get; set; }
        public string? name { get; set; }
    }
}
