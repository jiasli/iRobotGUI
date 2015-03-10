using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI.WinAvr
{
	/// <summary>
	/// This class is the information for WinAVR compilation and loading routine.
	/// </summary>
	public class WinAvrConfiguation
	{
		public string comPort = "COM1";  // com4
		public string firmwareVersion = STK500V1; // stk500v1

		public const string STK500 = "stk500";
		public const string STK500V1 = "stk500v1";
		public const string STK500V2 = "stk500v2";


		public override string ToString()
		{
			return string.Format("COM Port: {0}\nFirmware Version: {1}", comPort, firmwareVersion);
		}
	}
}
