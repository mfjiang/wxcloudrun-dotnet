using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspnetapp.Code.OAuthModel
{
    public class TokenUser
    {
        /// <summary>
        /// id
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string? name { get; set; }

        ///// <summary>
        ///// 是否合伙人
        ///// </summary>
        //public bool isPartner { get; set; }

        ///// <summary>
        ///// 是否代理商
        ///// </summary>
        //public bool isAgent { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string? phone { get; set; }

        /// <summary>
        /// 邀请人id
        /// </summary>
        public long? id_inviter { get; set; }

        /// <summary>
        /// 合伙人id
        /// </summary>
        public long? id_partner { get; set; }

        /// <summary>
        /// 有效状态
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 会员等级
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 微信统一开放平台唯一标识
        /// </summary>
        public string? wx_unionid { get; set; }

        /// <summary>
        /// 微信小程序唯一标识
        /// </summary>
        public string? wx_openid { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool is_activate { get; set; }

        /// <summary>
        /// 是否在微信激活 1 是 0否
        /// </summary>
        public bool is_activate_by_wx { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? last_login_time { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? update_time { get; set; }

        /// <summary>
        /// 更新人id
        /// </summary>
        public long? update_id { get; set; }

        /// <summary>
        /// 升级时间
        /// </summary>
        public DateTime? upgrade_time { get; set; }

        /// <summary>
        /// 性别 SEX_WOMEN SEX_MAN
        /// </summary>
        public string? sex { get; set; }

        /// <summary>
        /// 出生年月
        /// </summary>
        public DateTime? birthday { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string? id_card { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string? head_url { get; set; }
        /// <summary>
        /// 密码错误次数
        /// </summary>
        public int error { get; set; }
    }
}
