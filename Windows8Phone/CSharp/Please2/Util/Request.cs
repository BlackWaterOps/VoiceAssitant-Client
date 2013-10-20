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

namespace Please2.Util
{

    public class Request
    {
        CookieContainer cookieJar = new CookieContainer();

        public String ContentType = null;
        public String AcceptType = null;
        public String Method = "GET";

        public async Task<System.IO.TextReader> DoRequestAsync(WebRequest req, String requestData = "")
        {
            // if we have post/put data, write it to the request stream
            if ((req.Method == "POST" || req.Method == "PUT") && requestData.Length > 0)
            {
                Debug.WriteLine("POST DATA");
                Debug.WriteLine(requestData);

                byte[] data = Encoding.UTF8.GetBytes(requestData);
                req.ContentLength = data.Length;

                using (var reqStream = await Task<Stream>.Factory.FromAsync(req.BeginGetRequestStream, req.EndGetRequestStream, req))
                {
                    await reqStream.WriteAsync(data, 0, data.Length);
                }
            }

            var task = Task<WebResponse>.Factory.FromAsync(req.BeginGetResponse, req.EndGetResponse, req);

            var result = await task;

            var resp = result;
            var stream = resp.GetResponseStream();
            var sr = new System.IO.StreamReader(stream);

            return sr;
        }

        public async Task<System.IO.TextReader> DoRequestAsync(String url, String requestData = "")
        {
            HttpWebRequest req = HttpWebRequest.CreateHttp(url);
            req.Method = Method;
            req.AllowReadStreamBuffering = true;
            req.ContentType = ContentType;
            req.Accept = this.AcceptType;
            //req.CookieContainer = this.cookieJar;
            
            var tr = await DoRequestAsync(req, requestData);

            return tr;
        }

        public async Task<T> DoRequestJsonAsync<T>(WebRequest req, String requestData = "")
        {
            var ret = await DoRequestAsync(req, requestData);
            var response = await ret.ReadToEndAsync();

            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonSettings.NullValueHandling = NullValueHandling.Include;

            return JsonConvert.DeserializeObject<T>(response, jsonSettings);
        }

        public async Task<T> DoRequestJsonAsync<T>(String uri, String requestData = "")
        {
            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonSettings.NullValueHandling = NullValueHandling.Include;

            try
            {
                var ret = await DoRequestAsync(uri, requestData);
                var response = await ret.ReadToEndAsync();

                Debug.WriteLine("RESPONSE DATA");
                Debug.WriteLine(response.ToString());

                return JsonConvert.DeserializeObject<T>(response, jsonSettings);
            }
            catch (WebException err)
            {
                var resp = (HttpWebResponse)err.Response;

                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    var stream = resp.GetResponseStream();

                    var reader = new System.IO.StreamReader(stream);

                    var errResp = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<T>(errResp, jsonSettings);
                }

                return default(T);
            }
        }
    }
}
