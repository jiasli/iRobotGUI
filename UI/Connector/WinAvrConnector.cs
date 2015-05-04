using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using iRobotGUI.Properties;

namespace iRobotGUI.WinAvr
{
	/// <summary>
	/// This class is the information for WinAVR compilation and loading routine.
	/// </summary>
	internal class WinAvrConfiguation
	{
		public string comPort = "COM1";  // com1
		public string firmwareVersion = STK500V1; // stk500v1

		public const string STK500 = "stk500";
		public const string STK500V1 = "stk500v1";
		public const string STK500V2 = "stk500v2";


		public override string ToString()
		{
			return string.Format("COM Port: {0}\nFirmware Version: {1}", comPort, firmwareVersion);
		}
	}

	public class WinAvrConnector 
	{
		public const string MicrocontrollerTemplate = "mc_t.c";
		public const string MicrocontrollerOutputFile = "mc_o.c";
		public const string MakefileTemplate = "makefile_template";
		public const string MakefileOutput = "makefile";

		/// <summary>
		/// Customize makefile so that WinAVR can work properly. The COM port and Firmware will be
		/// changed according to the settings.
		/// </summary>
		public static void CustomizeMakefile()
		{			
			WinAvrConfiguation config = new WinAvrConfiguation();

			if (Settings.Default.MicrocontrollerComPort == "")
				throw new ComPortException();
			else config.comPort = Settings.Default.MicrocontrollerComPort;

			if (Settings.Default.FirmwareVersion == "")
				config.firmwareVersion = WinAvrConfiguation.STK500V1;
			else config.firmwareVersion = Settings.Default.FirmwareVersion;


			string makefileStr = File.ReadAllText(MakefileTemplate);
			makefileStr = makefileStr.Replace("{COM}", config.comPort);
			makefileStr = makefileStr.Replace("{FIRMWARE_VERSION}", config.firmwareVersion);
			File.WriteAllText(MakefileOutput, makefileStr);
		}

		public static void TranslateToC(HLProgram program)
		{
			string cCode = Translator.Translate(program);
			CodeGenerator.GenerateCSource(MicrocontrollerTemplate, MicrocontrollerOutputFile, cCode);			
		}

		/// <summary>
		/// Call "make all" to compile C. 
		/// </summary>
		public static void Make()
		{
			CustomizeMakefile();
			Process winAvrProcess = Process.Start("make.exe", "all");
			winAvrProcess.WaitForExit();
		}

		/// <summary>
		/// Call "make program" to load to Microcontroller.
		/// </summary>
		public static void Load()
		{
			//string d = Directory.GetCurrentDirectory();

			CustomizeMakefile();
			Process winAvrProcess = System.Diagnostics.Process.Start("make.exe", "program");
			winAvrProcess.WaitForExit();
		}

		/// <summary>
		/// Call "make clean" to clean all generated file.
		/// </summary>
		public static void Clean()
		{
			Process winAvrProcess = System.Diagnostics.Process.Start("make.exe", "clean");
			winAvrProcess.WaitForExit();
		}
	}
}
