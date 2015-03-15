using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace iRobotGUI
{
	/// <summary>
	/// The class is used to detect invalid function or program
	/// Trace to WC_3297
	/// </summary>
	public class Validator
	{
		/// <summary>
		/// The number of line under validation. Starts from -1.
		/// </summary>
		static int currentLine = -1;


		static private Stack<string> ifStack = new Stack<string>();

		static private Stack<string> loopStack = new Stack<string>();

		/// <summary>
		/// The default parameter length of current instruction under validation 
		/// </summary>
		static int paraLen = 0;

		/// <summary>
		/// The corresponding possible parameter range of current instruction under validation
		/// </summary>
		static Dictionary<int, DictionaryDef.Boundary> paraRan = new Dictionary<int, DictionaryDef.Boundary>();

		/// <summary>
		/// The program under validation.
		/// </summary>
		static HLProgram program = null;
		#region private methods

		/// <summary>
		/// The function check matching situation of instruction IF, ELSE & END_IF
		/// </summary>
		/// <param name="opcode">Opcode of current instruction under validation, should be one of IF, ELSE or END_IF</param>
		/// <returns>True when the starting if is matched, false if the match has not been finished.</returns>
		/// <exception cref="IfUnmatchedException">Thorwn when if unmatched.</exception>
		private static bool IfPushStack(string opcode)
		{
			string IF = Instruction.IF;
			string ELSE = Instruction.ELSE;
			string END_IF = Instruction.END_IF;

			if (ifStack.Count == 0)
			{
				// Empty stack		
				if (opcode == IF)
				{
					// Only IF is accepted
					ifStack.Push(opcode);
					return false;
				}
				else
				{
					// Do not accept ELSE and END_IF
					throw new IfUnmatchedException(currentLine, opcode);
				}
			}
			else
			{
				// Non-empty stack
				string top = ifStack.Peek();

				if (opcode == IF)
				{
					// Accept all IF
					ifStack.Push(opcode);
					return false;
				}
				else if (opcode == ELSE)
				{
					// Only IF on top of stack can accept ELSE
					if (top == IF)
					{ 
						ifStack.Pop(); 
						ifStack.Push(opcode);
						return false;
					}
					else throw new IfUnmatchedException(currentLine, opcode);

				}
				else if (opcode == END_IF)
				{
					// Only ELSE on top of stack can accept END_IF
					if (top == ELSE) 
					{
						ifStack.Pop();

						// The starting IF is matched.
						if (ifStack.Count == 0) return true;
					}
					else throw new IfUnmatchedException(currentLine, opcode);
				}
			}
			return false;
		}

		/// <summary>
		/// The function check matching situation of instruction LOOP & END_LOOP
		/// </summary>
		/// <param name="opcode">Opcode of current instruction under validation, should be either LOOP or END_LOOP</param>
		/// <returns>Return true when they are matched, throw LoopUnmatchedException otherwise</returns>
		/// /// <exception cref="LoopUnmatchedException">Thorwn when if unmatched.</exception>
		private static bool LoopPushStack(string opcode)
		{
			if (opcode == Instruction.LOOP)
			{
				// Accept any LOOP.
				loopStack.Push(Instruction.LOOP);
				return false;
			}
			else if (opcode == Instruction.END_LOOP)
			{
				// Only accept END_LOOP when the stack is non-empty.
				if (loopStack.Count == 0)
					throw new LoopUnmatchedException(currentLine, opcode);
				else
				{
					loopStack.Pop();
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Validate IF block starting from the if at specified position.
		/// ValidateIfBlock -> IfPushStack
		/// </summary>
		/// <param name="firstIfIndex"></param>
		private static void ValidateIfBlock(int firstIfIndex)
		{
			int insIndex = firstIfIndex;
			ifStack.Clear();

			// Scan through the program until starting IF is matched.
			while (insIndex < program.Count)
			{
				string opcode = program[insIndex].opcode;
				if (opcode == Instruction.IF | opcode == Instruction.ELSE | opcode == Instruction.END_IF)
				{
					// The matching ELSE and END_IF are found.
					if (IfPushStack(program[insIndex].opcode)) return;
				}
				insIndex++;
			}

			// Not all IF ELSE END_IF get matched.
			if (ifStack.Count != 0) throw new IfUnmatchedException(currentLine, "End of file.");
		}

		private static void ValidateLoopBlock(int currentLine)
		{
			throw new NotImplementedException();
		}
		#endregion
		

		/// <summary>
		/// The function validate an instruction
		/// </summary>
		/// <param name="ins">Instruction under validate</param>
		/// <returns>True for a valid instuction or test, throw exception otherwise</returns>
		public static void ValidateInstruction(Instruction ins)
		{
			ValidateParaLength(ins, currentLine);

			ValidateParaRange(ins, currentLine);

			switch (ins.opcode)
			{
				//Validate effectiveness and length of parameter of SONG_DEF
				case Instruction.SONG_DEF:
					ValidateSongDef(ins);
					break;

				// Check matching situation of instruction LOOP & END_LOOP
				// Don't care END_LOOP
				case Instruction.LOOP:
					ValidateLoopBlock(currentLine);
					break;

				//check matching situation of instruction IF ELSE & END_IF
				// Don't care ELSE and END_IF
				case Instruction.IF:
					ValidateIfBlock(currentLine);
					break;
			}
		}

		public static bool ValidateParaLength(Instruction ins, int currentLine)
		{
			//Check Parameter Count
			if (DictionaryDef.paraLength.TryGetValue(ins.opcode, out paraLen))
			{
				if (ins.paramList.Count != paraLen)
					throw new ParameterCountInvalidException(currentLine, ins.opcode);
			}
			return true;
		}

		public static bool ValidateParaRange(Instruction ins, int currentLine)
		{
			// Check Parameter Range
			if (DictionaryDef.paraRange.TryGetValue(ins.opcode, out paraRan))
			{
				DictionaryDef.Boundary currentBoundary = new DictionaryDef.Boundary();

				if (ins.opcode == Instruction.SONG_DEF)
				{
					int i = 0;
					currentBoundary = paraRan.ElementAt(i).Value;
					if ((ins.paramList[i] >= currentBoundary.lowerBoundary)
						&& (ins.paramList[i] <= currentBoundary.upperBoundary))
						i++;
					else
						throw new ParameterRangeInvalidException(currentLine, ins.opcode);
					while (i < ins.paramList.Count)
					{
						if (i % 2 == 0)
							currentBoundary = paraRan.ElementAt(2).Value;
						else
							currentBoundary = paraRan.ElementAt(i % 2).Value;
						if ((ins.paramList[i] >= currentBoundary.lowerBoundary)
							&& (ins.paramList[i] <= currentBoundary.upperBoundary))
						{
							i++;
							continue;
						}
						else
							throw new ParameterRangeInvalidException(currentLine, ins.opcode);
					}
				}
				else
				{
					for (int i = 0; i < paraRan.Count; i++)
					{
						currentBoundary = paraRan.ElementAt(i).Value;
						if (currentBoundary.fixArguValue.Count == 0)
						{
							if ((ins.paramList[i] >= currentBoundary.lowerBoundary)
								&& (ins.paramList[i] <= currentBoundary.upperBoundary))
								continue;
							else
								throw new ParameterRangeInvalidException(currentLine, ins.opcode);
						}
						else
						{
							if (currentBoundary.fixArguValue.Contains(ins.paramList[i]))
								continue;
							else
								throw new ParameterRangeInvalidException(currentLine, ins.opcode);
						}
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Validate a program.
		/// </summary>
		/// <param name="program">program is the input of test program</param>
		/// <returns>True for a valid program, throw exception otherwise</returns>
		public static bool ValidateProgram(HLProgram program)
		{
			currentLine = 0;
			Validator.program = program;

			foreach (Instruction ins in program)
			{
				ValidateInstruction(ins);
			}			
			
			// Not all LOOP END_LOOP get matched.
			if (loopStack.Count != 0) throw new LoopUnmatchedException(currentLine, "End of file.");
			return true;
		}

		/// <summary>
		/// Validate effectiveness and length of parameter of SONG_DEF
		/// </summary>
		/// <param name="songIns">SONG_DEF instruction under validation</param>
		/// <returns>True for a valid program, false otherwise</returns>
		public static bool ValidateSongDef(Instruction songIns)
		{
			int songLength = songIns.paramList.Count - 1;
			if (songLength < 2 || songLength > 32) return false;

			int songNo = songIns.paramList[0];
			if ((songIns.paramList[0] < 0 || songIns.paramList[0] > 15))
			{
				return false;
			}

			return true;

		}
	}


}
