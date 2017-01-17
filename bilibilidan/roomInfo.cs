﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace bilibilidan
{
    class roomInfo
    {
        private string _roomTitle;
        public string roomTitle {
            get{
                return _roomTitle;
            } }
        private string _roomNum;
        public string roomNum
        {
            get
            {
                return _roomNum;
            }
        }
        private string _uper;
        public string uper
        {
            get
            {
                return _uper;
            }
        }
        public roomInfo(string room)
        {
            _roomNum=_roomInfo(room);
        }
        public string _roomInfo(string room)
        {

            string http = @"GET /" + room + @" HTTP/1.1
Host: live.bilibili.com
User-Agent: star_danmuji
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
Connection: keep-alive

";
            Socket socket0 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket0.Connect("live.bilibili.com", 80);
            byte[] bs = Encoding.ASCII.GetBytes(http);//
            socket0.Send(bs);
            Byte[] buffer=new byte[4096];
            int l = socket0.Receive(buffer, 0, buffer.Length, 0);
            string s = Encoding.UTF8.GetString(buffer, 0, l);
            //window.write(s.IndexOf("3").ToString());
            if (s.Length > 8 && s.IndexOf("3") == 9)
            {

                return _roomInfo(Regex.Match(s, "location: /(\\d+)").ToString().Substring(11));
            }
            while (l > 0)
            {
                try
                {
                    string t = Regex.Match(s, "<title>.+</title>").ToString();
                    string ts = t.Substring(7, t.Length - 7);//.Split('-');
                    ts = ts.Substring(0, ts.LastIndexOf('-')-1);
                    _roomTitle = ts.Substring(0,ts.LastIndexOf('-')-1);
                    _uper = ts.Substring(ts.LastIndexOf('-')+2);
                    s = Regex.Match(s, "var ROOMID = \\d+;").ToString();
                    string[] list = s.Split('=');
                    string roomNu = list[1].Substring(1, list[1].Length - 2);
                    MainWindow.main.write("已识别播放地址：http://live.bilibili.com/" +room);
                    MainWindow.main.write("已识别真实房间号：" +roomNu);
                    return roomNu;
                }
                catch (Exception)
                {

                }
                l = socket0.Receive(buffer, 0, buffer.Length, 0);
                s = Encoding.UTF8.GetString(buffer, 0, l);
            }
            return "";
        }
    }
}
