using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace iRobotGUI
{         
	/// <summary>
	/// Base window for all ParamWindows. It has a property Ins which can be overridden to achieve
	/// specific logic.
	/// </summary>
	public class BaseParamWindow : Window
	{
		/// <summary>
		/// The Instruction being changed.
		/// </summary>
		public virtual Instruction Ins
		{
			get;
			set;
		}

	}
}
