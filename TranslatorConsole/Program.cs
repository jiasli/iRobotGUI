using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iRobotGUI;

namespace TranslatorConsole
{
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
				string input_program = File.ReadAllText(fileName);
				string output_program;
				Console.WriteLine(input_program + "\n>>>>>>>>>\n");
				output_program = Translator.TranslateProgramString(input_program);
				Console.WriteLine(output_program);
				string c_program = File.ReadAllText("mc_t.c");
				c_program = c_program.Replace("##main_program##", output_program);
                File.WriteAllText("testTrans2.c", c_program);
			}
			catch (InstructionExpection e)
			{
				Console.WriteLine(e.Line + " " + e.InsStr);
			}
			Console.Read();
		}
	}
}
