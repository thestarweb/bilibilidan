using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Web;

namespace bilibilidan
{
	internal static class dmSender
	{
		private static string html = @"POST /msg/send HTTP/1.1
Accept	*/*
Accept-Language	zh-CN
Referer	http://static.hdslb.com/live-static/swf/LivePlayerEx_1.swf
x-flash-version: 15,0,0,167
Content-Type: application/x-www-form-urlencoded
Content-Length: {con_len}
Accept-Encoding: deflate
User-Agent:{UA}
Host: live.bilibili.com
DNT: 1
Connection: Keep-Alive
Cache-Control:no-cache
Cookie: {cookie}

";
		private static string body = "color=16777215&fontsize=25&mode=1&msg={msg}&roomid={room}";
		public static void send(string con,int room){
			string b=body.Replace("{msg}", HttpUtility.UrlEncode (con)).Replace("{room}",room.ToString());
			string s=html.Replace("{con_len}",b.Length.ToString()).Replace("{UA}",ini.UA).Replace("{cookie}",ini.cookie);
			s+=b+"\r\n";
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect("live.bilibili.com", 80);
			byte[] bs = Encoding.ASCII.GetBytes(s);//
			socket.Send(bs);
			socket.Receive(bs, 0, bs.Length, 0);
		}
	}
}
