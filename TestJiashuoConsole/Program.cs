using iRobotGUI;
using iRobotGUI.ViewModel;
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
			LedViewModel imb = new LedViewModel();

			imb.Ins = Instruction.CreatFromOpcode("LED");
			Console.WriteLine(imb.Ins.ToString());
		}
	}
}
