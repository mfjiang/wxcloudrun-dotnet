using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspnetapp.Entity
{
    public class CombinSessionInfo
    {
        /// <summary>
        /// 返回token值
        /// </summary>
        public string? token { get; set; }
        public string? user_name { get; set; }
        public string? user_mobile { get; set; }
        public string? user_id { get; set; }
        public dynamic? userinfo { get; set; }
        public int type { get; set; }
        public string? name { get; set; }


        public string client_id { get; set; }
    }
}
