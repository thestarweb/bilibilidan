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
		private byte[] re= { 0x00, 0x00, 0x00, 0x10, 0x00, 0x10 ,0x00 ,0x01 ,0x00 ,0x00 ,0x00 ,0x02 ,0x00 ,0x00 ,0x00 ,0x01 };//00 00 00 10 00 10 00 01 00 00 00 02 00 00 00 01 
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
                //socket0.Receive(buffer, 0, buffer.Length, 0);
                //socket0.Receive(buffer, 0, buffer.Length, 0);
                byte[] temp = Encoding.ASCII.GetBytes("{\"roomid\":"+r+",\"uid\":116364117067476}");
				window.write("连接到"+r+"中...");
				socket.Connect("58.220.29.21", 788);
                byte[] temp2 = { 0x00, 0x00, 0x00, (byte)(0x31 +r.Length), 0x00, 0x10, 0x00, 0x01, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x01 };
                socket.Send(temp2);//
				socket.Send(temp);
				socket.Send(re);
                if (!flag) ThreadPool.QueueUserWorkItem(getdanmu,null);
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
			int l; ThreadPool.QueueUserWorkItem(keep_link, null);
            while (socket != null && socket.Connected){
				Thread.Sleep(10);
				try{
					l=socket.Receive(buffer,0,buffer.Length,0);
                    if (l == 0) continue;
					if (l < 22){
                        vNum[0] = buffer[l-1];
                        vNum[1] = buffer[l-2];
                        window.setVNum((int)BitConverter.ToUInt16(vNum, 0));
                        if (!keeping_link)ThreadPool.QueueUserWorkItem(keep_link, null);
						continue;
					};
					s=Encoding.UTF8.GetString(buffer,0,l);//裁掉前面无用的信息
                    //jsonReader json = new jsonReader(s);
                    /* if(json.get("cmd")== "DANMU_MSG")
                     {
                         window.danmu("1", "0");
                     }*/
                    try {
                        jsonReader js = new jsonReader(s.Substring(s.IndexOf("{")));
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
