using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpUtil
{
    public class HttpUtil
    {
        private CookieContainer cookie;

        private string HttpPost(string URL, string postDataStr) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            request.CookieContainer = cookie;
            Stream myRequestStream= request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream,Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Cookies = cookie.GetCookies(response.ResponseUri);
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myRequestStream,Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        private string HttpGet(string Url, string postDataStr) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url+ (postDataStr==""?"":"?")+postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            Stream myResponseSteam = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseSteam,Encoding.GetEncoding("UTF-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseSteam.Close();
            return retString;
        }

    }
}
