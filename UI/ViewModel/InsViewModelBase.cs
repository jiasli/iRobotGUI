using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI.ViewModel
{
	public class InsViewModelBase : ViewModelBase
	{
		public virtual Instruction Ins
		{
			get;
			set;
		}
	}
}
