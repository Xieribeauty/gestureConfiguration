//#define MOTION 0;
//#define STABLE 1;<<

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
        //static private String _filePath = "../../../testResource/5_100/index.txt";
        static private String _filePath = "../../../testResource/4/index.txt";
        static private StreamReader _sr = new StreamReader(_filePath);

        static private int[] _stableX = new int[50];
        static private int[] _stableY = new int[50];
        static private int[] _stableZ = new int[50];
        static private int[] _stableAxis = new int[3];

        //stable循环队列游标
        static private int _stableCursorX = 0;
        static private int _stableCursorY = 0;
        static private int _stableCursorZ = 0;

        static private int _threshold = 5300;

        static private bool[] _clickIndex = {false, true};
        static private bool _isClick;
        static private int _clickValueTime = 0; // temp
        static private int _clickRecognizeGroup = 67;
        static private int _clickRest = 26;
        static private int _clickPartitionNow = 0;

        static private int _status = 0;

        static private void setStableArray(int n) {
            String line;
            //创建稳定数组
            for (int i = 0; i < n && (line = _sr.ReadLine()) != null; i++){
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

            _temp_totalGroup += n;

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

        //因正常或异常结束而重置click参数
        static private bool resetClick(bool isInTime) {
            if (isInTime) {
                _clickPartitionNow = 0;
                _clickValueTime = 0;
            }
            else {
                _clickPartitionNow = 0;
                _clickValueTime = 0;
                _isClick = false;
                _status--;
            }
            
            return false;
        }

        static private bool goOnClick(int[] data) {
             
            _clickPartitionNow++;

            // 未完成识别
            if (_clickValueTime < _clickIndex.Length) { 
                // 未完成超时
                if (_clickPartitionNow > _clickRecognizeGroup) {
                    if ((_clickPartitionNow - _clickRecognizeGroup) > _clickRest) {
                        resetClick(false);
                        setStableArray(30);
                        return false;
                    }
                }
                // 未完成有效 1
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
                // 未完成有效 2
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
            // 已经完成识别
            else { 
                if (_clickPartitionNow > _clickRecognizeGroup){
                    resetClick(true);
                    setStableArray(30);
                }
            }

            return true;
        }

        //main function
        static void Main(String[] args) {

            String line;
            setStableArray(50);

            // 继续读取直到文件的末尾 
            while ((line = _sr.ReadLine()) != null) {
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
                            _status ++;
                            _clickValueTime++;
                            _clickPartitionNow = 1;
                            _isClick = true;
                        }
                        else {
                            //非单双击动作
                        }
                    }
                }
                else {
                    //运动状态
                    if (_isClick) {
                        if (!goOnClick(data)){
                            //click 结束
                        }
                    }
                }
            }

            Console.ReadKey();

            return ;
        }
    }
}
