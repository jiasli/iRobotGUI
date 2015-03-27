using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iRobotGUI;

namespace TranslatorConsole
{
	/// <summary>
	/// Used to test TranslatorLib
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			string fileName;
			if (args.Length == 0)
			{
				fileName = "input.igp";
			}
			else
			{
				fileName = args[0];
			}

			Console.WriteLine("Read from: " + fileName);

			try
			{
				string igpStr = File.ReadAllText(fileName);
				string cProgram;

				Console.WriteLine(igpStr + "\n>>>>>>>>>\n");
				cProgram = Translator.Translate(new HLProgram(igpStr));
				Console.WriteLine(cProgram);

				string c_program = File.ReadAllText("mc_t.c");
				c_program = c_program.Replace("##main_program##", cProgram);
				File.WriteAllText("console_o.c", c_program);
			}
			catch (InstructionException e)
			{
				Console.WriteLine(e.Line + " " + e.InsStr);
			}
			Console.ReadLine();
		}
	}
}
