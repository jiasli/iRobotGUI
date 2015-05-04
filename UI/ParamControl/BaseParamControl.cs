using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace iRobotGUI.Controls
{
	/// <summary>
	/// Base control for all ParamControls. It has a property Ins which can be overridden to achieve
	/// specific logic.
	/// </summary>
	public class BaseParamControl : UserControl
	{
		public virtual Instruction Ins
		{
			get;
			set;
		}

	}
}
