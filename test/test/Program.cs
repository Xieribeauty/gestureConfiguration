// usings
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
        // class of stable queue
            class Stable {
                public int[] x = new int[50];
                public int[] y = new int[50];
                public int[] z = new int[50];
                public int[] axis = new int[3];
                public int cursor = 0;
            };

        // struct of stable 350 queue
            struct LongStable {
                int valueNum;
                int[] queue;
                int threshold;
                void init() {
                    valueNum = 0;
                    queue = new int[7];
                    threshold = 350;
                }
            };

        // others
            // 运动状态
            static private int _status = 0;
            // 阈值
            static private int _threshold = 5300;

        // source data array(including thumb index and middle finger)
            static private int[] _thumbData = new int[3];
            static private int[] _indexData = new int[3];
            static private int[] _middleData = new int[3];

        // temp parameter
            static private int _temp_clickTime = 0;
            static private int _temp_totalGroup = 0;
            static private int _temp_motivateTime = 0;

        // static private String _filePath = "../../../testResource/5_100/index.txt";
            static private String _filePath = "../../../testResource/4/index.txt";
            static private StreamReader _sr = new StreamReader(_filePath);

        // stable loop queue
            static private Stable _stableIndex = new Stable();
            static private Stable _stableMiddle = new Stable();

        // stable 350 loop queue
            static private LongStable _longIndexZ = new LongStable();
            static private LongStable _longthumbZ = new LongStable();

        // 单击参数
            static private bool[] _clickIndex = {false, true};
            static private bool _isClick;
            static private int _clickValueTime = 0; // temp
            static private int _clickRecognizeGroup = 67;
            static private int _clickRest = 26;
            static private int _clickPartitionNow = 0;

        static private void setStableArray(int n) {
            String line;
            // 创建稳定数组
            for (int i = 0; i < n && (line = _sr.ReadLine()) != null; i++){
                char[] splitChar = {'\t'};
                String[] s = line.Split(splitChar);

                _indexData[0] = Convert.ToInt32(s[0]);
                _indexData[1] = Convert.ToInt32(s[1]);
                _indexData[2] = Convert.ToInt32(s[2]);

                _stableIndex.x[i] = _indexData[0];
                _stableIndex.y[i] = _indexData[1];
                _stableIndex.z[i] = _indexData[2];
                _stableIndex.axis[0] += _indexData[0];
                _stableIndex.axis[1] += _indexData[1];
                _stableIndex.axis[2] += _indexData[2];
            }

            _temp_totalGroup += n;

            // 默认输入超过50组，赋值稳定数组均值
            _stableIndex.axis[0] = _stableIndex.axis[0]/n;
            _stableIndex.axis[1] = _stableIndex.axis[1]/n;
            _stableIndex.axis[2] = _stableIndex.axis[2]/n;

            for (; n < 50; n++){
                _stableIndex.x[n] = _stableIndex.axis[0];
                _stableIndex.y[n] = _stableIndex.axis[1];
                _stableIndex.z[n] = _stableIndex.axis[2];
            }

            _stableIndex.cursor = n-1;

            return;
        }

        // 因正常或异常结束而重置click参数
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
                            //Console.WriteLine("click recognized by index data\n-----{0},{1}"
                            //    , _temp_clickTime
                            //    , _temp_totalGroup
                            //);
                        }
                    }
                }
                // 未完成有效 2
                else {
                    if (data[2] < -_threshold){
                        _clickValueTime++;
                        if (_clickValueTime == _clickIndex.Length){
                            _temp_clickTime++;
                            //Console.WriteLine("click recognized by index data\n-----{0},{1}"
                            //    , _temp_clickTime
                            //    , _temp_totalGroup
                            //);
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

        // main function
        static void Main(String[] args) {

            String line;
            setStableArray(50);

            // 继续读取直到文件的末尾 
            while ((line = _sr.ReadLine()) != null) {
                char[] splitChar = {'\t'};
                String[] s = line.Split(splitChar);
                int[] data = new int[3];

                data[0] = Convert.ToInt32(s[0])-_stableIndex.axis[0];
                data[1] = Convert.ToInt32(s[1])-_stableIndex.axis[1];
                data[2] = Convert.ToInt32(s[2])-_stableIndex.axis[2];

                _temp_totalGroup++;

                if (_status == 0) { // 之前是平衡状态

                    if (data[2]>-_threshold && data[2]<_threshold) { // 是平衡状态

                        _stableIndex.axis[2] = _stableIndex.axis[2] + (
                            Convert.ToInt32(s[2]) - _stableIndex.z[_stableIndex.cursor]
                            )/50;
                        _stableIndex.z[_stableIndex.cursor] = Convert.ToInt32(s[2]);
                        _stableIndex.cursor = (_stableIndex.cursor + 1)%50;

                    }
                    else { // 运动状态初始
                        
                        _temp_motivateTime++;
                        Console.WriteLine("this is the beginning of motivation status {0}\n{1}", _temp_totalGroup, _temp_motivateTime);
                        bool temp0 = data[2] < -_threshold;
                        if (temp0) {
                            _status ++;
                            _clickValueTime++;
                            _clickPartitionNow = 1;
                            _isClick = true;
                        }
                        else {
                            // 非单双击动作
                        }
                    }
                }
                else { // 之前是运动状态
                    if (_isClick) {
                        if (!goOnClick(data)){
                            // click 结束
                        }
                    }
                }
            }

            Console.ReadKey();

            return ;
        }
    }
}
