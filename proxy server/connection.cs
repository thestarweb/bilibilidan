using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace proxy_server
{
	public class connection
	{
		private MainWindow win;
		private Socket socket;
		private Byte[] buff= new Byte[2048];
		private string _ip;
		public string ip { get { return _ip; } }
		private int _port;
		public int port { get { return _port; } }
		public connection(Socket s,MainWindow w) {
			socket = s;
			win = w;
			_ip = ((IPEndPoint)s.RemoteEndPoint).Address.ToString();
			_port = ((IPEndPoint)s.RemoteEndPoint).Port;
			s.BeginReceive(buff, 0, buff.Length, 0, new AsyncCallback(onread), s);
		}
		public void onread(IAsyncResult ar) {
			int l= socket.EndReceive(ar);
			if (l < 1) {
				win.onclose(this);
				socket.Close();
				return;
			}
            win.write(ip + ":" + port+"："+Encoding.UTF8.GetString(buff, 0, l));
			socket.BeginReceive(buff, 0, buff.Length, 0, new AsyncCallback(onread), socket);
		}
	}
}
