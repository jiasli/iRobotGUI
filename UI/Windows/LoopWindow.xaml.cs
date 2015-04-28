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
	/// <remarks>
	/// In high-level program, the condition to enter the loop body is that the condition is true.
	/// However, in UI, the condition to enter the loop body is that the sensor is not detected.
	/// There for NOT_EQUAL is used for the condition.
	/// </remarks>
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

				// 1. Select the sensor
				sensorSelector.SelectedSensor = subProgram[0].paramList[0];

				// 2. Set loop body
				programListLoopBody.Program = subProgram.SubProgram(1, subProgram.Count - 2);
			}
			get
			{
				HLProgram result = new HLProgram();

				// 1. Read back sensor
				Instruction conditionIns = Instruction.CreatDefaultFromOpcode(Instruction.LOOP);
				conditionIns.paramList[0] = sensorSelector.SelectedSensor;
				// Notice that the Operator for LOOP is different from that for IF, which is EQUAL.
				conditionIns.paramList[1] = iRobotGUI.Operator.EQUAL;
				conditionIns.paramList[2] = 0;
				result.Add(conditionIns);

				// 2. Add loop body
				result.Add(programListLoopBody.Program);

				// 3. Add END_LOOP
				result.Add(Instruction.CreatDefaultFromOpcode(Instruction.END_LOOP));

				return result;
			}
		}

		private void InstructionPanel_AddNewInstruction(string opcode)
		{
			programListLoopBody.AddNewInstruction(opcode);
		}



	}
}
