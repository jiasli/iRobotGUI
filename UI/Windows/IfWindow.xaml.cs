using iRobotGUI.Util;
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
using iRobotGUI.Controls;

namespace iRobotGUI
{
	/// <summary>
	/// Interaction logic for IfWindow.xaml
	/// </summary>
	public partial class IfWindow : Window
	{
		/// <summary>
		/// Set the high-level program to be presented, including IF and END_IF.
		/// </summary>
		public HLProgram SubProgram
		{
			set
			{
				HLProgram subProgram = value;

				// 1. Select the sensor
				sensorSelector.SelectedSensor = subProgram[0].paramList[0];

				// 2. Set if body
				int elsePosition = subProgram.FindElse(0);
				programListIfBody.Program = subProgram.SubProgram(1, elsePosition - 1);

				// 3. Set else body
				int endIfPosition = subProgram.FindEndIf(0);
				programListElseBody.Program = subProgram.SubProgram(elsePosition + 1, endIfPosition - 1);
			}
			get
			{
				HLProgram result = new HLProgram();

				// 1. Read back sensor
				Instruction conditionIns = Instruction.CreatDefaultFromOpcode(Instruction.IF);
				conditionIns.paramList[0] = sensorSelector.SelectedSensor;
				conditionIns.paramList[1] = iRobotGUI.Operator.EQUAL;
				conditionIns.paramList[2] = 1;
				result.Add(conditionIns);

				// 2. Add if body
				result.Add(programListIfBody.Program);

				// 3. Add ELSE
				result.Add(Instruction.CreatDefaultFromOpcode(Instruction.ELSE));

				// 2. Add if body
				result.Add(programListElseBody.Program);

				// 3. Add END_IF
				result.Add(Instruction.CreatDefaultFromOpcode(Instruction.END_IF));

				return result;
			}
		}

		public IfWindow()
		{
			InitializeComponent();
		}

		private void InstructionPanel_AddNewInstruction(string opcode)
		{
			MessageBox.Show(this, "Use drag-and-drop to add new instructions.");
		}
	}
}
