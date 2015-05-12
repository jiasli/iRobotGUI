using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
	public class EmulatorConnector
	{
		public const string EmulatorTemplate = "em_t.c";
		public const string EmulatorOutputFile = "em_o.c";

		public const string MainTemplate = "main_t.c";
		public const string MainOutput = "main.c";

		public static void TranslateToC(HLProgram program)
		{
			// Program file
			string emulatorPath = Properties.Settings.Default.EmulatorPath;

			string cCode = Translator.Translate(program);
			string templateFullPath = System.IO.Path.Combine(emulatorPath, "MCEmulator", EmulatorTemplate);
			string outputFullPath = System.IO.Path.Combine(emulatorPath, "MCEmulator", EmulatorOutputFile);

			CodeGenerator.GenerateCSource(templateFullPath, outputFullPath, cCode);

			// Main file and COM Port
			if (Properties.Settings.Default.EmulatorComPort == "")
				throw new ComPortException();
			else
			{
				string mainTemplateFullPath = System.IO.Path.Combine(emulatorPath, "MCEmulator", MainTemplate);
				string mainOutputFullPath = System.IO.Path.Combine(emulatorPath, "MCEmulator", MainOutput);
				string mainStr = File.ReadAllText(mainTemplateFullPath);

				// COM1 is the 0th
				int comNo = Int32.Parse(Properties.Settings.Default.EmulatorComPort.Substring(3)) - 1;
				mainStr = mainStr.Replace("/**change_com_port**/", (comNo.ToString()));
				File.WriteAllText(mainOutputFullPath, mainStr);
			}
		}

		public static void Build()
		{
			string emulatorPath = Properties.Settings.Default.EmulatorPath;
			string buildBatName = "MSMake.bat";
			string buildBatFullPath = System.IO.Path.Combine(emulatorPath, buildBatName);

			Process process = Process.Start(buildBatFullPath);
			process.WaitForExit();
		}

		public static void Run()
		{
			string emulatorPath = Properties.Settings.Default.EmulatorPath;
			string exeName = @"Debug\MCEmulator.exe";
			string exeFullPath = System.IO.Path.Combine(emulatorPath, exeName);

			Process process = Process.Start(exeFullPath);
			process.WaitForExit();
		}
	}
}
