using iRobotGUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestJiashuoConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			Instruction ins1 = new Instruction("MOVE 100, 2");
			Instruction ins2 = new Instruction("MOVE -100, 2");
			Instruction ins3 = new Instruction("MOVE 0, 2");

			Console.WriteLine(TextDescriber.GetTextDescription(ins1));
			Console.WriteLine(TextDescriber.GetTextDescription(ins2));
			Console.WriteLine(TextDescriber.GetTextDescription(ins3));

			Console.WriteLine(ins1.ToString(true));
			Console.WriteLine(ins2.ToString(true));
			Console.WriteLine(ins3.ToString(true));

			string commentPattern = "//.*";
			Regex rgx = new Regex(commentPattern);
			var result = rgx.Replace("aa /// dsfsdf","");

			Console.WriteLine(result);

			string input = File.ReadAllText("demo_forward_until_bump.igp");
			HLProgram pro = new HLProgram(input);
			File.WriteAllText("demo_forward_until_bump_with_description.igp", pro.ToString(true));

		}
	}
}
