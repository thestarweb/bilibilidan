using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using bilibilidan.json;
using System.Threading;

namespace bilibilidan
{
    class roomInfo
    {
        private string _roomTitle;
        public string getTitle()
		{
				Socket socket0 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket0.Connect("api.live.bilibili.com", 80);
				byte[] bs = Encoding.ASCII.GetBytes(@"GET /room/v1/Room/get_info?from=room&room_id=" + _roomId + http);//
				socket0.Send(bs);
				Byte[] buffer = new byte[20480];
				Thread.Sleep(100);
				int l = socket0.Receive(buffer, 0, buffer.Length, 0);
				string s = Encoding.UTF8.GetString(buffer, 0, l);
				jsonReader j = new jsonReader(s.Substring(s.IndexOf("{")));
				j = j.get_o("data");
				s=j.get("title");
				return s;
				//return _roomTitle;
		}
		private string _roomId;
        private string _roomNum;
        public string roomNum
        {
            get
            {
                return _roomNum;
            }
        }
        private string _uper;
        public string uper
        {
            get
            {
                return _uper;
            }
        }
        public roomInfo(string room)
        {
            _roomNum=_roomInfo(room);
        }
		string http =  @" HTTP/1.1
Host: api.live.bilibili.com
User-Agent: star_danmuji
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
Connection: keep-alive


";
        public string _roomInfo(string room)
        {

            
            Socket socket0 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket0.Connect("api.live.bilibili.com", 80);
			byte[] bs = Encoding.ASCII.GetBytes(@"GET /room/v1/Room/room_init?id=" + room + http);//
            socket0.Send(bs);
            Byte[] buffer=new byte[4096];
            int l = socket0.Receive(buffer, 0, buffer.Length, 0);
            string s = Encoding.UTF8.GetString(buffer, 0, l);
            //window.write(s.IndexOf("3").ToString());
            try
            {
				jsonReader j = new jsonReader(s.Substring(s.IndexOf("{")));
				if (j.get("code") == "0")
				{
					_roomId = j.get_o("data").get("room_id");
					MainWindow.main.write("已识别真实房间号：" + _roomId);
					string short_id = j.get_o("data").get("short_id");
					if(short_id!="0") {
						MainWindow.main.write("房间短号：" + short_id);
					}

					//识别up
					socket0.Send(Encoding.ASCII.GetBytes(@"GET /live_user/v1/UserInfo/get_anchor_in_room?roomid=" + _roomId + http));
					l = socket0.Receive(buffer, 0, buffer.Length, 0);
					s = Encoding.UTF8.GetString(buffer, 0, l);
					j = new jsonReader(s.Substring(s.IndexOf("{")));
					_uper = j.get_o("data").get_o("info").get("uname");
                    return _roomId;
				}
				else 
				{
					MainWindow.main.write("解析失败:"+j.get("message"));
                }
            }
            catch (Exception)
            {
				MainWindow.main.write("解析房间地址时发生未知错误！");
            }
            l = socket0.Receive(buffer, 0, buffer.Length, 0);
            s = Encoding.UTF8.GetString(buffer, 0, l);
            return "";
        }
    }
}
