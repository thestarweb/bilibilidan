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
		private byte[] buffer = new byte[4096];
		private bool flag=false;
        private bool keeping_link=false;
        private Thread reader = null;//弹幕读取线程
        private Thread keeper = null;//维持连接并发送获取人数指令线程
		public dmReader(MainWindow win)
		{
            window = win;
		}
		public bool link(int room){
			if (socket != null) unlink();//断开之前的连接
			socket=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个socket
			string r =room.ToString();
			try{
				window.write("连接到"+r+"中...");
                byte[] temp = Encoding.ASCII.GetBytes("{\"roomid\":"+r+ ",\"uid\":201510566613409}");//构造房间信息
                socket.Connect("livecmt-2.bilibili.com", 788);//连接到弹幕服务器
                //构造消息头
                byte[] head = { 0x00, 0x00, 0x00, (byte)(0x31+r.Length), 0x00, 0x10, 0x00, 0x01, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x01 };
                socket.Send(head);//发送消息头
				socket.Send(temp);//发送请求正文——房间信息
				socket.Send(re);//这个本来应该是用来获取人数的，但是长期不发送服务器会断开连接，这里是为了防止这样的情况发生
                if (!flag)//检查读弹幕线程是否在工作
                {
                    reader = new Thread(getdanmu);
                    reader.IsBackground = true;
                    reader.Start();
                }
				check_keeper();
				window.write("连接成功");
			}
			catch (InvalidCastException)
			{
				window.write("连接失败");
			}
			return true;
		}
		private void getdanmu(object obj){
            byte[] vNum = new Byte[2];
            string s="";
			int l;
            //ThreadPool.QueueUserWorkItem(keep_link, null);//此线程定时发送获取人数的信息
            while (socket != null && socket.Connected){
				try{
					l=socket.Receive(buffer,0,buffer.Length,0);
                    if (l == 0) continue;
					if (l < 22){
                        //这种情况一般是返回房间人数 是二进制信息
                        vNum[0] = buffer[l-1];
                        vNum[1] = buffer[l-2];
                        window.setVNum((int)BitConverter.ToUInt16(vNum, 0));
						check_keeper();
						continue;
					};
					s=Encoding.UTF8.GetString(buffer,0,l);//取出信息
                    try {
                        jsonReader js = new jsonReader(s.Substring(s.IndexOf("{")));
                        if (ini.debug) window.write(s);
                        if (js.get("cmd").StartsWith("DANMU_MSG"))//弹幕消息
                        {
                            string color = (new jsonReader(js.get("info"))).get_o(0).get(3);
                            switch (color)
                            {
                                case "16777215":
                                    color = "[白]";
                                    break;
                                case "65532":
                                    color = "[青]";
                                    break;
                                case "16738408":
                                    color = "[红]";
                                    break;
                                case "6737151":
                                    color = "[蓝]";
                                    break;
                                case "14893055":
                                    color = "[紫]";
                                    break;
                                case "8322816":
                                    color = "[绿]";
                                    break;
                                case "16772431":
                                    color = "[黄]";
                                    break;
                                case "16746752":
                                    color = "[橙]";
                                    break;
                                default:
                                    color="["+color+"]";
                                    break;
                            }
                            jsonReader info = js.get_o("info");
                            jsonReader user = info.get_o(2);
                            string username = user.get(1);
                            int u_type = 13;
                            if (username == "星星☆star") u_type = 0;
                            else if (username == window.upname) u_type = 11;
                            else if (user.get(2) == "1") u_type = 12;
                            window.write(username, color+info.get(1),u_type);
                        }
                        else if(js.get("cmd")== "SEND_GIFT")//礼物信息
                        {
                            jsonReader data = js.get_o("data");
                            window.write("礼物提醒", data.get("uname")+" 送了 "+data.get("num")+" 个 "+data.get("giftName"), 2);
                        }else if(js.get("cmd")== "WELCOME")//老爷进入房间
                        {
                            window.write("迎宾小姐", "欢迎"+js.get_o("data").get("uname")+"老爷",2);
                        }
                        else
                        {
                            if(ini.debug)window.write(s);
                        }
                    }
                    catch (Exception)
                    {
                        if(ini.debug)window.write(s);
                    }
				}catch(SocketException){
                    keeper.Abort();
					break;
				}
			}
			window.write("连接已断开");
			flag=false;
		}
        
        public void keep_link(Object obj){
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
		}
		private void  check_keeper() {
			if(keeper==null||!keeper.IsAlive) {
				keeper = new Thread(keep_link);
				keeper.IsBackground = true;
				keeper.Start();
			}
		}
		~dmReader(){//析构函数，运行结束后释放连接
            unlink();
        }
		public void unlink(){
			if (socket != null)
			{
				socket.Close();
				socket=null;
			}
            if (reader!=null&&reader.IsAlive) reader.Abort();
            if (keeper!=null&&keeper.IsAlive) keeper.Abort();
		}
	}
    internal class dmMessage
    {
        public string cmd="";
    }
}
