using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Firadio
{
    internal class FiradioException : System.Exception
    {
        public string Title;
    }

    internal class HttpUtils
    {
        private string ServerURL;
        public class ClassMessageShow
        {
            public string Text;
            public string Caption;
        }
        public ClassMessageShow MessageShow = new ClassMessageShow();
        public HttpUtils(string _ServerURL)
        {
            ServerURL = _ServerURL;
        }

        public async Task<Firadio.Response> POST数据(string path, Dictionary<string, string> postData)
        {
            WebUtils classWebUtils = new WebUtils();
            if (path.IndexOf("://") > 0)
            {
                classWebUtils.ServerUrl = path;
            }
            else
            {
                classWebUtils.ServerUrl = ServerURL + path;
            }
            string sJson = await classWebUtils.PostForm(postData);
            Response response;
            try
            {
                response = JSON.Parse<Response>(sJson);
            }
            catch (Exception ex)
            {
                MessageShow.Caption = "JSON解析报错";
                MessageShow.Text = sJson;
                throw ex;
            }
            return response;
        }

        private static readonly string[] suffixes = new string[] { " B", " KB", " MB", " GB", " TB", " PB" };
        /// <summary>
        /// 获取文件大小的显示字符串
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string BytesToReadableValue(long number)
        {
            double last = 1;
            for (int i = 0; i < suffixes.Length; i++)
            {
                var current = Math.Pow(1024, i + 1);
                var temp = number / current;
                if (temp < 1)
                {
                    return (number / last).ToString("n2") + suffixes[i];
                }
                last = current;
            }
            return number.ToString();
        }

        static public string PostFormData(string url, Dictionary<string, string> post)
        {
            MultipartFormDataContent postContent = new MultipartFormDataContent();
            postContent.Headers.Add("ContentType", $"multipart/form-data");
            //value-key 键值对
            foreach (KeyValuePair<string, string> keyValuePair in post)
            {
                postContent.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
            }
            //这是Json-key的键值对,param是一个类的对象
            //postContent.Add(new StringContent(param.ToJsonStr()), "content");
            //form表单格式传参
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PostAsync(url, postContent).Result;
            return response.Content.ReadAsStringAsync().Result;

        }

        /// <summary>
        /// 解析JSON，仿Javascript风格
        /// </summary>
        public static class JSON
        {

            public static T Parse<T>(string jsonString)
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                {
                    DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
                    return (T)new DataContractJsonSerializer(typeof(T), settings).ReadObject(ms);
                }
            }

            public static string Stringify(object jsonObject)
            {
                using (var ms = new MemoryStream())
                {
                    DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
                    new DataContractJsonSerializer(jsonObject.GetType(), settings).WriteObject(ms, jsonObject);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }


    }


}
