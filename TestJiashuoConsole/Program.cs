using iRobotGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		}
	}
}
