using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreSys.Common
{
    /// <summary>
    /// 表示HTTP请求辅助类
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// Http发送Get请求方法
        /// </summary>
        /// <param name="Url">请求地址</param>
        /// <param name="postDataStr">参数</param>
        /// <returns></returns>
        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            ///证书验证返回true,ssl异常
            ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback((s, c, ch, ss) => true);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, System.Text.Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        // <summary>
        /// Http发送Get请求方法
        /// </summary>
        /// <param name="Url">请求地址</param>
        /// <param name="postDataStr">参数</param>
        /// <param name="timeout">超时，默认5000毫秒</parama>
        /// <returns></returns>
        public static string HttpGet(string Url, string postDataStr, out string hash, int timeout = 5000)
        {
            hash = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Timeout = timeout;//5秒
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback((s, c, ch, ss) => true);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            hash = response.Headers.Get("PDF-HASH");
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, System.Text.Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        /// <summary>
        /// Http发送Post请求方法
        /// </summary>
        /// <param name="Url">请求地址</param>
        /// <param name="postDataStr">参数</param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr, int retryTimes = 0)
        {
            byte[] postdata = Encoding.UTF8.GetBytes(postDataStr);
            WebRequest request = WebRequest.Create(Url);
            request.Credentials = CredentialCache.DefaultCredentials;
            //request.Accept = "*/*";
            //request.KeepAlive = true;
            request.Timeout = 3000;
            //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;";
            //request.MediaType = "text/plain;charset=utf-8";
            request.ContentLength = postdata.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
            writer.Write(postdata);
            writer.Flush();
            string retString = "";
            int retryCounter = 0;
            Exception e = null;

        DOREQUEST:
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码 
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(encoding));
                retString = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                e = ex;
            }
            finally
            {

            }

            if (retryTimes > 0 && retryCounter <= retryTimes && e != null)
            {
                Thread.Sleep(500);
                retryCounter += 1;
                goto DOREQUEST;
            }
            else if (e != null)
            {
                throw e;
            }

            return retString;
        }

        /// <summary>
        /// Http Get Stream
        /// </summary>
        /// <param name="url">get 地址</param>
        /// <param name="retryTimes">重试次数</param>
        /// <returns></returns>
        public static MemoryStream HttpGetStreamViaClient(string url, out string contentType, int retryTimes = 0)
        {
            var stream = new MemoryStream();
            int retryCounter = 0;
            Exception ex = null;
            HttpResponseMessage response = null;
            var sslHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip,
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
            };

            using (var client = new HttpClient(sslHandler))
            {
                contentType = "application/octet-stream";
                string userAgen = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36";
                client.DefaultRequestHeaders.Add("user-agent", userAgen);
                client.CancelPendingRequests();
                client.DefaultRequestHeaders.Clear();

            DOREQUEST:
                try
                {
                    Task<HttpResponseMessage> taskResponse = client.GetAsync(url);
                    taskResponse.Wait();
                    response = taskResponse.Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var ct = response.Content.Headers.GetValues("Content-Type");
                        if (ct != null)
                        {
                            var enu = ct.GetEnumerator();
                            if (enu.MoveNext())
                            {
                                contentType = enu.Current;
                            }
                        }
                        Task<System.IO.Stream> taskStream = response.Content.ReadAsStreamAsync();
                        taskStream.Wait();
                        taskStream.Result.Position = 0;                   
                        taskStream.Result.CopyTo(stream);                        
                    }
                    else
                    {
                        ex = new HttpRequestException($"请求失败（StatusCode {(int)response.StatusCode}）");
                    }
                }
                catch (Exception e)
                {
                    ex = e;
                    response?.Dispose();
                }

                if (retryTimes > 0 && retryCounter <= retryTimes && ex != null)
                {
                    Thread.Sleep(500);
                    retryCounter += 1;
                    goto DOREQUEST;
                }
                else if (ex != null)
                {
                    throw ex;
                }
            }

            return stream;
        }

        /// <summary>
        /// Post请求返回字符
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postDataStr">请求数据</param>
        /// <param name="retryTimes">重试次数</param>
        /// <returns>字符</returns>
        public static string PostViaHttpClient(string url, string postDataStr, int retryTimes = 0)
        {
            string responsedata = string.Empty;
            Exception ex = null;
            int retryCounter = 0;
            HttpClient httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });
            string userAgen = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36";
            HttpResponseMessage response = null;

        DOREQUEST:
            try
            {
                //using (HttpClient httpClient = new HttpClient())
                //{
                httpClient.MaxResponseContentBufferSize = 256000;
                httpClient.DefaultRequestHeaders.Add("user-agent", userAgen);
                //httpClient.DefaultRequestHeaders.Add("content-type", "application/x-www-form-urlencoded;");
                httpClient.CancelPendingRequests();
                httpClient.DefaultRequestHeaders.Clear();
                string postData = postDataStr;
                HttpContent httpContent = new StringContent(postData);
                //httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                Task<HttpResponseMessage> taskResponse = httpClient.PostAsync(url, httpContent);
                taskResponse.Wait();
                response = taskResponse.Result;

                if (response.IsSuccessStatusCode)
                {
                    Task<System.IO.Stream> taskStream = response.Content.ReadAsStreamAsync();
                    taskStream.Wait();
                    System.IO.Stream dataStream = taskStream.Result;
                    System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                    responsedata = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();
                }
                if (httpClient != null)
                {
                    httpClient.Dispose();
                }
            }

            if (retryTimes > 0 && retryCounter <= retryTimes && ex != null)
            {
                Thread.Sleep(500);
                retryCounter += 1;
                goto DOREQUEST;
            }
            else if (ex != null)
            {
                throw ex;
            }

            return responsedata;
        }

        /// <summary>
        /// Get请求返回字符
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="httpStatusCode">传出http状态码</param>
        /// <param name="retryTimes">重试次数</param>
        /// <returns></returns>
        public static string GetViaHttpClient(string url, out string httpStatusCode, int retryTimes = 0)
        {
            string responsedata = string.Empty;
            Exception ex = null;
            int retryCounter = 0;

            httpStatusCode = string.Empty;
            string userAgen = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36";
            HttpClient httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });
            HttpResponseMessage response = null;

        DOREQUEST:
            try
            {
                httpClient.DefaultRequestHeaders.Add("user-agent", userAgen);
                httpClient.CancelPendingRequests();
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                   new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> taskResponse = httpClient.GetAsync(url);
                taskResponse.Wait();
                response = taskResponse.Result;
                if (response.IsSuccessStatusCode)
                {
                    Task<System.IO.Stream> taskStream = response.Content.ReadAsStreamAsync();
                    taskStream.Wait();
                    //此处会抛出异常：不支持超时设置，对返回结果没有影响
                    System.IO.Stream dataStream = taskStream.Result;
                    System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                    responsedata = reader.ReadToEnd();
                }
            }
            catch (Exception x)
            {
                ex = x;
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();
                }
                if (httpClient != null)
                {
                    httpClient.Dispose();
                }
            }

            if (retryTimes > 0 && retryCounter <= retryTimes && ex != null)
            {
                Thread.Sleep(500);
                retryCounter += 1;
                goto DOREQUEST;
            }
            else if (ex != null)
            {
                throw ex;
            }

            return responsedata;
        }

        /// <summary>
        /// Post请求返回stream
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postDataStr">请求数据</param>
        /// <param name="retryTimes">重试次数</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream PostHttpClient(string url, string postDataStr, int retryTimes = 0)
        {
            Exception ex = null;
            int retryCounter = 0;
            var memoryStream = new MemoryStream();

            var sslHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
            };

            using (var client = new HttpClient(sslHandler))
            {
            DOREQUEST:
                try
                {
                    //序列化
                    HttpContent content = new StringContent(postDataStr);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    Task<HttpResponseMessage> taskResponse = client.PostAsync(url, content);//改成自己的                    
                    var response = taskResponse.Result;
                    response.EnsureSuccessStatusCode();//用来抛异常的
                    if (response.IsSuccessStatusCode)
                    {
                        Task<System.IO.Stream> taskStream = response.Content.ReadAsStreamAsync();
                        taskStream.Result.CopyTo(memoryStream);
                    }
                }
                catch (Exception e)
                {
                    ex = e;
                }

                if (retryTimes > 0 && retryCounter <= retryTimes && ex != null)
                {
                    Thread.Sleep(500);
                    retryCounter += 1;
                    goto DOREQUEST;
                }
                else if (ex != null)
                {
                    throw ex;
                }

                return memoryStream;
            }
        }

        // <summary>
        /// post方法（使用该方法请求Api时，Api中参数前需要加[FromBody]）
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="PostData">JsonConvert.SerializeObject(post数据对象)</param>
        /// <param name="timeout">超时时间，默认为60000毫秒（1分钟）</param>
        /// <returns></returns>
        public static Stream HttpPostStream(string url, string PostDatastr, int timeout = 60000)
        {
            HttpWebRequest request = null;
            //HTTPSQ请求
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Timeout = timeout;
            //如果需要POST数据
            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(PostDatastr);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var resultStream = (request.GetResponse() as HttpWebResponse);

            //获取响应内容            
            return resultStream.GetResponseStream();
        }
    }
}
