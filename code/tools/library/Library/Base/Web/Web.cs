using System;
using System.IO;
using System.Net;
using System.Text;

namespace Library.Web
{
    /// <summary>
    ///https://blog.csdn.net/zghnpdswyp/article/details/77164080 
    /// </summary>
    public class Web
    {
        public class Base
        {
            public static CookieContainer cookie;
            public static string Host = "";
            public Action OnStart;
            public Action<string> OnComplete;
        }

        public class Post : Base
        {
            public Post(string url, string postDataStr)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                request.CookieContainer = cookie;
                Stream myRequestStream = request.GetRequestStream();
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                myStreamWriter.Write(postDataStr);
                myStreamWriter.Close();

                HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                response.Cookies = cookie.GetCookies(response.ResponseUri);
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                OnComplete.Invoke(retString);
            }
        }

        public class Get : Base
        {
            public Get(string url, string postDataStr)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host + url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                OnComplete.Invoke(retString);
            }
        }
    }
}