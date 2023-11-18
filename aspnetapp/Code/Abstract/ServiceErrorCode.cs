using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspnetapp.Code.Abstract
{
    public enum ServiceErrorCode
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        Success = 200,
        /// <summary>
        /// 重复键
        /// </summary>
        DoubleKey = 201,
        /// <summary>
        /// 不可操作
        /// </summary>
        NoOperation = 202,
        //下单失败
        OrderFail = 300,
        /// <summary>
        /// 禁止访问
        /// </summary>

        Forbidden = 403,
        /// <summary>
        /// 没有找到请求的资源
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// 系统错误，请联系管理员
        /// </summary>
        InternalFault = 500,

        /// <summary>
        /// 调用短信接口失败
        /// </summary>
        InternalFault_CallSMSFailed = 997,

        /// <summary>
        /// 参数错误
        /// </summary>
        ArgumentError = 1000,

        /// <summary>
        /// Http Header 校验失败
        /// </summary>
        HttpHeaderError = 1001,

        /// <summary>
        /// APP验证失败
        /// </summary>
        AppValidateFailed = 1002,

        /// <summary>
        /// 数据签名验证失败
        /// </summary>
        DataSignatureValidateFailed = 1003,

        /// <summary>
        /// 用户令牌验证失败
        /// </summary>
        AccessTokenValidateFailed = 401,

        ///// <summary>
        ///// 用户令牌过期
        ///// </summary>
        //AccessTokenHasExpired = 1011,

        /// <summary>
        /// 手机短信发送次数过多
        /// </summary>
        SMSSendTooMuch = 1012,

        /// <summary>
        /// 短信验证码过期
        /// </summary>
        SMSValidCodeHasEXpired = 1013,

        /// <summary>
        /// 短信验证码验证失败
        /// </summary>
        SMSValidCodeFailed = 1014,

        ///// <summary>
        ///// 昵称不可用
        ///// </summary>
        //NickNameUnavailable = 1020,

        ///// <summary>
        ///// 手机号不可用
        ///// </summary>
        //MobileUnavailabe = 1021,

        /// <summary>
        /// 微信登录ID不可用
        /// </summary>
        WeiXinUnavailable = 1024,

        /// <summary>
        /// 创建用户失败
        /// </summary>
        CreateUserFailed = 1025,

        /// <summary>
        /// 更新用户数据失败
        /// </summary>
        UpdateUserFailed = 1026,
        /// <summary>
        /// 调用微信API失败
        /// </summary>
        WeiXinApiFailed = 1029,

        /// <summary>
        /// 苹果登录ID不可用
        /// </summary>
        AppleIDUnavailable = 1030,

        /// <summary>
        /// 苹果身份令牌不可用
        /// </summary>
        AppleIdentityTokenUnavailable = 1031,

        /// <summary>
        /// 没有找到指定的用户
        /// </summary>
        UserNotFound = 1040,

        /// <summary>
        /// 登录失败（账号或密码错误）
        /// </summary>
        UserLoginFailed = 1041,

        /// <summary>
        /// 账号被封停 or 未登录 or 不可用
        /// </summary>
        UserUnavailable = 1042,

        /// <summary>
        /// IP被封禁
        /// </summary>
        IPUnavailable = 1043,
        /// <summary>
        /// 硬件被封禁
        /// </summary>
        HardwareUnavailable = 1044

    }
}
