using aspnetapp.Code.ApiModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace aspnetapp.Controllers
{
    [Route("api/wx")]
    [ApiController]
    public class WXController : EnhancedApiController
    {
        [HttpPost]
        [Route("GetWXOpenData")]
        public async Task<ApiResult<string>> GetWXOpenData(string cloudid)
        {
            ReqWXOpenDataModel req = new ReqWXOpenDataModel();
             
            string appsecret = "?";
            string json = "";
            ApiResult<string> r = new ApiResult<string>();

            try
            {
                req.openid = Request.Headers["x-wx-from-openid"].ToString();
                req.appid = Request.Headers["x-wx-from-appid"].ToString();
                req.cloudid = cloudid;

                Log(nameof(WXController), $"ip:{base.RemoteIP}", JsonConvert.SerializeObject(req));

                json = await GetOpenData(req.appid, req.openid, req.cloudid);
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

        [HttpGet]
        [Route("Test")]
        public string TestRoute()
        {
            return "hi,here is api";
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
