using bilibilidan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace bilibilidan
{
	public static class pluginCenter
	{
		private static SortedList<string,cmdFun> cmdList=new SortedList<string,cmdFun>();
		private static MainWindow main=null;
		private static List<iBilibilidanPlugin> plugins=new List<iBilibilidanPlugin>();
		internal static void load(MainWindow win){
			main=win;
			string path=Path.Combine(System.Environment.CurrentDirectory,"plugin");
			if(!Directory.Exists(path)){
				win.write("没有发现插件目录");
				return;
			}
			string[] pluginsName= Directory.GetFiles(path,"*.dll");
			Type myType=typeof(iBilibilidanPlugin);
			foreach(string pluginPath in pluginsName){
				Assembly ass=Assembly.LoadFile(pluginPath);
				Type[] type;
				try{
					type=ass.GetExportedTypes();
				}
				catch (Exception)
				{
					win.write("无法兼容模块（已抛弃，请检查插件是否过旧）"+pluginPath);
					continue;
				}
				foreach(Type cla in type){
					if(myType.IsAssignableFrom(cla)&&!cla.IsAbstract){
						iBilibilidanPlugin p=(iBilibilidanPlugin)Activator.CreateInstance(cla);
						plugins.Add(p);
					}
				}
			}
			win.write("共载入"+pluginsName.Length+"个模块，识别出"+plugins.Count+"个插件");
		}
		internal static void showPlugins(){
			if(main==null) return;
			main.write("正在展示已载入插件");
			foreach (iBilibilidanPlugin d in plugins){
				main.write(d.name);
			}
			main.write("所有已载入插件已展示完毕");
		}
		public static void write(string info,iBilibilidanPlugin p){
			if (main == null) return;
			main.write(p.name,info,6);
		}
		internal static void dm(string username, string cont,int type){
			foreach(iBilibilidanPlugin d in plugins){
				d.dm(type,username,cont);
			}
		}
        internal static void off()
        {
            foreach (iBilibilidanPlugin d in plugins)
            {
                d.off();
            }
        }
        public static void regCmd(string cmdname,cmdFun callback){
			cmdname="/"+cmdname;
			if (cmdList.IndexOfKey(cmdname)==-1){
				cmdList.Add(cmdname,callback);
				return;
			}
			main.write("插件冲突：使用共同指令"+cmdname);
		}
		internal static bool runCmd(string cmdname,string cmd){
			cmdFun cf;
			if(cmdList.TryGetValue(cmdname,out cf)){
				cf(cmd);
				return true;
			}
			return false;
		}
	}

	//
	public delegate void cmdFun(string cmd);
	public interface iBilibilidanPlugin{
		string name{get;}
        int[] dmtype { get; }
		void dm(int type,string username,string cont);
        void off();
	}
}