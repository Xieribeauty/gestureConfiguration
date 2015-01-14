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
        static SerialPort ComPort;

        public static void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            string data = ComPort.ReadExisting();
            //ComPort.DiscardInBuffer();
            //Console.Write(data.Replace("\r", "\n"));
            Console.Write(data);
        }

        static void Main(string[] args)
        {
            string port = "COM9";
            int baud = 600;
            if (args.Length >= 1)
            {
                port = args[0];
            }
            if (args.Length >= 2)
            {
                baud = int.Parse(args[1]);
            }

            InitializeComPort(port, baud);

            string text;
            do
            {
                String[] mystring = System.IO.Ports.SerialPort.GetPortNames();

                text = Console.ReadLine();
                text = ComPort.ReadExisting();
                Console.Write(text);
                int STX = 0x2;
                int ETX = 0x3;
                ComPort.Write(Char.ConvertFromUtf32(STX) + text + Char.ConvertFromUtf32(ETX));
            } while (text.ToLower() != "q");
        }

        private static void InitializeComPort(string port, int baud)
        {
            ComPort = new SerialPort(port, baud);
            ComPort.PortName = port;
            ComPort.BaudRate = baud;
            ComPort.Parity = Parity.None;
            ComPort.StopBits = StopBits.One;
            ComPort.DataBits = 8;
            ComPort.ReceivedBytesThreshold = 9;
            ComPort.RtsEnable = true;
            ComPort.DtrEnable = true;
            ComPort.Handshake = System.IO.Ports.Handshake.XOnXOff;
            ComPort.DataReceived += OnSerialDataReceived;
            OpenPort(ComPort);
        }

        public static void OpenPort(SerialPort ComPort)
        {
            try
            {
                if (!ComPort.IsOpen)
                {
                    ComPort.Open();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}