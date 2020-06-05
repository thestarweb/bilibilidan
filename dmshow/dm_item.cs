using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace dmshow
{
    class dm_item
    {
        private static int  _maxWidth=200;
        public static int maxWidth
        {
            get
            {
                return _maxWidth;
            }
            set
            {
                _maxWidth = value;
            }
        }
        private static int _size = 12;
        public static int size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }
        public static LinkedList<dm_item> list = new LinkedList<dm_item>();
        public static void add(String sender, String data, int type)
        {
            list.AddFirst(new dm_item(sender, data, type));
            cleanOutOfDate();
        }
        public static TimeSpan ts = new TimeSpan(0, 0, -10);
        public static int cleanOutOfDate()
        {
            DateTime outTime = DateTime.Now.Add(ts);
            int num = 0;
            while (list.Count>0&&DateTime.Compare(list.Last.Value.time, outTime) < 0)
            {
                list.RemoveLast();
                num++;
            }
            return num;
        }
        private FormattedText _data;
        public FormattedText data
        {
            get
            {
                return _data;
            }
        }
        private DateTime time;
        private dm_item(String sender,String data,int type)
        {
            time = DateTime.Now;
            _data =new FormattedText(sender+": "+data, new System.Globalization.CultureInfo("zh-CN", false), FlowDirection.LeftToRight, new Typeface("微软雅黑"),_size, Brushes.White);
            _data.MaxTextWidth = maxWidth;
            Brush b;
            switch (type)
            {
                case 0:
                    b = Brushes.SkyBlue;
                    break;
                case 1:
                    b = Brushes.Red;
                    break;
                case 2:
                    b = Brushes.Green;
                    break;
                case 3:
                    b = Brushes.DarkRed;
                    break;
                case 11:
                    b = Brushes.LightBlue;
                    break;
                case 12:
                    b = Brushes.Orange;
                    break;
                case 13:
                    b = Brushes.GreenYellow;
                    break;
                default:
                    b = Brushes.White;
                    break;

            }
            _data.SetForegroundBrush(b, 0, sender.Length);
        }
    }
}
