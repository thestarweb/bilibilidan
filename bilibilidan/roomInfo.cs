using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace bilibilidan
{
    class roomInfo
    {
        public static string getTrueRoomNum(string room)
        {

            string http = @"GET /" + room + @" HTTP/1.1
Host: live.bilibili.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: deflate
Connection: keep-alive
Cache-Control: max-age=0

";
            Socket socket0 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket0.Connect("live.bilibili.com", 80);
            byte[] bs = Encoding.ASCII.GetBytes(http);//
            socket0.Send(bs);
            Byte[] buffer=new byte[2048];
            int l = socket0.Receive(buffer, 0, buffer.Length, 0);
            string s = Encoding.UTF8.GetString(buffer, 0, l);
            //window.write(s.IndexOf("3").ToString());
            if (s.Length > 8 && s.IndexOf("3") == 9)
            {

                return getTrueRoomNum(Regex.Match(s, "location: http://live.bilibili.com/(\\d+)").ToString().Substring(35));
            }
            while (l > 0)
            {
                try
                {
                    s = Regex.Match(s, "var ROOMID = \\d+;").ToString();
                    string[] list = s.Split('=');
                    return list[1].Substring(1, list[1].Length - 2);
                    break;
                }
                catch (Exception)
                {

                }
                l = socket0.Receive(buffer, 0, buffer.Length, 0);
                s = Encoding.UTF8.GetString(buffer, 0, l);
            }
            return "";
        }
        public static string[] getRoomInfo(string room)
        {
            string http = @"GET /live/getInfo?roomid=" + room + @" HTTP/1.1
Host: live.bilibili.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: deflate
Connection: keep-alive
Cache-Control: max-age=0

";
            Socket socket0 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket0.Connect("live.bilibili.com", 80);
            byte[] bs = Encoding.ASCII.GetBytes(http);//
            socket0.Send(bs);
            Byte[] buffer = new byte[2048];
            int l = socket0.Receive(buffer, 0, buffer.Length, 0);
            string s = Encoding.UTF8.GetString(buffer, 0, l);
            string data = new json.jsonReader(s.Substring(s.IndexOf("{"))).get("data");
            json.jsonReader jr = new json.jsonReader(data);
            string[] res = { jr.get("ANCHOR_NICK_NAME"),jr.get("ROOMTITLE") };
            return res;
        }
    }
}
