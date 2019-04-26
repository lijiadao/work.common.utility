using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Work.Http
{
    /// <summary>
    /// http://tmenier.github.io/Flurl/
    /// </summary>
    public class HttpMethods
    {
        /// <summary>
        /// post json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="cookies"> new { h1 = "foo", h2 = "bar" } </param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<T> PostJsonAsync<T>(string url
            , object postData = null
            , object cookies = null
            , int timeout = 15)
        {
            try
            {
                if (postData == null)
                {
                    postData = new { };
                }
                var client = url.WithTimeout(timeout);
                if (cookies != null)
                {
                    client.WithCookies(cookies, System.DateTime.Now.AddDays(1));
                }
                var poco = await client.PostJsonAsync(postData).ReceiveJson<T>();
                return poco;
            }
            catch (FlurlHttpTimeoutException)
            {
                throw new Exception("Timed out!");
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.Response != null)
                    throw new Exception("Failed with response code " + ex.Call.Response.StatusCode + "," + ex.Message);
                else
                    throw new Exception("Totally failed before getting a response! " + ex.Message);
            }
        }


        /// <summary>
        /// form 表单提交
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="cookies"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<T> PostFormStringAsync<T>(string url
            , string postData = null
            , object cookies = null
            , int timeout = 15)
        {
            try
            {
                var client = url.WithTimeout(timeout);
                if (cookies != null)
                {
                    client.WithCookies(cookies, System.DateTime.Now.AddDays(1));
                }
                HttpContent content = new StringContent(postData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var poco = await client.PostAsync(content).ReceiveJson<T>();
                return poco;
            }
            catch (FlurlHttpTimeoutException)
            {
                throw new Exception("Timed out!");
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.Response != null)
                    throw new Exception("Failed with response code " + ex.Call.Response.StatusCode + "," + ex.Message);
                else
                    throw new Exception("Totally failed before getting a response! " + ex.Message);
            }
        }


        /// <summary>
        /// get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="queryParams">键值对形式</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string url
            , object queryParams
            , int timeout = 15)
        {
            try
            {
                var poco = await url.SetQueryParams(queryParams).WithTimeout(timeout).GetJsonAsync<T>();
                return poco;
            }
            catch (FlurlHttpTimeoutException)
            {
                throw new Exception("Timed out!");
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.Response != null)
                    throw new Exception("Failed with response code " + ex.Call.Response.StatusCode + "," + ex.Message);
                else
                    throw new Exception("Totally failed before getting a response! " + ex.Message);
            }
        }



        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">POST的数据</param>
        /// <returns></returns>
        public static string HttpPost(string url, string param = null, string contentType = null, int timeout = 30000)
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Method = "POST";
            if (contentType == null)
                request.ContentType = "application/x-www-form-urlencoded";
            else
                request.ContentType = contentType;
            request.Accept = "*/*";
            request.Timeout = timeout;
            request.AllowAutoRedirect = false;



            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());

                requestStream.Write(param);
                requestStream.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }

            return responseStr;
        }


        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
    }
}
