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
		/// <summary>
		/// The default parameter length of current instruction under validation 
		/// </summary>
		static int paraLen = 0;

		static private Stack<string> ifStack = new Stack<string>();
		static private Stack<string> loopStack = new Stack<string>();
		
		/// <summary>
		/// The function check matching situation of instruction IF, ELSE & END_IF
		/// </summary>
		/// <param name="opcode">
		/// Opcode of current instruction under validation, should be one of IF, ELSE or END_IF
		/// </param>
		/// <returns>
		/// Return true when they are matched, throw IfUnmatchedException otherwise
		/// </returns>
		public static bool CheckIf(string opcode)
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
					return true;
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
					return true;
				}
				else if (opcode == ELSE)
				{
					// Only IF on top of stack can accept ELSE
					if (top == IF) { ifStack.Pop(); ifStack.Push(opcode); }
					throw new IfUnmatchedException(currentLine, opcode);

				}
				else if (opcode == END_IF)
				{
					// Only ELSE on top of stack can accept END_IF
					if (top == ELSE) { ifStack.Pop(); }
					throw new IfUnmatchedException(currentLine, opcode);
				}
			}
			return true;

		}

		/// <summary>
		/// The function check matching situation of instruction LOOP & END_LOOP
		/// </summary>
		/// <param name="opcode">
		/// Opcode of current instruction under validation, should be either LOOP or END_LOOP
		/// </param>
		/// <returns>
		/// Return true when they are matched, throw LoopUnmatchedException otherwise
		/// </returns>
		public static bool CheckLoop(string opcode)
		{
			if (opcode == Instruction.LOOP)
			{
				// Accept any LOOP.
				loopStack.Push(Instruction.LOOP);
				return true;
			}
			else if (opcode == Instruction.END_LOOP)
			{
				// Only accept END_LOOP when the stack is non-empty.
				if (loopStack.Count == 0)
					throw new LoopUnmatchedException(currentLine, opcode);
				else
				{
					loopStack.Pop();
					return true;
				}
			}
			return true;
		}

		/// <summary>
		/// The function transfer a test instruction string to Instruction and validate it
		/// </summary>
		/// <param name="insStr">
        /// insStr is the input of test string
        /// </param>
		/// <returns>
        /// True for a valid instuction or test, throw exception otherwise
        /// </returns>
		public static bool ValidateInstruction(String insStr)
		{
			Instruction ins;
			try
			{
				ins = new Instruction(insStr);
			}
			catch (InvalidOpcodeException ex)
			{
				throw ex;
			}

			return ValidateInstruction(ins);
		}

        /// <summary>
        /// The function validate an instruction
        /// </summary>
        /// <param name="ins">
        /// Instruction under validate
        /// </param>
        /// <returns>
        /// True for a valid instuction or test, throw exception otherwise
        /// </returns>
		public static bool ValidateInstruction(Instruction ins)
		{
			currentLine++;

            //Check Parameter Count
			if (DictionaryDef.paraLength.TryGetValue(ins.opcode, out paraLen))
			{
				if (ins.paramList.Count != paraLen)
					throw new ParameterLengthException(currentLine, ins.opcode);
			}

			switch (ins.opcode)
			{
                //Validate effectiveness and length of parameter of SONG_DEF
				case Instruction.SONG_DEF:
					return ValidateSongDef(ins);

                //check matching situation of instruction LOOP & END_LOOP
				case Instruction.LOOP:
					return CheckLoop(ins.opcode);

				case Instruction.END_LOOP:
					return CheckLoop(ins.opcode);

                //check matching situation of instruction IF ELSE & END_IF
				case Instruction.IF:
				case Instruction.ELSE:
				case Instruction.END_IF:
					return CheckIf(ins.opcode);
			}
			return true;
		}

        /// <summary>
        /// The function transfer a test program string to Instruction and validate it
        /// </summary>
        /// <param name="programString">
        /// programString is the input of test program string
        /// </param>
        /// <returns>
        /// True for a valid program, throw exception otherwise
        /// </returns>
		public static bool ValidateProgram(string programString)
		{
			return ValidateProgram(new HLProgram(programString));
		}

        /// <summary>
        /// The function validate a program
        /// </summary>
        /// <param name="program">
        /// program is the input of test program
        /// </param>
        /// <returns>
        /// True for a valid program, throw exception otherwise
        /// </returns>
		public static bool ValidateProgram(HLProgram program)
		{
			foreach (Instruction ins in program)
			{
				if (ValidateInstruction(ins) == false) return false;
			}

			// Not all IF ELSE END_IF get matched.
			if (ifStack.Count != 0) throw new IfUnmatchedException(currentLine, "End of file.");
			
			// Not all LOOP END_LOOP get matched.
			if (loopStack.Count != 0) throw new LoopUnmatchedException(currentLine, "End of file.");
			return true;
		}

        /// <summary>
        /// Validate effectiveness and length of parameter of SONG_DEF
        /// </summary>
        /// <param name="songIns">
        /// SONG_DEF instruction under validation
        /// </param>
        /// <returns>
        /// True for a valid program, false otherwise
        /// </returns>
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
