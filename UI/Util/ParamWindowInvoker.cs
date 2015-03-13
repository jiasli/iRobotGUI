using System;
using System.Collections.Generic;
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
		/// Show dialog for an Instruction.
		/// </summary>
		/// <param name="ins"></param>
		/// <param name="owner"></param>
		public static void ShowDialog(Instruction ins, Window owner)
		{
			BaseParamWindow dlg = null; 			

			switch (ins.opcode)
			{
				case Instruction.FORWARD:
					
					break;
				case Instruction.LEFT:
					break;
				case Instruction.LED:
					dlg = new LedWindow();
					break;
				case Instruction.SONG_DEF:
					dlg = new SongWindow();
					break;
				case Instruction.DEMO:
					dlg = new DemoWindow();
					break;
			}          

			if (dlg != null)
			{
				dlg.Owner = owner;
				dlg.Ins = ins;
				dlg.ShowDialog();
			}
			else 
			{
				MessageBox.Show(ins.opcode + " no implemented.");
			}				
		}

		/// <summary>
		/// Show dialog for IF or LOOP.
		/// </summary>
		/// <param name="program"></param>
		/// <param name="owner"></param>
		public static void ShowDialog(HLProgram program, Window owner)
		{
			if (program[0].opcode == Instruction.IF)
			{
				IfWindow dlg = new IfWindow();
				dlg.Owner = owner;
				dlg.SubProgram = program;
				dlg.ShowDialog();
			}
			else if (program[0].opcode == Instruction.LOOP)
			{
				LoopWindow dlg = new LoopWindow();
				dlg.Owner = owner;
				dlg.SubProgram = program;
				dlg.ShowDialog();
			}
		}

	}
}
