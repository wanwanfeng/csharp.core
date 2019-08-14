using System;
using System.IO;
using System.Net;
using System.Text;

namespace Library.Helper
{
    /// <summary>
    ///https://blog.csdn.net/zghnpdswyp/article/details/77164080 
    /// </summary>
    public class WebHelper
    {
        public class Base
        {
            public static CookieContainer cookie;
            public static string Host = "";
            public Action<bool, string> OnComplete;

            protected void Callback(HttpWebRequest request)
            {
                try
                {
                    var response = (HttpWebResponse) request.GetResponse();
                    response.Cookies = cookie.GetCookies(response.ResponseUri);
                    using (var rs = response.GetResponseStream())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var sr = new StreamReader(rs, Encoding.GetEncoding("utf-8")))
                            {
                                OnComplete.Invoke(true, sr.ReadToEnd());
                            }
                        }
                        else
                        {
                            OnComplete.Invoke(false, response.StatusDescription);
                        }
                    }
                }
                catch (WebException e)
                {
                    OnComplete.Invoke(false, e.Message);
                }
                catch (Exception e)
                {
                    OnComplete.Invoke(false, e.Message);
                }
            }

            public void Post(string url, string args)
            {
                var uri = new Uri(Host + url);
                var request = (HttpWebRequest) WebRequest.Create(uri);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = Encoding.UTF8.GetByteCount(args);
                request.CookieContainer = cookie;
                using (var rs = request.GetRequestStream())
                {
                    StreamWriter sw = new StreamWriter(rs, Encoding.GetEncoding("gb2312"));
                    sw.Write(args);
                }

                Callback(request);
            }

            public void Get(string url, string args)
            {
                var uri = new Uri(Host + url + (args == "" ? "" : "?") + args);
                var request = (HttpWebRequest) WebRequest.Create(uri);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.CookieContainer = cookie; 

                Callback(request);
            }
        }

        public static void Post(string url, string args = null, Action<bool, string> callAction = null)
        {
            new Base()
            {
                OnComplete = callAction
            }.Post(url, args);
        }

        public static void Get(string url, string args = null, Action<bool, string> callAction = null)
        {
            new Base()
            {
                OnComplete = callAction
            }.Get(url, args);
        }
    }
}