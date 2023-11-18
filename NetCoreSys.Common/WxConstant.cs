using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreSys.Common
{
    /// <summary>
    /// 微信相关参数实体
    /// </summary>
    public class WxConstant
    {
        public static string appid { get; set; } = "wx04d5fb69dccb8a33";

        public static string secret { get; set; } = "115965c9149d49c04f9778b0d2d66d61";

        public static string AES { get; set; } = "AES";
        public static string AES_CBC_PADDING { get; set; } = "AES/CBC/PKCS7Padding";


        public static string publicAppid { get; set; } = "wx2d59315149aa15c4";//wxeeb65effff738707

        public static string publicSecret { get; set; } = "30517b50c241718503a1c1bfadefa8fc";//68ee3e78da2f97682b40d67a5e9bf95d

    }
}
