///this is a test for serial read and handle
///by tirstan
///
///2014/12/28

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

namespace handleInput{

	enum Axis {x, y, z};

	public struct StableAxis {
		public int[] data;
		public bool status;
		public float average;
		public StableAxis() {
			data = new int[10];
			status = false;
			average = 0;
		}
	};

	public class Port {
		public SerialPort port;
		public string name;
		public byte[] buffer;
		public StableAxis stableX;
		public StableAxis stableY;
		public StableAxis stableZ;

		public Port (string tName){
			//try catch if there is no serial port named tName
			port = new SerialPort(tName);
			name = tName;
			buffer = new byte[12];
			stableX = new StableAxis;
			stableY = new StableAxis;
			stableZ = new StableAxis;
		}

		public bool isStable(Axis a, int tIn) {
			switch a {
				case x:
					if (tIn - stableX.average > threshold || stableX.average - tIn > threshold){
						return false;
					}
					return true;
				case x:
					if (tIn - stableY.average > threshold || stableY.average - tIn > threshold){
						return false;
					}
					return true;
				case x:
					if (tIn - stableZ.average > threshold || stableZ.average - tIn > threshold){
						return false;
					}
					return true;
				default:
					return false;
			}
		}
	};

	class Program{

        //----------------------------------------------------- GLOBAL PARAMETER ------------------------------------------------------
        
        static private List <SerialPort> _serialPort = new List<SerialPort>();
        static private List <byte[]> _buffer = new List<byte[]>();
        static private int _wait = 4;

        static private Port _thumb = new Port("COM8");
        static private Port _index = new Port("COM10");
        static private Port _middle = new Port("COM7");

        static private int[] _clickIndexZ = {-13000, 13000};
        static private int[] _clickThumbX = {-9600};
        static private int _clickRecognizeTime = 95;
        static private int _clickTimeout = 75;

        static private int threshold = 5400;

		//-------------------------------------------------- FEATURED FUNCTION ----------------------------------------------------
        
        static public bool filter (ref string tData, ref int[] data) {
            char[] splitChar0 = {'\n'};
            char[] splitChar1 = {'\t'};

            if (tData == ""){//move this to a later location
                return false;
            }

            string[] s0 = tData.Split(splitChar0);
            string[] s1;
            int i = 1;

            for (; i < s0.Length; i++){
            	s1 = s0[i].Split(splitChar1);
            	if (s1.Length = 3){
            		data[0] = Convert.ToInt32(s1[0]);
		            data[1] = Convert.ToInt32(s1[1]);
		            data[2] = Convert.ToInt32(s1[2]);
		            break;
            	}
            }

            if (i < s0.Length){
            	return true;
            }
            return false;
        }

        static public bool clickThumbStart() {
        	//read data
        	//
        }

        static public bool clickIndexStart() {}

        static public bool isClick (int dThumbX, int dIndexZ, int i) {

        }

        //-------------------------------------------------- MAIN -----------------------------------------------------------------

        static void Main(string[] arg) {				//Main function

            Thread ClickThumb = new Thread(new ThreadStart(clickThumbStart));
            Thread ClickIndex = new Thread(new ThreadStart(clickIndexStart));


        	string stringThumb;
        	string stringIndex;
        	string stringMiddle;

        	int[] dataThumb = new int[3];
        	int[] dataIndex = new int[3];
        	int[] dataMiddle = new int[3];

        	bool thumbDataStatus = false;
        	bool indexDataStatus = false;
        	bool doubleDataStatus = false;

        	bool motionThumb  = false;
        	bool motionIndex  = false;
        	bool motionMiddle  = false;

        	bool isMotivate = false;

        	// init the stable array by the first ten groups
        	for (int i = 0; i < 100; i++){
        		//read from the file to data arrays
        		_thumb.stableLiX[i] = dataThumb[0];
        		_thumb.stableLiY[i] = dataThumb[1];
        		_thumb.stableLiZ[i] = dataThumb[2];
        	}

        	while(true){
	        	//read from file <<
	        	
        		motionThumb  = false;
        		motionIndex  = false;
        		motionMiddle  = false;

	        	thumbDataStatus = filter(ref stringThumb, ref dataThumb);
	        	indexDataStatus = filter(ref stringIndex, ref dataIndex);
	        	middleDataStatus = filter(ref stringMiddle, ref dataMiddle);
				
	        	if (thumbDataStatus&&indexDataStatus) {
	        		if (!_thumb.isStable(x, dataThumb[0]) && !_index.isStable(z, dataIndex[2])){
	        			isMotivate = true;
	        		}
	        	}

	        	if (isMotivate) {
	        		
	        		//DateTime beginTime = DateTime.Now(); //<< now the timer is by group
	        		//int excuteTime =  DateTime.Compare(DateTime.Now(), beginTime);
	        		
	        		bool[] iStatus = {false, false};

	        		for (int i = 0; i < 2 ; i++){

		        		for (int numGroup = 0; numGroup < 95; numGroup++){
		        			
		        		}

			        	if ((!isClick<<(data<<, i))&&tieout<<){
			        		//go to if stable;
			        	}

	        			//read from file <<
			        	thumbDataStatus = filter(ref stringThumb, ref dataThumb);
			        	indexDataStatus = filter(ref stringIndex, ref dataIndex);
			        	middleDataStatus = filter(ref stringMiddle, ref dataMiddle);
	        		}
	        		if (i == length<<){
	        			//success and click
	        		}
	        	}

        	}
        }
	};
}