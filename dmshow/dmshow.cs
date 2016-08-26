using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bilibilidan;
namespace dmshow
{
    public class dmshow : iBilibilidanPlugin
    {
        show_list list = new show_list();
        public dmshow()
        {
            list.Show();
        }
        private int[] _dmtype = { 0, 1, 2, 3 };
        public int[] dmtype
        {
            get
            {
                return _dmtype;
            }
        }

        public string name
        {
            get
            {
                return "弹幕侧边栏";
            }
        }

        public void dm(int type, string username, string cont)
        {
            list.adddm(type, username, cont);
        }

        public void off()
        {
            //
        }
    }
}
