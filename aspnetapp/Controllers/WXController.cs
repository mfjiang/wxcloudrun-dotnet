﻿using aspnetapp.Code.ApiModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

namespace aspnetapp.Controllers
{
    [Route("api/wx")]
    [ApiController]
    public class WXController : EnhancedApiController
    {
        [HttpGet]
        [Route("GetWXOpenData")]
        public async Task<ApiResult<string>> GetWXOpenData(string cloudid)
        {
            ReqWXOpenDataModel req = new ReqWXOpenDataModel();
             
            string appsecret = "?";
            string json = "";
            ApiResult<string> r = new ApiResult<string>();

            //StringBuilder headerdic = new StringBuilder();
            //foreach (var item in Request.Headers)
            //{
            //    headerdic.AppendLine($"{item.Key}:{item.Value}");
            //}

            try
            {
                req.openid = Request.Headers["X-WX-OPENID"].ToString();
                req.appid = Request.Headers["X-WX-APPID"].ToString();
                req.cloudid = cloudid;

                Log(nameof(WXController), $"ip:{base.RemoteIP}", JsonConvert.SerializeObject(req));
                //Log(nameof(WXController), $"ip:{base.RemoteIP}", headerdic.ToString());

                json = await GetOpenData(req.appid, req.openid, req.cloudid);
                r.Succeed("",json);
            }
            catch (Exception ex)
            {
                Log(nameof(WXController), $"ip:{base.RemoteIP}，{ex.Message}\r\n{ex.StackTrace}", JsonConvert.SerializeObject(r));
                r.Error(ex.Message, "301");
            }
            
            if (!r.HasError)
            {
                Log(nameof(WXController), $"ip:{base.RemoteIP} ok", JsonConvert.SerializeObject(r));
            }

            return r;
        }

        [HttpGet]
        [Route("GetWXOpenDataObj")]
        public async Task<ApiResult<WXApiRspModel>> GetWXOpenDataObj(string cloudid)
        {
            ReqWXOpenDataModel req = new ReqWXOpenDataModel();

            string appsecret = "?";
            string json = "";
            ApiResult<WXApiRspModel> r = new ApiResult<WXApiRspModel>();

            //StringBuilder headerdic = new StringBuilder();
            //foreach (var item in Request.Headers)
            //{
            //    headerdic.AppendLine($"{item.Key}:{item.Value}");
            //}

            try
            {
                req.openid = Request.Headers["X-WX-OPENID"].ToString();
                req.appid = Request.Headers["X-WX-APPID"].ToString();
                req.cloudid = cloudid;

                Log(nameof(WXController), $"ip:{base.RemoteIP}", JsonConvert.SerializeObject(req));
                //Log(nameof(WXController), $"ip:{base.RemoteIP}", headerdic.ToString());

                json = await GetOpenData(req.appid, req.openid, req.cloudid);
                WXApiRspModel jobj = JsonConvert.DeserializeObject<WXApiRspModel>(json);
                if (jobj != null)
                {
                    jobj.data_list[0].JsonObj = JsonConvert.DeserializeObject<WXRspInnerJsonObj>(jobj.data_list[0].json);
                }
                r.Succeed("", jobj);
            }
            catch (Exception ex)
            {
                Log(nameof(WXController), $"ip:{base.RemoteIP}，{ex.Message}\r\n{ex.StackTrace}", JsonConvert.SerializeObject(r));
                r.Error(ex.Message, "301");
            }

            if (!r.HasError)
            {
                Log(nameof(WXController), $"ip:{base.RemoteIP} ok", JsonConvert.SerializeObject(r));
            }

            return r;
        }

        [HttpGet]
        [Route("Test")]
        public string TestRoute()
        {
            WXApiRspModel jobj;
            try
            {
                string json = "{\"errcode\":0,\"errmsg\":\"ok\",\"data_list\":[{\"cloud_id\":\"74_BSsDgjPCNGZVIWceIJK3mk4zeXAZhFoOGpEclmjZC9le5Rv64tAHk_fVboA\",\"json\":\"{ \\\"cloudID\\\":\\\"74_BSsDgjPCNGZVIWceIJK3mk4zeXAZhFoOGpEclmjZC9le5Rv64tAHk_fVboA\\\", \\\"data\\\":{\\\"opengid\\\":\\\"tGopay62TqCwp7mXeJcZcH2QsUe2A\\\"} }\"}]}";
                jobj = JsonConvert.DeserializeObject<WXApiRspModel>(json);
                if (jobj != null)
                {
                    jobj.data_list[0].JsonObj = JsonConvert.DeserializeObject<WXRspInnerJsonObj>(jobj.data_list[0].json);
                }
            }
            catch(Exception e) 
            {
                return e.StackTrace;
            }
            return $"hi,here is api，{jobj.errcode},{jobj.errmsg}";
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
