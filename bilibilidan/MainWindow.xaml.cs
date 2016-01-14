using System;
using System.Collections.Generic;
using System.Linq;
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

namespace bilibilidan
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private dmReader dm;
		private int room=0;
		public MainWindow()
		{
			dm=new dmReader(this);
			InitializeComponent();
			write("欢迎使用由星星制作的弹幕姬");
			if(ini.load("./ini.ini")){
				write("成功读取ini文件");
			}else{
				write("没有找到ini文件");
			}
			pluginCenter.load(this);
        }
		public void write(string text){
			write("system",text,"+ ");
		}
		public void danmu(string username,string t){
			write(username,t,username=="星星☆star"?"☆":"   ");
			pluginCenter.dm(username,t);

		}
		public void write(string username,string cont,string type){
			dms.Dispatcher.Invoke(
					new Action(
						 delegate
						 {

							 dms.AppendText(type + username + " 说 : " + cont + "\r\n");
							 if ((dms.ExtentHeight - dms.ViewportHeight - dms.VerticalOffset) <1) dms.ScrollToEnd();
						 }
				   )
			  );
		}
        public void setVNum(int nu)
        {
            vNum.Dispatcher.Invoke(
                    new Action(
                         delegate
                         {

                             vNum.Text = nu.ToString();
                         }
                   )
              );
        }
		private void keydown(object sender, KeyEventArgs e)
		{
			if(e.Key==Key.Enter){
				runCmd();
			}
		}
		private void click(object sender, RoutedEventArgs e){
			runCmd();
		}
		private void runCmd()
		{
			if(cmd.Text=="") return;
			if(cmd.Text.Substring(0,1)=="/"){
				string[] c=cmd.Text.Split(new char[]{' '},2);
				if(c[0]=="/room"){
					int roomNu=0;
					if(c.Length==2){
						try{
							roomNu=int.Parse(c[1]);
						}
						catch (InvalidCastException)
						{
						}
					}
					if(roomNu==0){
						write("参数不合法");
					}else{
						room=roomNu;
						dm.link(roomNu);
					}
				}else if(c[0]=="/top"){
					this.Topmost = true;
					write("窗口已经置顶");
				}else if(c[0]=="/untop"){
					this.Topmost = false;
					write("窗口已取消置顶");
				}else if(c[0]=="/clean"){
					this.dms.Document.Blocks.Clear();
				}else if (c[0] == "/pluginList"){
					pluginCenter.showPlugins();
				}else if (c[0] == "/help" || c[0] == "/?"){
					write(@"帮助：
/top 置顶
/untop 取消置顶
/room 房间号 连接到指定房间
/clean 清空显示框
/pluginList 展示插件列表
插件指令请参阅插件帮助文档
");
				}else{
					if(!pluginCenter.runCmd(c[0],c.Length==2?c[1]:"")) write("无效的指令"+c[0]+"\r\n输入/help 或/? 以查看帮助");
				}
			}else{
				if(ini.cookie==""||ini.UA==""){
					write("请在配置文件中添加登录信息再发送弹幕");
					return;
				}
				if(room==0){
					write("请先连接到房间");
					return;
				}
				//write("暂时不支持发弹幕啊");
				write("发送弹幕 "+cmd.Text+" 到 "+room+" 房间");
				dmSender.send(cmd.Text,room);
			}
			cmd.Text="";
		}
        public string title
        {
            set{
                _title.Text = value;
            }
        }
        public string roomInfo
        {
            set
            {
                _roomInfo.Text = value;
            }
        }
    }
}
