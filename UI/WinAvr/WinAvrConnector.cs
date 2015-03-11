using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace iRobotGUI.WinAvr
{
	public class WinAvrConnector
	{
		public static WinAvrConfiguation config= new WinAvrConfiguation();

		/// <summary>
		/// Tweak makefile so that WinAVR can work properly.
		/// </summary>
		public static void CustomizeMakefile()
		{
			string makefile = File.ReadAllText("makefile_template");
			makefile = makefile.Replace("{COM}", config.comPort);
			makefile = makefile.Replace("{FIRMWARE_VERSION}", config.firmwareVersion);
			File.WriteAllText("makefile", makefile);
		}

		/// <summary>
		/// Call "make all" to compile C. 
		/// </summary>
		public static void Make()
		{
			CustomizeMakefile();
			string d = Directory.GetCurrentDirectory();
			System.Diagnostics.Process.Start("make", "all");
		}

		/// <summary>
		/// Call "make program" to load to Microcontroller.
		/// </summary>
		public static void Load()
		{         
			//string d = Directory.GetCurrentDirectory();

			CustomizeMakefile();
			System.Diagnostics.Process.Start("make","program");
		}

		/// <summary>
		/// Call "make clean" to clean all generated file.
		/// </summary>
		public static void Clean()
		{
			System.Diagnostics.Process.Start("make", "clean");
		}
	}
}
