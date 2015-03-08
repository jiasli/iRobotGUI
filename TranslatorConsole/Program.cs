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
				Console.WriteLine(input_program + "\n>>>>>>>>>\n");
				Console.WriteLine(Translator.TranslateProgramString(input_program));
			}
			catch (InstructionExpection e)
			{
				Console.WriteLine(e.Line + " " + e.InsStr);
			}

		}
	}
}
