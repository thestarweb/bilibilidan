using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dmshow
{
    /// <summary>
    /// show_list.xaml 的交互逻辑
    /// </summary>
    public partial class show_list : Window
    {
        public double x;
        public double y;
        public show_list()
        {
            InitializeComponent();
            y = 768 - 60;
            x = 1280 - this.Width - 10;
            this.Show();
            dms.Document.Blocks.Remove(dms.Document.Blocks.FirstBlock);
        }
        public void adddm(int type, string name, string text)
        {
            //dms.CaretBrush= Brush.
            //dms.Document.Blocks.Remove(dms.Document.Blocks.FirstBlock);

            dms.Dispatcher.Invoke(
                    new Action(
                         delegate
                         {
                             var p = new Paragraph();
                             var _name = new Run(name);
                             switch (type)
                             {
                                 case 0:
                                     _name.Foreground = Brushes.SkyBlue;
                                     break;
                                 case 1:
                                     _name.Foreground = Brushes.Red;
                                     break;
                                 case 2:
                                     _name.Foreground = Brushes.Green;
                                     break;
                                 case 3:
                                     _name.Foreground = Brushes.DarkRed;
                                     break;
                                 case 11:
                                     _name.Foreground = Brushes.LightBlue;
                                     break;
                                 case 12:
                                     _name.Foreground = Brushes.Orange;
                                     break;
                                 case 13:
                                     _name.Foreground = Brushes.GreenYellow;
                                     break;
                                 default:
                                     _name.Foreground = Brushes.White;
                                     break;

                             }
                             var _text = new Run(" : " + text);
                             _text.Foreground = Brushes.White;
                             p.Inlines.Add(_name);
                             p.Inlines.Add(_text);
                             dms.Document.Blocks.Add(p);
                             ThreadPool.QueueUserWorkItem(set_weizhi);
                             ThreadPool.QueueUserWorkItem(remove_dm, p);
                         }
                   )
              );
        }
        private void remove_dm(object o)
        {
            Thread.Sleep(9000);

            dms.Dispatcher.Invoke(
                    new Action(
                         delegate
                         {
                             dms.Document.Blocks.Remove(dms.Document.Blocks.FirstBlock);
                             ThreadPool.QueueUserWorkItem(set_weizhi);
                         }
                   )
              );
        }
        private void set_weizhi(object o)
        {
            Thread.Sleep(1);
            dms.Dispatcher.Invoke(
                    new Action(
                         delegate
                         {
                             this.Top = y - this.Height;
                             this.Left = x;
                         }
                   )
              );
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            int oldStyle = (int)(Win32API.GetWindowLong(handle, -20));
            //-20 GWL_EXSTYLE 窗口样式
            //0x00000080=WS_EX_TOOLWINDOW 小标题工具窗口 在Alt+tab中不会显示此窗口
            //0x08000000=WS_EX_NOACTIVATE 窗口不会变成前台
            //0x80000=WS_EX_LAYERED 窗口具有透眀属性 大概是没用
            //0x20=WS_EX_TRANSPARENT 窗口透明 这里其实是指的窗口不会捕获鼠标事件 鼠标可以穿过窗口点击到后面的东西
            Win32API.SetWindowLong(handle, -20, (IntPtr)(oldStyle | 0x00000080 | 0x08000000| 0x80000| 0x20));
        }

    }
}
