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
        Thread checker;
        public show_list()
        {
            initBackgroundBrush();
            InitializeComponent();
            y = 768 - 60;
            x = 1280 - this.Width - 10;
            this.Show();
            this.Height = 500;
            //dms.Document.Blocks.Remove(dms.Document.Blocks.FirstBlock);
            this.set_weizhi(null);
            checker = new Thread(remove_dm);
            checker.IsBackground = true;
            checker.Start();
        }
        public void adddm(int type, string name, string text)
        {
            dm_item.add(name, text, type);
            askReRender();
        }
        private void remove_dm(object o)
        {
            while (true)
            {
                Thread.Sleep(100);
                if (dm_item.cleanOutOfDate() > 0)
                {
                    this.askReRender();
                }
            }
        }
        private void askReRender()
        {
            this.Dispatcher.Invoke(
                new Action(
                        delegate
                        {
                            InvalidateVisual();
                        }
                )
            );
        }
        private void set_weizhi(object o)
        {
            this.Dispatcher.Invoke(
                    new Action(
                         delegate
                         {
                             this.Top = 0;
                             this.Left = x;
                             this.Height = y;
                             this.main.Height = y;
                             //this.mai
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
            //https://www.cnblogs.com/wangjixianyun/p/3427453.html
            Win32API.SetWindowLong(handle, -20, (IntPtr)(oldStyle | 0x00000080 | 0x08000000 | 0x80000 | 0x20));
        }
        private Pen groundPen = new Pen();
        private Brush groundBrush;
        public void initBackgroundBrush()
        {
            BrushConverter brushConverter = new BrushConverter();
            groundBrush = (Brush)brushConverter.ConvertFromString("#80000000");
        }
        protected override void OnRender(DrawingContext dc)
        {
            double baseH = y;
            foreach (dm_item item in dm_item.list)
            {
                baseH -= item.data.Height;
                Rect r = new Rect();
                r.X = 0;
                r.Y = baseH;
                r.Width = dm_item.maxWidth+10;
                r.Height = item.data.Height;
                dc.DrawRectangle(groundBrush, groundPen, r);
                Point p = new Point(5, baseH);
                dc.DrawText(item.data, p);
            }
        }
    }
}
