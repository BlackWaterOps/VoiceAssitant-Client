using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Phone.Controls
{
    public class WebThumbnailedImage : IThumbnailedImageAsync
    {
        public Uri ImagePath { get; private set; }

        public WebThumbnailedImage(string path)
        {
            ImagePath = new Uri(path, UriKind.Absolute);
        }

        /// <summary>
        /// Returns a Stream representing the thumbnail image.
        /// </summary>
        /// <returns>Stream of the thumbnail image.</returns>
        public async Task<Stream> GetThumbnailImage()
        {
            return await GetImageStream();
        }

        /// <summary>
        /// Returns a Stream representing the full resolution image.
        /// </summary>
        /// <returns>Stream of the full resolution image.</returns>
        public async Task<Stream> GetImage()
        {
            return await GetImageStream();
        }

        /// <summary>
        /// Represents the time the photo was taken, useful for sorting photos.
        /// </summary>
        public DateTime DateTaken
        {
            get
            {
                return DateTime.Now;
            }
        }

        private async Task<Stream> GetImageStream()
        {
            try
            {
                HttpWebRequest req = HttpWebRequest.CreateHttp(this.ImagePath);

                req.AllowReadStreamBuffering = true;

                var task = Task<WebResponse>.Factory.FromAsync(req.BeginGetResponse, req.EndGetResponse, req);

                var result = await task;
                
                var resp = result;

                return resp.GetResponseStream();
            }
            catch (WebException webErr)
            {
                Debug.WriteLine(String.Format("DoRequestAsync Error: {0}", webErr.Message));
                return null;
            }
        }
    }
}
