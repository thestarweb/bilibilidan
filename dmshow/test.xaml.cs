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
using System.Windows.Shapes;

namespace bilibilidan
{
    /// <summary>
    /// test.xaml 的交互逻辑
    /// </summary>
    public partial class test : Window
    {
        public test()
        {
            InitializeComponent();
            this.Show();
            dms.Document.Blocks.Remove(dms.Document.Blocks.FirstBlock);
            adddm(3, "hello", "233");
            adddm(1, "star", "233");
        }
        public void adddm(int type,string name,string text)
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
                                     _name.Foreground = Brushes.Red;
                                     break;
                                 case 1:
                                     _name.Foreground = Brushes.SkyBlue;
                                     break;
                                 case 2:
                                     _name.Foreground = Brushes.Orange;
                                     break;
                                 case 3:
                                     _name.Foreground = Brushes.GreenYellow;
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
            Thread.Sleep(5000);

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
                             this.Top = 768 - this.Height - 60;
                             this.Left = 1280 - this.Width - 10;
                         }
                   )
              );
        }
    }
}
