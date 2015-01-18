//#define MOTION 0;
//#define STABLE 1;
//<< 未确定define使用方法

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
        //temp parameter
        static private int _temp_clickTime = 0;
        static private int _temp_totalGroup = 0;

        static private int[] _stableX = new int[50];
        static private int[] _stableY = new int[50];
        static private int[] _stableZ = new int[50];
        static private int[] _stableAxis = new int[3];

        //stable循环队列游标
        static private int _stableCursorX = 0;
        static private int _stableCursorY = 0;
        static private int _stableCursorZ = 0;

        static private int _threshold = 5300;

        static private bool[] _clickIndex = {true, false};
        static private int _clickValueTime = 0;
        static private int _clickRecognizeGroup = 85;
        static private int _clickGroup = 167;

        static private int _dClickRecognizeGroup = 170;
        static private int _dClickGroup = 252;

        static private int _status = 0;
        static private int _groupNum = 0;

        static private void setStableArray(StreamReader sr, int n) {
            String line;
            //创建稳定数组
            for (int i = 0; i < n && (line = sr.ReadLine()) != null; i++){
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
            _stableAxis[0] = _stableAxis[0]/n;
            _stableAxis[1] = _stableAxis[1]/n;
            _stableAxis[2] = _stableAxis[2]/n;

            for (; n < 50; n++){
                _stableX[n] = _stableAxis[0];
                _stableY[n] = _stableAxis[1];
                _stableZ[n] = _stableAxis[2];
            }

            _stableCursorX = n-1;
            _stableCursorY = n-1;
            _stableCursorZ = n-1;

            return;
        }

        //main function
        static void Main(String[] args)
        {
            String filePath = "../../../testResource/5_100/index.txt";

            // 创建一个 StreamReader 的实例来读取文件 
            // using 语句也能关闭 StreamReader
            using (StreamReader sr = new StreamReader(filePath))
            {
                String line;

                setStableArray(sr, 50);

                _temp_totalGroup = 50;


                // 继续读取直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    char[] splitChar = {'\t'};
                    String[] s = line.Split(splitChar);
                    int[] data = new int[3];

                    data[0] = Convert.ToInt32(s[0])-_stableAxis[0];
                    data[1] = Convert.ToInt32(s[1])-_stableAxis[1];
                    data[2] = Convert.ToInt32(s[2])-_stableAxis[2];

                    _temp_totalGroup++;

                    if (_status == 0) {
                        if (data[2]>-_threshold && data[2]<_threshold) {
                            //是平衡状态
                            _stableAxis[2] = _stableAxis[2] + (
                                Convert.ToInt32(s[2]) - _stableZ[_stableCursorZ]
                                )/50;
                            _stableZ[_stableCursorZ] = Convert.ToInt32(s[2]);
                            _stableCursorZ = (_stableCursorZ + 1)%50;
                        }
                        else {
                            //运动状态初始
                            bool temp0 = data[2] < -_threshold;

                            if (temp0) {
                                _status = 1;
                                _clickValueTime++;
                                _groupNum = 1;
                                //修改单击和双击的标志为活跃
                            }
                            else {
                                //非单双击动作
                            }
                        }
                    }
                    else {
                        //运动状态
                        _groupNum++;
                        
                        //暂时缺少判断之前运动状态是否为单击
                        if (_clickValueTime < _clickIndex.Length) {
                            if (_clickIndex[_clickValueTime]){
                                if (data[2] > _threshold) {
                                    _clickValueTime++;
                                    if (_clickValueTime == _clickIndex.Length){
                                        _temp_clickTime++;
                                        Console.WriteLine("click recognized by index data\n-----{0},{1}"
                                            , _temp_clickTime
                                            , _temp_totalGroup
                                            );
                                    }
                                }
                            }
                            else {
                                if (data[2] < -_threshold){
                                    _clickValueTime++;
                                    if (_clickValueTime == _clickIndex.Length){
                                        _temp_clickTime++;
                                        Console.WriteLine("click recognized by index data\n-----{0},{1}"
                                            , _temp_clickTime
                                            , _temp_totalGroup
                                            );
                                    }
                                }
                            }
                        }
                        else {
                            if (_groupNum > _clickGroup){
                                _status = 0;
                                _clickValueTime = 0;
                                _groupNum = 0;
                                setStableArray(sr, 30);
                            }
                        }
                    }
                }
            }

            Console.ReadKey();

            return ;
        }
    }
}