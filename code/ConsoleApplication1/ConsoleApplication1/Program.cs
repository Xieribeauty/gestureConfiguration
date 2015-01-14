///this is a test for serial read and handle
///by tirstan
///
///2014/12/

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
using System.Threading;


namespace ConsoleApplication1
{
    class Program
    {
        //----------------------------------------------------- GLOBAL PARAMETER ------------------------------------------------------
        
        static private List <SerialPort> _serialPort = new List<SerialPort>();
        static private List <byte[]> _buffer = new List<byte[]>();
        static private int _wait = 4;

        static private string _thumbName = "COM13";
        static private string _indexName = "COM16";
        static private string _middleName = "COM15";

        static private SerialPort _thumb = new SerialPort(_thumbName);
        static private SerialPort _index = new SerialPort(_indexName);
        static private SerialPort _middle = new SerialPort(_middleName);

        static private byte[] _thumbBuffer = new byte[12];
        static private byte[] _indexBuffer = new byte[12];
        static private byte[] _middleBuffer = new byte[12];

        //--------------------------------------------------- INIT FUNCTIONS ----------------------------------------------------------
    	
    	static private void initialization(){             //init all
            SetPortOption(_thumb);
            SetPortOption(_index);
            SetPortOption(_middle);

            //CreateBuffer();
            return;
    	}
        
        static private void SetPortOption(SerialPort inPort) {              //Set the port objs
            SerialPort tPort = inPort;

            tPort.BaudRate = 19200;
            tPort.Parity = Parity.None;
            tPort.StopBits = StopBits.One;
            tPort.DataBits = 8;
            tPort.ReceivedBytesThreshold = 12;
            tPort.Handshake = Handshake.None;
            
            tPort.RtsEnable = true;
            tPort.DtrEnable = true;
            
            //tPort.DataReceived += DataReceivedHandler;
            tPort.Open();
            
            return;
        }

        //-------------------------------------------------- FEATURED FUNCTION ----------------------------------------------------
        static public bool filter (ref string tData, ref int[] data) {
            char[] splitChar0 = {'\n'};
            char[] splitChar1 = {'\t'};

            if (tData == ""){//move this to a later location
                return false;
            }

            string[] s0 = tData.Split(splitChar0);
            string[] s1 = s0[1].Split(splitChar1);
            //try catch, what if the second data group is not complete, if not try the next group<<
            data[0] = Convert.ToInt32(s1[0]);
            data[1] = Convert.ToInt32(s1[1]);
            data[2] = Convert.ToInt32(s1[2]);

            return true;
        }

        static public void thumbStart(){
            SerialPort tPort = _thumb;              //serial port
            string tData;               //data of string type
            int[] data = new int[3];                //data list, every 3 datas is one group
            int temp = 0;
            while (true){
                tData = tPort.ReadExisting();
                //tPort.DiscardInBuffer();
                Console.WriteLine(tData);
                //if (!filter(ref tData, ref data)){
                //    continue;
                //}
                Thread.Sleep(_wait);

                Console.WriteLine("--thumb{0}\n\n", temp);
                temp++;
                if (temp == 30) {
                    break;
                }
            }
        }

        static public void indexStart(){
            SerialPort tPort = _index;
            string tData;               //data of string type
            int[] data = new int[3];                //data list, every 3 datas is one group
            int temp = 0;
            while (true){
                tData = tPort.ReadExisting();
                //tPort.DiscardInBuffer();
                Console.WriteLine(tData);
                //if (!filter(ref tData, ref data)){
                //    continue;
                //}
                Thread.Sleep(_wait);

                Console.WriteLine("--thumb{0}\n\n", temp);
                temp++;
                if (temp == 30) {
                    break;
                }
            }
        }

        static public void middleStart(){
            SerialPort tPort = _middle;
            string tData;               //data of string type
            int[] data = new int[3];                //data list, every 3 datas is one group
            int temp = 0;
            while (true){
                tData = tPort.ReadExisting();
                //tPort.DiscardInBuffer();
                Console.WriteLine(tData);
                //if (!filter(ref tData, ref data)){
                //    continue;
                //}
                Thread.Sleep(_wait);

                Console.WriteLine("--thumb{0}\n\n", temp);
                temp++;
                if (temp == 30) {
                    break;
                }
            }
        }

        //-------------------------------------------------- RETRIVE FUNCTION ----------------------------------------------------
         
        static private void Retrive(){              //Retrive the source
            foreach (SerialPort port in _serialPort){
                port.Close();
            }
            return;
        }

        //-------------------------------------------------- MAIN -----------------------------------------------------------------
    	
        static void Main(string[] args){                //Main function
        	initialization();

            Thread thumb = new Thread(new ThreadStart(thumbStart));
            Thread index = new Thread(new ThreadStart(indexStart));
            Thread middle = new Thread(new ThreadStart(middleStart));

            thumb.Start();
            index.Start();
            middle.Start();

            Retrive();

            Console.ReadLine();
        }
    };
}

/*
创建线程
    com16
    com13
    com15

    com7
    com8
    com10

开始线程
    开启对应串口
    循环
        读取
        分离数据
        判断是否稳定（1、之前的动作状态是否结束；2、如果之前是稳定状态或动作状态刚刚结束，判断是否符合稳定状态0；）
            若是，
                若符合稳定态0，go on
                若符合稳定态1，将1置为0，清空原稳定态0队列
            若非，
                根据变化判断手势
                    每个传感器对应几个手势
                    每个手势对应三条数据流
                    每条数据流对应几个状态
                    每个状态对应一个采样点的数据范围
                    当三条数据流的同时状态都满足或者至少两个满足时，该状态有效
                    判断是否为最后一个状态
                存入稳定态1队列或清空该队列
        将结果存入对应buffer（锁机制）
 */

/* 可能用到的语句
temp = _serialPort[i].Read(_buffer[i], 100, _buffer[i].Length);

 */ 

/*
单击 食指 阈值 32767

 */