using System;
using System.IO;
using System.Net;
using System.Text;
using Library.Extensions;

namespace Library.Helper
{
    /// <summary>
    ///https://blog.csdn.net/zghnpdswyp/article/details/77164080 
    /// </summary>
    public class WebHelper
    {
        public class Request
        {
            public static CookieContainer cookie;
            public static string Host = "";
            public Action<bool, string> OnComplete;

            public void GetResponseValue(HttpWebRequest request)
            {
                try
                {
                    using (var response = (HttpWebResponse) request.GetResponse())
                    {
                        if (cookie != null)
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

            public string GetResponsePath(HttpWebRequest request)
            {
                try
                {
                    var path = Path.GetTempFileName();
                    using (var response = (HttpWebResponse) request.GetResponse())
                    {
                        if (cookie != null)
                            response.Cookies = cookie.GetCookies(response.ResponseUri);
                        using (var myResponseStream = response.GetResponseStream())
                        {
                            if (myResponseStream == null)
                            {
                                OnComplete.Invoke(false, null);
                                return null;
                            }
                            using (var fw = new FileStream(path, FileMode.OpenOrCreate))
                            {
                                myResponseStream.CopyTo(fw);
                            }
                        }
                    }
                    OnComplete.Invoke(false, path);
                    return path;
                }
                catch (WebException e)
                {
                    OnComplete.Invoke(false, e.Message);
                    return null;
                }
                catch (Exception e)
                {
                    OnComplete.Invoke(false, e.Message);
                    return null;
                }
            }

            public HttpWebRequest Post(string url, string args = "")
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

                return request;
            }

            public HttpWebRequest Get(string url, string args = "")
            {
                var uri = new Uri(Host + url + (args == "" ? "" : "?") + args);
                var request = (HttpWebRequest) WebRequest.Create(uri);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.CookieContainer = cookie;

                return request;
            }
        }

        public static void Post(string url, string args, Action<bool, string> callAction)
        {
            var request = new Request()
            {
                OnComplete = callAction
            };
            request.GetResponseValue(request.Post(url, args));

        }

        public static void Get(string url, string args, Action<bool, string> callAction)
        {
            var request = new Request()
            {
                OnComplete = callAction
            };
            request.GetResponseValue(request.Get(url, args));
        }

        public static string GetFile(string url)
        {
            var request = new Request()
            {
                OnComplete = (state, res) => { }
            };
            return request.GetResponsePath(request.Get(url));
        }
    }
}