using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI.Util
{
	/* The Mapping Mechanism
	 * 
	 * ViewModel : Pointers          Program : Instructions
	 *                [0] 0 -------> [0] MOVE ...
	 *                [1] 1 -------> [1] LED ...
	 *                [2] 2 -------> [2] LOOP ...
	 *                [3] 5 --\      [3]     MOVE...
	 *                         \     [4] END_LOOP ...
	 *                          \--> [5] SONG ...
	 */

	/// <summary>
	/// This class consists of a list of points pointing to Instructions in a HLProgram. It
	/// describes the mapping from diagram (the graphical program presented in ProgramList) to HLProgram.
	/// 
	/// The mapping is a 0..1 1 mapping. For single instructions like DRIVE and LED, the original
	/// instruction is mapped. For sub-programs like IF and LOOP, the leading instruction which
	/// contains IF or LOOP is mapped.
	/// </summary>
	public class ProgramViewModel : List<int>
	{
		// The program being mapped.
		private HLProgram program;

		public ProgramViewModel(HLProgram program)
		{
			this.program = program;
			for (int i = 0; i < program.Count; i++)
			{
				if (program[i].opcode == Instruction.IF)
				{
					base.Add(i);
					i = program.FindEndIf(i);
				}
				else if (program[i].opcode == Instruction.LOOP)
				{
					base.Add(i);
					i = program.FindEndLoop(i);
				}
				else
				{
					base.Add(i);
				}
			}
		}

		/// <summary>
		/// Get the HLProgram for furthur processing, like translating.
		/// </summary>
		/// <returns></returns>
		public HLProgram GetHLProgram()
		{
			HLProgram result = new HLProgram();

			for (int pvmIndex = 0; pvmIndex < this.Count(); pvmIndex++)
			{
				Instruction ins = program[this[pvmIndex]];

				if (ins.opcode == Instruction.IF || ins.opcode == Instruction.LOOP)
					result.Add(GetSubProgram(pvmIndex));
				else
					// Single instruction.
					result.Add(GetInstruction(pvmIndex));
			}
			return result;
		}

		/// <summary>
		/// Get the Instruction pointed by the pointer at specified index.
		/// </summary>
		/// <param name="index">The index of pointer.</param>
		/// <returns>The instruction pointed by the pointer.</returns>
		public Instruction GetInstruction(int pvmIndex)
		{
			return program[this[pvmIndex]];
		}
				

		/// <summary>
		/// Get the sub-program pointed by the pointer at specified index.
		/// </summary>
		/// <param name="index">The index of pointer.</param>
		/// <returns>The sub-program pointed by the pointer.</returns>
		public HLProgram GetSubProgram(int pvmIndex)
		{
			int programIndex = this[pvmIndex];
			Instruction ins = program[programIndex];

			if (ins.opcode == Instruction.IF)
			{
				return program.SubProgram(programIndex, program.FindEndIf(programIndex));
			}
			else if (ins.opcode == Instruction.LOOP)
			{
				return program.SubProgram(programIndex, program.FindEndLoop(programIndex));
			}
			else
			{
				// A HLProgram with only one instruction.
				HLProgram result = new HLProgram();
				result.Add(ins);
				return result;
			}
		}

		/// <summary>
		/// Add new Instrction at specified index.
		/// </summary>
		/// <param name="index">The zero-base index at which the new instruction should be inserted. </param>
		/// <param name="newIns">The new Instruction.</param>
		public void InsertInstruction(int index, Instruction newIns)
		{
			this.Insert(index, program.Count);
			program.Add(newIns);
		}

		/// <summary>
		/// Add new sub-program at specified index.
		/// </summary>
		/// <param name="index">The zero-base index at which the new sub-program should be inserted. </param>
		/// <param name="newIns">The new sub-program.</param>
		public void InsertSubProgram(int index, HLProgram newSubProgram)
		{
			this.Insert(index, program.Count);
			program.Add(newSubProgram);
		}
		/// <summary>
		/// Update the Instrction at specified index.
		/// </summary>
		/// <param name="index">The zero-base index at which the instruction should be updated. </param>
		/// <param name="newIns">The new Instruction.</param>
		public void UpdateInstruction(int index, Instruction newIns)
		{
			program[this[index]] = newIns;
		}

		/// <summary>
		/// Update the SubProgram starting from specified index in ViewModel.
		/// </summary>
		/// <param name="index">The zero-base index at which the sub-program should be updated. </param>
		/// <param name="newSubProgram">The new sub-program.</param>
		public void UpdateSubProgram(int index, HLProgram newSubProgram)
		{
			// Update the pointer in ViewModel to the new position and add the new newSubProgram to
			// the end of program. We don't need to remove the previous sub-program since they will
			// no longer be indexed.
			this[index] = program.Count;
			program.Add(newSubProgram);
		}	

	}
}
