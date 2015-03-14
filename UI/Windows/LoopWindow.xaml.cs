using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using iRobotGUI.Util;

namespace iRobotGUI
{

	/// <summary>
	/// Interaction logic for LoopWindow.xaml
	/// </summary>
	public partial class LoopWindow : Window
	{
		public LoopWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Set the high-level program to be presented, including LOOP and END_LOOP.
		/// </summary>
		public HLProgram SubProgram
		{
			set
			{
				HLProgram subProgram = value;

				// 1. Set condition
				InsCondition condition = new InsCondition(subProgram[0]);
				conditionPanel.Condition = condition;

				// 2. Set loop body
				programListLoopBody.Program = subProgram.SubProgram(1, subProgram.Count - 2);
			}
			get
			{
				HLProgram result = new HLProgram();

				// 1. Read back condition
				Instruction conditionIns = Instruction.CreatFromOpcode(Instruction.LOOP);
				InsCondition condition = conditionPanel.Condition;
				conditionIns.paramList[0] = condition.sensor;
				conditionIns.paramList[1] = condition.op;
				conditionIns.paramList[2] = condition.num;
				result.Add(conditionIns);

				// 2. Add loop body
				result.Add(programListLoopBody.Program);

				// 3. Add END_LOOP
				result.Add(Instruction.CreatFromOpcode(Instruction.END_LOOP));

				return result;
			}
		}



	}
}
