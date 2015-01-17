using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            String _file = "../../../testResource/5_100/index.txt";
            FileStream F = new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.Read);
            F.Position = 0;

            try
            {
                // 创建一个 StreamReader 的实例来读取文件 
                // using 语句也能关闭 StreamReader
                using (StreamReader sr = new StreamReader(_file))
                {
                    string line;
                   
                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        //output line of data
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                // 向用户显示出错消息
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();

            return ;
        }
    }
}