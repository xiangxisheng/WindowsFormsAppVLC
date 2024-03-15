using System;
using System.Net;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Web;
using System.Threading.Tasks;


namespace Firadio
{
    internal class WebUtils
    {
        /// <summary>
        /// WebClient实体
        /// </summary>
        private readonly WebClient webClient;

        private Action<long, long> actionProgressChanged;
        private Action<int, string> actionCompleted;

        public WebUtils()
        {
            webClient = new WebClient { Encoding = Encoding.UTF8 };
            webClient.UploadProgressChanged += Client_UploadProgressChanged;
            webClient.UploadFileCompleted += Client_UploadFileCompleted;
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged; ;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted; ;
        }

        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            IsLoading = false;
            if (e.Cancelled)
            {
                actionCompleted(0, "下载被取消");
                webClient.Dispose();
                return;
            }
            if (e.Error != null)
            {
                if (e.Error.InnerException != null)
                {
                    actionCompleted(e.Error.InnerException.HResult, e.Error.InnerException.Message);
                    return;
                }
                actionCompleted(e.Error.HResult, e.Error.Message);
                return;
            }
            WebHeaderCollection responseHeaders = webClient.ResponseHeaders;
            if (responseHeaders == null)
            {
                actionCompleted(-1, "下载失败，没有头信息");
                return;
            }
            if (responseHeaders["Content-Error"] != null)
            {
                string error = HttpUtility.UrlDecode(responseHeaders["Content-Error"]);
                actionCompleted(-1, error);
                return;
            }
            if (responseHeaders["X-Status"] == "OK")
            {
                actionCompleted(0, "下载成功");
                return;
            }
            actionCompleted(-1, "下载失败，未知错误");
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (actionProgressChanged == null)
            {
                return;
            }
            actionProgressChanged(e.BytesReceived, e.TotalBytesToReceive);
        }

        /// <summary>
        /// 服务器路径
        /// </summary>
        public string ServerUrl { get; internal set; }
        internal bool IsLoading { get; private set; }

        /// <summary>
        /// 异步上传
        /// </summary>
        /// <param name="serverPath">服务地址</param>
        /// <param name="webHeaderCollection">请求头</param>
        internal void UploadFileAsync(Action<long, long> _callback1, Action<int, string> _callback2, string filePath)
        {
            this.actionProgressChanged = _callback1;
            this.actionCompleted = _callback2;
            IsLoading = true;
            //client.Headers = webHeaderCollection;
            webClient.UploadFileAsync(new Uri(ServerUrl), filePath);
        }
        internal void Header(string key, string val)
        {
            webClient.Headers[key] = val;
        }
        internal void CancelAsync()
        {
            //取消文件传输
            actionCompleted(0, "取消传输");
            IsLoading = false;
            webClient.CancelAsync();
        }

        /// <summary>
        /// 上传完毕
        /// </summary>
        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            IsLoading = false;
            if (e.Cancelled)
            {
                actionCompleted(0, "上传被取消");
                webClient.Dispose();
                return;
            }
            if (e.Error != null)
            {
                if (e.Error.InnerException != null)
                {
                    actionCompleted(e.Error.InnerException.HResult, e.Error.InnerException.Message);
                    return;
                }
                actionCompleted(e.Error.HResult, e.Error.Message);
                return;
            }
            WebHeaderCollection responseHeaders = webClient.ResponseHeaders;
            if (responseHeaders == null)
            {
                actionCompleted(-1, "上传失败，没有头信息");
                return;
            }
            if (responseHeaders["Content-Error"] != null)
            {
                string error = HttpUtility.UrlDecode(responseHeaders["Content-Error"]);
                actionCompleted(-1, error);
                return;
            }
            if (false && responseHeaders["X-Status"] == "OK")
            {
                actionCompleted(0, "上传成功");
                return;
            }
            string sJson = Encoding.UTF8.GetString(e.Result);
            try
            {
                Firadio.Response response = HttpUtils.JSON.Parse<Firadio.Response>(sJson);
                //actionCompleted(response.Errno, response.Message);
            }
            catch (Exception)
            {
                actionCompleted(-1, "上传失败，未知错误");
            }
        }

        /// <summary>
        /// 上传进度
        /// </summary>
        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            //pbUpload.Minimum = 0;
            //pbUpload.Maximum = (int)e.TotalBytesToSend;
            //pbUpload.Value = (int)e.BytesSent;
            //lbUpload.Text = e.ProgressPercentage.ToString();
            if (actionProgressChanged == null)
            {
                return;
            }
            actionProgressChanged(e.BytesSent, e.TotalBytesToSend);
        }

        internal async Task<string> PostString(string postStr)
        {
            byte[] sendData = Encoding.UTF8.GetBytes(postStr);
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            webClient.Headers.Add("ContentLength", sendData.Length.ToString(CultureInfo.InvariantCulture));
            byte[] Result= await webClient.UploadDataTaskAsync(new Uri(ServerUrl), "POST", sendData);
            return Encoding.UTF8.GetString(Result);
        }
        internal async Task<string> PostForm(Dictionary<string, string> dPost)
        {
            string sPost = http_build_query(dPost);
            //sPost = HttpUtils.JSON.Stringify(dPost);
            return await PostString(sPost);
        }

        public static string http_build_query(Dictionary<string, string> dict = null)
        {
            if (dict == null)
            {
                return "";
            }
            List<string> QueryStrings = new List<string>();

            foreach (KeyValuePair<string, string> kvp in dict)
            {
                QueryStrings.Add(System.Net.WebUtility.UrlEncode(kvp.Key) + "=" + System.Net.WebUtility.UrlEncode(kvp.Value));
            }
            return string.Join("&", QueryStrings);
        }

        internal void DownloadFileAsync(Action<long, long> _callback1, Action<int, string> _callback2, string savePath)
        {
            this.actionProgressChanged = _callback1;
            this.actionCompleted = _callback2;
            IsLoading = true;
            //client.Headers = webHeaderCollection;
            webClient.DownloadFileAsync(new Uri(ServerUrl), savePath);
        }
    }
}
