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
            pluginCenter.regCmd("dmshow", cmd);
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
            list.Close();
        }
        public void cmd(string _cmd)
        {
            string[] cmds = _cmd.Split(' ');
            if (cmds.Length == 4)
            {
                if (cmds[0] == "pos")
                {
                    try
                    {
                        if (cmds[1] == "add")
                        {
                            if (cmds[2] == "x")
                            {
                                list.x += int.Parse(cmds[3]);
                            }
                            else if (cmds[2] == "y")
                            {
                                list.y += int.Parse(cmds[3]);
                            }
                        }
                        else if (cmds[1] == "set")
                        {
                            if (cmds[2] == "x")
                            {
                                list.x = int.Parse(cmds[3]);
                            }
                            else if (cmds[2] == "y")
                            {
                                list.y = int.Parse(cmds[3]);
                            }
                        }
                        pluginCenter.write("窗口位置已设置x=" + list.x + "y=" + list.y,this);
                    }
                    catch (Exception)
                    {
                        pluginCenter.write("最后一个参数必须为整数", this);
                    }
                }
            }
        }
    }
}
