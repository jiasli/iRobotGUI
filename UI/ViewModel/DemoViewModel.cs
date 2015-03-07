using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI.ViewModel
{
	public class DemoViewModel
	{
		private Instruction ins;

		public DemoViewModel()
		{
			ins = Instruction.CreatFromOpcode(Instruction.DEMO);
		}

		public Instruction Ins
		{
			get
			{
				return ins;
			}
			set
			{
				ins = value;
			}
		}

		public int DemoNumber
		{
			get
			{
				return ins.parameters[0];
			}
			set
			{
				ins.parameters[0] = value;
			}
		}
	}
}
