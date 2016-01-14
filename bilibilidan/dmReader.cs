using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using bilibilidan.json;
using System.Windows;
using System.Text.RegularExpressions;

namespace bilibilidan
{
	internal class dmReader
	{
		private MainWindow window;
		private byte[] re=BitConverter.GetBytes(0x04000201);//01 02 00 04
		private Socket socket=null;
		private byte[] buffer = new byte[1024];
		private bool flag=false;
        private bool keeping_link=false;
		public dmReader(MainWindow win)
		{
			window=win;
			// = IPAddress.Parse("127.0.0.1");
		}
		public bool link(int room){
			if (socket != null) unlink();
			Thread.Sleep(10);
			socket=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			string r =room.ToString();
			string http1 = @"GET /api/player?id=cid:"+r+@" HTTP/1.1
Host: live.bilibili.com
Connection: Keep-Alive

";
			try{
                flash_info(r);
                //socket0.Receive(buffer, 0, buffer.Length, 0);
                //socket0.Receive(buffer, 0, buffer.Length, 0);
                byte[] bs =new byte[12];
				BitConverter.GetBytes(0x0101000C).CopyTo(bs, 0);
				BitConverter.GetBytes(room).CopyTo(bs, 4);
				BitConverter.GetBytes(0).CopyTo(bs, 8);
				byte temp;
				for(int i=0;i<3;i++){
					temp = bs[0 + i * 4]; bs[0 + i * 4] = bs[3 + i * 4]; bs[3 + i * 4] = temp;
					temp = bs[1 + i * 4]; bs[1 + i * 4] = bs[2 + i * 4]; bs[2 + i * 4] = temp;
				}
				window.write("连接到"+r+"中...");
				socket.Connect("livecmt.bilibili.com", 88);
				socket.Send(bs);//
				socket.Send(re);
				if(!flag) ThreadPool.QueueUserWorkItem(getdanmu,null);
				window.write("连接成功");
			}
			catch (InvalidCastException)
			{
				window.write("无法连接到服务器");
			}
			return true;
		}
		private void getdanmu(object obj){
            /*sb so = ar.AsyncState;
			strContent = so.sb.ToString();*/
            //byte[] block = new byte[2048];
            byte[] vNum = new Byte[2];
            string s="";
			int l;
			while (socket != null && socket.Connected){
				Thread.Sleep(10);
				try{
					l=socket.Receive(buffer,0,buffer.Length,0);
					if (l < 20){
                        vNum[0] = buffer[5];
                        vNum[1] = buffer[4];
                        window.setVNum((int)BitConverter.ToUInt16(vNum, 0));
                        if (!keeping_link)ThreadPool.QueueUserWorkItem(keep_link, null);
						continue;
					};
					s=Encoding.UTF8.GetString(buffer,4,l-4);//裁掉前面无用的信息
                    //jsonReader json = new jsonReader(s);
                    /* if(json.get("cmd")== "DANMU_MSG")
                     {
                         window.danmu("1", "0");
                     }*/
                    try {
                        jsonReader js = new jsonReader(s);
                        if (js.get("cmd") == "DANMU_MSG")
                        {
                            jsonReader info = js.get_o("info");
                            window.danmu(info.get_o(2).get(1), info.get(1));
                        }
                        else if(js.get("cmd")== "SEND_GIFT")
                        {
                            jsonReader data = js.get_o("data");
                            window.write("礼物提醒", data.get("uname")+" 送了 "+data.get("num")+" 个 "+data.get("giftName"), "·");
                        }else if(js.get("cmd")== "WELCOME")
                        {
                            window.write("迎宾小姐", js.get_o("data").get("uname")," · ");
                        }
                        else
                        {
                            window.write(s);
                        }
                    }
                    catch (Exception)
                    {
                        window.write(s);
                    }
				}catch(SocketException){
					break;
				}
			}
			window.write("连接已断开");
			flag=false;
		}
        private void flash_info(string r)
        {

            string http = @"GET /" + r + @" HTTP/1.1
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
            int l = socket0.Receive(buffer, 0, buffer.Length, 0);
            string s=Encoding.UTF8.GetString(buffer, 0, l);
            //window.write(s.IndexOf("3").ToString());
            if (s.Length>8&&s.IndexOf("3") == 9)
            {
                
                flash_info(Regex.Match(s, "location: http://live.bilibili.com/(\\d+)").ToString().Substring(35));
                return;
            }
            while (l > 0)
            {
                try
                {
                    s = Regex.Match(s, "<title>.+</title>").ToString();
                    string[] list=s.Split('-');
                    window.title = list[0].Substring(7);
                    window.roomInfo = "房间号：" + r + " up主：" + list[1];
                    break;
                }catch(Exception)
                {

                }
                l = socket0.Receive(buffer, 0, buffer.Length, 0);
                s = Encoding.UTF8.GetString(buffer, 0, l);
            }
        }
        public void keep_link(Object obj){
            if (keeping_link) return;
            keeping_link = true;
            while (true)
            {
                Thread.Sleep(10000);
                try
                {
                    socket.Send(re);
                }
                catch (SocketException)
                {
                    break;
                }
            }
            keeping_link = false;
		}
		~dmReader(){//析构函数，运行结束后释放连接
			if(socket!=null){
				socket.Close();
			}
		}
		public void unlink(){
			if (socket != null)
			{
				socket.Close();
				socket=null;
			}
		}
	}
    internal class dmMessage
    {
        public string cmd="";
    }
}
