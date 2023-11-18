using System.Text.Json;
using System.Text.Unicode;

namespace NetCoreSys.Common
{
    public static class JsonUtil
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="aims">要序列化的对象</param>
        /// <returns>json文本</returns>
        public static string Serialize(object aims)
        {
            var option = new System.Text.Json.JsonSerializerOptions();
            option.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All);
            return JsonSerializer.Serialize(aims, option);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="json">json文本</param>
        /// <typeparam name="T">反序列化的类型</typeparam>
        /// <returns>反序列化后的T类型对象</returns>
        public static T Deserialize<T>(string json)
        {
           
            //option.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}