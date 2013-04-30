using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Please.Util
{

    public class Request
    {
        static CookieContainer cookieJar = new CookieContainer();

        public static async Task<System.IO.TextReader> DoRequestAsync(WebRequest req, String requestData = "")
        {
            // if we have post/put data, write it to the request stream
            if ((req.Method == "POST" || req.Method == "PUT") && requestData.Length > 0)
            {
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                req.ContentLength = data.Length;

                using (var reqStream = await Task<Stream>.Factory.FromAsync(req.BeginGetRequestStream, req.EndGetRequestStream, req))
                {
                    await reqStream.WriteAsync(data, 0, data.Length);
                }
            }

            //var task = Task.Factory.FromAsync((cb, o) => ((HttpWebRequest)o).BeginGetResponse(cb, o), res => ((HttpWebRequest)res.AsyncState).EndGetResponse(res), req);
            var task = Task<WebResponse>.Factory.FromAsync(req.BeginGetResponse, req.EndGetResponse, req);

            var result = await task;
            var resp = result;
            var stream = resp.GetResponseStream();
            var sr = new System.IO.StreamReader(stream);

            return sr;
        }

        public static async Task<System.IO.TextReader> DoRequestAsync(String url, String requestMethod = "GET", String requestData = "")
        {
            HttpWebRequest req = HttpWebRequest.CreateHttp(url);
            req.Method = requestMethod;
            req.AllowReadStreamBuffering = true;

            req.CookieContainer = Please.Util.Request.cookieJar;
            
            var tr = await DoRequestAsync(req, requestData);

            return tr;
        }

        public static async Task<T> DoRequestJsonAsync<T>(WebRequest req, String requestData = "")
        {
            var ret = await DoRequestAsync(req, requestData);
            var response = await ret.ReadToEndAsync();

            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonSettings.NullValueHandling = NullValueHandling.Include;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response, jsonSettings);
        }

        public static async Task<T> DoRequestJsonAsync<T>(String uri, String requestMethod = "GET", String requestData = "")
        {
            var ret = await DoRequestAsync(uri, requestMethod, requestData);
            var response = await ret.ReadToEndAsync();

            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonSettings.NullValueHandling = NullValueHandling.Include;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response, jsonSettings);
        }
    }
}
