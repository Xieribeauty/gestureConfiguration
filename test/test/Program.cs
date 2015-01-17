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
        static private int[] _stableX = new int[50];
        static private int[] _stableY = new int[50];
        static private int[] _stableZ = new int[50];
        static private int[] _stableAxis = new int[3];

        static private int _threshold = 5400;

        static private int _timeOutClick = 167;
        static private int _recognizeClick = 85;
        static private int _timeOutDoubleClick = 252;

        //main function
        static void Main(String[] args)
        {
            String filePath = "../../../testResource/5_100/index.txt";

            // 创建一个 StreamReader 的实例来读取文件 
            // using 语句也能关闭 StreamReader
            using (StreamReader sr = new StreamReader(filePath))
            {
                String line;

                //创建稳定数组
                for (int i = 0; i < 50 && (line = sr.ReadLine()) != null; i++){
                    char[] splitChar = {'\t'};
                    String[] s = line.Split(splitChar);
                    int[] data = new int[3];

                    data[0] = Convert.ToInt32(s[0]);
                    data[1] = Convert.ToInt32(s[1]);
                    data[2] = Convert.ToInt32(s[2]);

                    _stableX[i] = data[0];
                    _stableY[i] = data[1];
                    _stableZ[i] = data[2];
                    _stableAxis[0] += data[0];
                    _stableAxis[1] += data[1];
                    _stableAxis[2] += data[2];
                }

                //默认输入超过50组，赋值稳定数组均值
                _stableAxis[0] = Convert.ToInt32(_stableAxis[0]/50);
                _stableAxis[1] = Convert.ToInt32(_stableAxis[1]/50);
                _stableAxis[2] = Convert.ToInt32(_stableAxis[2]/50);

                //stable循环队列游标
                int stableCursorX = 0;
                int stableCursorY = 0;
                int stableCursorZ = 0;

                // 继续读取直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    char[] splitChar = {'\t'};
                    String[] s = line.Split(splitChar);
                    int[] data = new int[3];

                    data[0] = Convert.ToInt32(s[0]-_stableAxis[0]);
                    data[1] = Convert.ToInt32(s[1]-_stableAxis[1]);
                    data[2] = Convert.ToInt32(s[2]-_stableAxis[2]);

                    if (data[2]>-_threshold && data[2]<_threshold) {
                        //是平衡状态
                        _stableAxis[2] = _stableAxis + Convert.ToInt32(
                            data[2] - (_stableZ[stableCursorZ])/50
                            );
                        _stableZ[stableCursorZ] = data[2];
                        stableCursorZ = (stableCursorZ + 1)%50 - 1;
                    }
                    else {
                        //运动状态
                        
                    }
                }
            }

            Console.ReadKey();

            return ;
        }
    }
}