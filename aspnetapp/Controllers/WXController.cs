using aspnetapp.Code.ApiModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;

namespace aspnetapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WXController : EnhancedApiController
    {
        [HttpPost]
        public async Task<ApiResult<string>> LoadWXOpenData([FromBody]ReqWXOpenDataModel req)
        {
            Log(nameof(WXController), $"ip:{base.RemoteIP}", JsonConvert.SerializeObject(req));
            string appid = "wx74885639a90f3ac7";
            string appsecret = "?";
            string json = "";
            ApiResult<string> r = new ApiResult<string>();

            try
            {
                json = await GetOpenData(appid, req.openid, req.cloudid);
                r.Succeed("", json);
            }
            catch (Exception ex)
            {
                Log(nameof(WXController), $"ip:{base.RemoteIP}，{ex.Message}\r\n{ex.StackTrace}", JsonConvert.SerializeObject(r));
                r.Error(ex.Message, "301");
            }
            
            if (!r.HasError)
            {
                Log(nameof(WXController), $"ip:{base.RemoteIP}", JsonConvert.SerializeObject(r));
            }

            return r;
        }

        private static async Task<string> GetOpenData(string appid, string openid, string cloudid)
        {
            //using (var httpClient = new HttpClient())
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"http://api.weixin.qq.com/wxa/getopendata?from_appid={appid}&openid={openid}";
                var requestBody = new { cloudid_list = new[] { cloudid } };
                var response = await httpClient.PostAsJsonAsync(url, requestBody);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
