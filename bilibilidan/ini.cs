using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace bilibilidan
{
	static class ini
	{
		private static string _UA="";
		public static string UA{
			get{
				return _UA;
			}
		}
		public static int roomNu=0;
		private static string _cookie = "";
		public static string cookie{
			get{
				return _cookie;
			}
		}
        private static bool _debug = false;
        public static bool debug
        {
            get
            {
                return _debug;
            }
        }
		public static bool load(string file)
		{
			if(File.Exists(file)){
				//FileStream fp=File.Open(file,FileMode.Open);
				//fp.r
				IEnumerable<string> lines=File.ReadLines(file);
				int i=0;
				foreach(string line in lines){
					switch (i){
						case 0:
							_UA=line;
							break;
						case 1:
							_cookie=line;
							break;
						case 3:
							return true;
					}
					i++;
				}
				return true;
			}
			return false;
		}
	}
}
