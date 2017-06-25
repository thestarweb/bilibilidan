using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace proxy_server
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool isstart = false;
		private Socket _socket = null;
		public MainWindow()
		{
			InitializeComponent();
		}
		public void write(string info) {
			log.Dispatcher.Invoke(
					new Action(
						 delegate
						 {

							 log.AppendText(info+"\n");
						 }
				   )
			  );
		}

		private void start(object sender, RoutedEventArgs e)
		{
			if (isstart)
			{
				//
			}
			try
			{
				int p = int.Parse(port.Text);
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				_socket.Bind(new IPEndPoint(IPAddress.Any, p));
				_socket.Listen(16);
				_socket.BeginAccept(new AsyncCallback(onconnect), _socket);
			}
			catch (FormatException)
			{
				log.AppendText("端口号必须是整数\n");
				return;
			}
			log.AppendText("开始监听\n");
			isstart = true;
		}
		public void onconnect(IAsyncResult ar)
		{
			if(ar==null) {
				return;
			}
			Socket s=_socket.EndAccept(ar);
			s.Send(Encoding.ASCII.GetBytes("hello"));
			connection c = new connection(s,this);
			write("新连接:"+c.ip+":"+ c.port);
			_socket.BeginAccept(new AsyncCallback(onconnect), _socket);

		}
		public void onclose(connection c) {
			write("连接断开" + c.ip + ":" + c.port);
		}
	}
}
