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
using Newtonsoft.Json.Linq;

namespace Plexi.Util
{
    public class Request
    {
        //CookieContainer cookieJar = new CookieContainer();

        public String ContentType = null;
        public String AcceptType = null;
        public String Method = "GET";

        public async Task<System.IO.TextReader> DoRequestAsync(WebRequest req, String requestData = "")
        {
            Debug.WriteLine("POST DATA");
            Debug.WriteLine(req.RequestUri.AbsolutePath);
            
            // if we have post/put data, write it to the request stream
            if ((req.Method == "POST" || req.Method == "PUT") && requestData.Length > 0)
            {
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

        #region json methods 
        public async Task<T> DoRequestJsonAsync<T>(WebRequest req, String requestData = "")
        {
            try
            {
                System.IO.TextReader ret = await DoRequestAsync(req, requestData);
                String response = await ret.ReadToEndAsync();

                return DeserializeData<T>(response);
            }
            catch (WebException err)
            {
                return HandleWebException<T>(err);
            }
        }

        public async Task<T> DoRequestJsonAsync<T>(String uri, String requestData = "")
        {
            try
            {
                System.IO.TextReader ret = await DoRequestAsync(uri, requestData);
                String response = await ret.ReadToEndAsync();

                Debug.WriteLine("RESPONSE DATA");
                Debug.WriteLine(response);

                return DeserializeData<T>(response);
            }
            catch (WebException err)
            {
                return HandleWebException<T>(err);
            }
        }

        public async Task<T> DoRequestJsonAsync<T>(String uri, object requestData)
        {
            return await DoRequestJsonAsync<T>(uri, SerializeData(requestData));
        }
        #endregion

        #region helpers
        private string SerializeData(object data, bool includeNulls = false)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Include;
            jsonSettings.NullValueHandling = (includeNulls == true) ? NullValueHandling.Include : NullValueHandling.Ignore;

            return JsonConvert.SerializeObject(data, jsonSettings);
        }

        private T DeserializeData<T>(String data)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonSettings.NullValueHandling = NullValueHandling.Include;

            return JsonConvert.DeserializeObject<T>(data, jsonSettings);
        }

        private T HandleWebException<T>(WebException e)
        {
            var resp = (HttpWebResponse)e.Response;

            if (resp.StatusCode != HttpStatusCode.OK)
            {
                System.IO.Stream stream = resp.GetResponseStream();

                System.IO.StreamReader reader = new System.IO.StreamReader(stream);

                String errResp = reader.ReadToEnd();

                return DeserializeData<T>(errResp);
            }

            return default(T);
        }

        #endregion
    }
}
