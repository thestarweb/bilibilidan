using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bilibilidan.json
{
    class jsonReader
    {
        private SortedList<string, string> infos = new SortedList<string, string>();
        public jsonReader(string jsonString)
        {
            if (jsonString.Length > 2)
            {
                if(jsonString.Substring(0, 1) == "{")
                {
                    isObj(jsonString);
                }
                else
                {
                    isArr(jsonString);
                }
            }
        }
        public void isObj(string jsonString)
        {
            Stack<string> indexs = new Stack<string>();
            string temp;
            indexs.Push("{");
            int flag = 0;
            StringBuilder name = new StringBuilder(), value = new StringBuilder();
            for (int i = 1; i < jsonString.Length; i++)
            {
                temp = jsonString.Substring(i, 1);
                if (indexs.Peek() == "\\")
                {
                    indexs.Pop();
                }
                else if (temp == "\\")
                {
                    indexs.Push("\\");
                    continue;
                }
                else if (indexs.Peek() == "\"" || indexs.Peek() == "'")
                {
                    if (temp == indexs.Peek())
                    {
                        indexs.Pop();
                        continue;
                    }
                }
                else if (temp == "\"" || temp == "'")
                {
                    indexs.Push(temp);
                    continue;
                }
                else if (temp == "[")
                {
                    indexs.Push(temp);
                }
                else if (temp == "]" && indexs.Peek() == "[")
                {
                    indexs.Pop();
                }
                else if (temp == ":"&&flag==0)
                {
                    flag=2;
                    continue;
                }
                else if (temp == "," && indexs.Count == 1)
                {
                    flag = 3;
                }
                else if (temp == "{")
                {
                    indexs.Push(temp);
                }
                else if (temp == "}")
                {
                    if (indexs.Peek() == "{")
                    {
                        indexs.Pop();
                    }
                }

                //读取结束
                if (indexs.Count == 0)
                {
                    infos.Add(name.ToString(), value.ToString());
                    name.Clear(); value.Clear();
                    break;
                }

                //内容拼接
                if (flag==2)
                {
                    value.Append(temp);
                }
                else if(flag==0)
                {
                    name.Append(temp);
                }
                else if (flag == 3)
                {
                    infos.Add(name.ToString(), value.ToString());
                    name.Clear();value.Clear();
                    flag = 0;
                }
            }
        }
        public void isArr(string jsonString)
        {
            Stack<string> indexs = new Stack<string>();
            string temp;
            indexs.Push("[");
            bool flag = false;
            StringBuilder value = new StringBuilder();
            int arrayIndex = 0;
            for (int i = 1; i < jsonString.Length; i++)
            {
                temp = jsonString.Substring(i, 1);
                if (indexs.Peek() == "\\")
                {
                    indexs.Pop();
                }
                else if (temp == "\\")
                {
                    indexs.Push("\\");
                }
                else if (indexs.Peek() == "\"" || indexs.Peek() == "'")
                {
                    if (temp == indexs.Peek())
                    {
                        indexs.Pop();
                        continue;
                    }
                }
                else if (temp == "\"" || temp == "'")
                {
                    indexs.Push(temp);
                    continue;
                }
                else if (temp == "[")
                {
                    indexs.Push(temp);
                }
                else if (temp == "]" && indexs.Peek() == "[")
                {
                    indexs.Pop();
                }
                else if (temp == "," && indexs.Count == 1)
                {
                    flag = true;
                }
                else if (temp == "[")
                {
                    indexs.Push(temp);
                }
                else if (temp == "]")
                {
                    if (indexs.Peek() == "[")
                    {
                        indexs.Pop();
                    }
                }

                //读取结束
                if (indexs.Count == 0)
                {
                    infos.Add(arrayIndex.ToString(), value.ToString());
                    value.Clear();
                    break;
                }

                //内容拼接
                if (flag)
                {
                    infos.Add(arrayIndex++.ToString(), value.ToString());
                    value.Clear();
                    flag = false;
                }
                else
                {
                    value.Append(temp);
                }
            }
        }
        public string get(string name)
        {
            return infos.IndexOfKey(name)!=-1?infos[name].ToString():"";
        }
        public jsonReader get_o(string name)
        {
            return infos.IndexOfKey(name) != -1 ? new jsonReader(infos[name].ToString()) : null; 
        }
        public string get(int name)
        {
            return infos.IndexOfKey(name.ToString()) != -1 ? infos[name.ToString()].ToString() : "";
        }
        public jsonReader get_o(int name)
        {
            return infos.IndexOfKey(name.ToString()) != -1 ? new jsonReader(infos[name.ToString()].ToString()) : null;
        }
    }
}
