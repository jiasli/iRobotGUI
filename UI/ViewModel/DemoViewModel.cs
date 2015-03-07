using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI.ViewModel
{
	public class DemoViewModel : InsViewModelBase
	{

		public DemoViewModel()
		{
			Ins = Instruction.CreatFromOpcode(Instruction.DEMO);
		}
	

		public int DemoNumber
		{
			get
			{
				return Ins.parameters[0];
			}
			set
			{
				Ins.parameters[0] = value;
			}
		}
	}
}
