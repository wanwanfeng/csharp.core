using Library.Extensions;
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
        private class Base
        {
            public static CookieContainer cookie = new CookieContainer();
            public static string Host = "";
            public Action<bool, byte[]> OnComplete;

            protected void Callback(HttpWebRequest request)
            {
                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    response.Cookies = cookie.GetCookies(response.ResponseUri);
                    using (var rs = response.GetResponseStream())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var sm = new MemoryStream())
                            {
                                rs.CopyTo(sm);
                                OnComplete.Call(true, sm.ToArray());
                            }
                        }
                        else
                        {
                            OnComplete.Call(false, Encoding.UTF8.GetBytes(response.StatusDescription));
                        }
                    }
                }
                catch (WebException e)
                {
                    OnComplete.Call(false, Encoding.UTF8.GetBytes(e.Message));
                }
                catch (Exception e)
                {
                    OnComplete.Call(false, Encoding.UTF8.GetBytes(e.Message));
                }
            }

            public void Post(string url, string args)
            {
                var uri = new Uri(Host + url);
                var request = (HttpWebRequest)WebRequest.Create(uri);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.CookieContainer = cookie;

                if (!string.IsNullOrEmpty(args))
                {
                    request.ContentLength = Encoding.UTF8.GetByteCount(args);
                    using (var rs = request.GetRequestStream())
                    {
                        using (var sw = new StreamWriter(rs, Encoding.UTF8))
                        {
                            sw.Write(args);
                        }
                    }
                }

                Callback(request);
            }

            public void Get(string url, string args)
            {
                Uri uri;
                if (!string.IsNullOrEmpty(args))
                {
                    uri = new Uri(Host + url + "?" + args);
                }
                else
                {
                    uri = new Uri(Host + url);
                }
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.CookieContainer = cookie;

                Callback(request);
            }

            public void Head(string url, string args)
            {
                var request = (HttpWebRequest)WebRequest.Create(new Uri(Host + url));
                request.Method = "Head";
                request.CookieContainer = cookie;

                var response = (HttpWebResponse)request.GetResponse();
                using (var rs = response.GetResponseStream())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        OnComplete.Call(true, null);
                    }
                    else
                    {
                        OnComplete.Call(false, Encoding.UTF8.GetBytes(response.StatusDescription));
                    }
                }
            }
        }

        public static void Post(string url, string args = null, Action<bool, byte[]> callAction = null)
        {
            new Base()
            {
                OnComplete = callAction
            }.Post(url, args);
        }

        public static void Get(string url, string args = null, Action<bool, byte[]> callAction = null)
        {
            new Base()
            {
                OnComplete = callAction
            }.Get(url, args);
        }

        public static void Head(string url, string args = null, Action<bool, byte[]> callAction = null)
        {
            new Base()
            {
                OnComplete = callAction
            }.Head(url, args);
        }
    }
}