using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlexiSDK.Util
{
    public class Request
    {
        //CookieContainer cookieJar = new CookieContainer();

        public string ContentType = null;
        public string AcceptType = null;
        public string Method = "GET";
        public Dictionary<string, string> Headers = new Dictionary<string, string>(); 

        public async Task<System.IO.TextReader> DoRequestAsync(WebRequest req, String requestData = "")
        {
            //try
            //{
                Debug.WriteLine(String.Format("post data: {0}{1}", req.RequestUri.Host, req.RequestUri.AbsolutePath));

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
                var sr = new StreamReader(stream);

                return sr;
            //}
            //catch (WebException webErr)
            //{
            //    Debug.WriteLine(String.Format("DoRequestAsync Error: {0}", webErr.Message));
            //    return null;
            //}
        }

        public async Task<System.IO.TextReader> DoRequestAsync(String url, String requestData = "")
        {
            HttpWebRequest req = HttpWebRequest.CreateHttp(url);
            req.Method = this.Method;
            req.AllowReadStreamBuffering = true;
            req.ContentType = ContentType;
            req.Accept = this.AcceptType;
            //req.CookieContainer = this.cookieJar;

            if (this.Headers.Count > 0)
            {
                foreach (KeyValuePair<string, string> header in this.Headers)
                {
                    req.Headers[header.Key] = header.Value;
                }
            }

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

                Debug.WriteLine(String.Format("Response Data: {0}", response));

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

                Debug.WriteLine(String.Format("Web Exception: {0}", errResp));

                return DeserializeData<T>(errResp);
            }

            return default(T);
        }

        #endregion
    }
}
