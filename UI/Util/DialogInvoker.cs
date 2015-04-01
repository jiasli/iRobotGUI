using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace iRobotGUI.Util
{
	/// <summary>
	/// This is a helper class which takes the instruction and invoke the window for it.
	/// </summary>
	public class DialogInvoker
	{
		/// <summary>
		/// Show dialog to modify an Instruction.
		/// </summary>
		/// <param name="ins">The Instruction being modified.</param>
		/// <param name="owner">The owner Window.</param>
		/// <returns>The modified Instruction.</returns>
		public static Instruction ShowDialog(Instruction ins, Window owner)
		{
			BaseParamWindow dlg = null;

			switch (ins.opcode)
			{
				case Instruction.MOVE:
					dlg = new MoveWindow();
					break;
				case Instruction.ROTATE:
					dlg = new RotateWindow();
					break;
				case Instruction.LED:
					dlg = new LedWindow();
					break;
				case Instruction.SONG:
					dlg = new SongWindow();
					break;
				case Instruction.DEMO:
					dlg = new DemoWindow();
					break;
				case Instruction.DRIVE:
					dlg = new DriveWindow();
					break;
				case Instruction.DELAY:
					dlg = new DelayWindow();
					break;

			}

			if (dlg != null)
			{
				dlg.Owner = owner;
				dlg.Ins = ins;
				dlg.ShowDialog();

				// Alway read from the Window. There is no guarantee that the original Instruction is modified.
				Instruction result = dlg.Ins;
				Debug.WriteLine(result);
				return dlg.Ins;
			}
			else
			{
				MessageBox.Show(ins.opcode + " no implemented.");
			}
			return null;
		}

		/// <summary>
		/// Show dialog for IF or LOOP to modify the sub-program.
		/// </summary>
		/// <param name="program">The program being modified, including IF ELSE END_IF and LOOP END_LOOP Instructions.</param>
		/// <param name="owner">The owner Window.</param>
		/// <returns>The modified HLProgram.</returns>
		public static HLProgram ShowDialog(HLProgram program, Window owner)
		{
			if (program[0].opcode == Instruction.IF)
			{
				IfWindow dlg = new IfWindow();
				dlg.Owner = owner;
				dlg.SubProgram = program;
				dlg.ShowDialog();
				return dlg.SubProgram;
			}
			else if (program[0].opcode == Instruction.LOOP)
			{
				LoopWindow dlg = new LoopWindow();
				dlg.Owner = owner;
				dlg.SubProgram = program;
				dlg.ShowDialog();
				return dlg.SubProgram;
			}
			return null;
		}

	}
}
