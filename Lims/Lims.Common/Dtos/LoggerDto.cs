﻿using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Lims.Common.Dtos
{
    public class LoggerDto : BaseDto
    {
        public LoggerDto()
        {
            
        }

        public LoggerDto(DateTimeOffset createTime)
        {
            CreateTime = createTime;
        }

        public int Id { get; set; }              
        public DateTimeOffset CreateTime { get; set; }
       

        public LogLevel LogLevel { get; set; }

        public ActionType ActionType { get; set; }

        public string? PublisherIP { get; set; }
        public string? PublisherName { get; set; }

        public string? ReceiverName { get; set; }

        public string? SampleCode
        {
            get; set;
        }

        public string? TestItem { get; set; }

        public string? Message { get; set; }

        private bool isReaded = false;

        public bool IsReaded
        {
            get { return isReaded; }
            set { isReaded = value; RaisePropertiesChanged(nameof(IsReaded)); }
        }

        public static string GetLocalIP()
        {
            string result = RunApp("route", "print", true);
            Match m = Regex.Match(result, @"0.0.0.0\s+0.0.0.0\s+(\d+.\d+.\d+.\d+)\s+(\d+.\d+.\d+.\d+)");
            if (m.Success)
            {
                return m.Groups[2].Value;
            }
            else
            {
                try
                {
                    System.Net.Sockets.TcpClient c = new System.Net.Sockets.TcpClient();
                    c.Connect("www.baidu.com", 80);
                    string ip = (c.Client.LocalEndPoint as System.Net.IPEndPoint).Address.ToString();
                    c.Close();
                    return ip;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }/// <summary>

         /// 运行一个控制台程序并返回其输出参数。
         /// </summary>
         /// <param name="filename">程序名</param>
         /// <param name="arguments">输入参数</param>
         /// <returns></returns>
        private static string RunApp(string filename, string arguments, bool recordLog)
        {
            try
            {
                if (recordLog)
                {
                    Trace.WriteLine(filename + " " + arguments);
                }
                Process proc = new Process();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                using (System.IO.StreamReader sr = new System.IO.StreamReader(proc.StandardOutput.BaseStream, Encoding.Default))
                {
                    //string txt = sr.ReadToEnd();
                    //sr.Close();
                    //if (recordLog)
                    //{
                    // Trace.WriteLine(txt);
                    //}
                    //if (!proc.HasExited)
                    //{
                    // proc.Kill();
                    //}
                    //上面标记的是原文，下面是我自己调试错误后自行修改的
                    Thread.Sleep(100);  //貌似调用系统的nslookup还未返回数据或者数据未编码完成，程序就已经跳过直接执行
                                        //txt = sr.ReadToEnd()了，导致返回的数据为空，故睡眠令硬件反应
                    if (!proc.HasExited)  //在无参数调用nslookup后，可以继续输入命令继续操作，如果进程未停止就直接执行
                    {    //txt = sr.ReadToEnd()程序就在等待输入，而且又无法输入，直接掐住无法继续运行
                        proc.Kill();
                    }
                    string txt = sr.ReadToEnd();
                    sr.Close();
                    if (recordLog)
                        Trace.WriteLine(txt);
                    return txt;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return ex.Message;
            }
        }
    }

    public enum LogLevel
    {
        OFF = 0,
        FATAL = 1,
        ERROR = 2,
        WARN = 3,
        INFO = 4,
        DEBUG = 5,
        ALL = 6,
    }

    public enum ActionType
    {
        删除任务 = 0,
        变更项目信息 = 1,
        退回任务 = 2,
        变更项目分析人 = 3,
        变更样品信息 = 4,
    }
}
