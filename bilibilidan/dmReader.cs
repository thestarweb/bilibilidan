﻿using System;
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
		}
		public bool link(int room){
			if (socket != null) unlink();//断开之前的连接
			socket=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个socket
			string r =room.ToString();
			try{
				window.write("连接到"+r+"中...");
                byte[] temp = Encoding.ASCII.GetBytes("{\"roomid\":"+r+",\"uid\":116364117067476}");//构造房间信息
                socket.Connect("livecmt-2.bilibili.com", 788);//连接到弹幕服务器
                //构造消息头
                byte[] head = { 0x00, 0x00, 0x00, (byte)(0x31 +r.Length), 0x00, 0x10, 0x00, 0x01, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x01 };
                socket.Send(head);//发送消息头
				socket.Send(temp);//发送请求正文——房间信息
				socket.Send(re);//这个本来应该是用来获取人数的，但是长期不发送服务器会断开连接，这里是为了防止这样的情况发生
                if (!flag) ThreadPool.QueueUserWorkItem(getdanmu,null);//检查读弹幕线程是否在工作
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
            ThreadPool.QueueUserWorkItem(keep_link, null);//此线程定时发送获取人数的信息
            while (socket != null && socket.Connected){
				//Thread.Sleep(10);
				try{
					l=socket.Receive(buffer,0,buffer.Length,0);
                    if (l == 0) continue;
					if (l < 22){
                        //这种情况一般是返回房间人数 是二进制信息
                        vNum[0] = buffer[l-1];
                        vNum[1] = buffer[l-2];
                        window.setVNum((int)BitConverter.ToUInt16(vNum, 0));
                        if (!keeping_link)ThreadPool.QueueUserWorkItem(keep_link, null);
						continue;
					};
					s=Encoding.UTF8.GetString(buffer,0,l);//取出信息
                    try {
                        jsonReader js = new jsonReader(s.Substring(s.IndexOf("{")));
                        if (js.get("cmd") == "DANMU_MSG")//弹幕消息
                        {
                            jsonReader info = js.get_o("info");
                            window.danmu(info.get_o(2).get(1), info.get(1));
                        }
                        else if(js.get("cmd")== "SEND_GIFT")//礼物信息
                        {
                            jsonReader data = js.get_o("data");
                            window.write("礼物提醒", data.get("uname")+" 送了 "+data.get("num")+" 个 "+data.get("giftName"), "·");
                        }else if(js.get("cmd")== "WELCOME")//老爷进入房间
                        {
                            window.write("迎宾小姐", "欢迎"+js.get_o("data").get("uname")+"老爷"," · ");
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
