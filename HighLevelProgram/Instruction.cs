using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
	public class Instruction
	{
		public string opcode;
		public List<int> paramList;

		public readonly string[] OpCodeSet = new string[] 
		{ 
			FORWARD,
			BACKWARD, 
			LEFT, 
			RIGHT, 
			DRIVE,
			LED, 
			DEMO,
			SONG_DEF,             
			SONG_PLAY, 
			IF, 
			ELSE,
			END_IF,
			LOOP,
			END_LOOP, 
			DELAY,
			READ_SENSOR
		};		

		#region OpCode

		// Navigation
		public const string FORWARD     = "FORWARD";
		public const string BACKWARD    = "BACKWARD";
		public const string LEFT        = "LEFT";
		public const string RIGHT       = "RIGHT";
		public const string DRIVE       = "DRIVE";		

		// LED Song
		public const string LED         = "LED";		
		public const string SONG_DEF    = "SONG_DEF";
		public const string SONG_PLAY   = "SONG_PLAY";

		// IF LOOP
		public const string IF          = "IF";
		public const string ELSE        = "ELSE";
		public const string END_IF      = "END_IF";
		public const string LOOP        = "LOOP";
		public const string END_LOOP    = "END_LOOP";

		// Other
		public const string DELAY       = "DELAY";
		public const string READ_SENSOR = "READ_SENSOR";
		public const string DEMO        = "DEMO";

		#endregion


		#region Constants

		public const int SRAIGHT = 0x8000;	// 32768
		public const int TURN_IN_PLACE_CLOCKWISE = 0xFFFF;
		public const int TURN_IN_PLACE_COUNTER_CLOCKWISE = 0x0001;

		#endregion

		

		public Instruction(string insStr)
		{
			// Remove leading indent.
			insStr = insStr.Trim(new char[] { ' ', '\t' });			

			string opcode;
			string[] paramArray;

			// Seperate the string using the first space ' '.
			int spaceIndex = insStr.IndexOf(' ');
			if (spaceIndex != -1)
			{
				// A space is found.
				opcode = insStr.Substring(0, spaceIndex);
				string param = insStr.Substring(spaceIndex + 1);
				paramArray = param.Split(new char[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			}
			else
			{
				// No space is found, only the opcode.
				opcode = insStr;
				paramArray = new string[0];
			}
			

			if (OpCodeSet.Contains(opcode))
			{
				// opcode
				this.opcode = opcode;
			}
			else
			{
				throw new InvalidOpcodeException(HLProgram.CurrentLine, opcode);
			}

			// parameters
			this.paramList = new List<int>();

			for (int i = 0; i < paramArray.Length; i++)
			{
				int paramInt;
				if (Int32.TryParse(paramArray[i], out paramInt) == false)
				{
					throw new NonNumericParameterException(HLProgram.CurrentLine, insStr);
				}
				this.paramList.Add(Convert.ToInt32(paramInt));
			}

		}

		/// <summary>
		/// A factory to create new Instruction object by opcode with default parameters.
		/// </summary>
		/// <returns></returns>
		public static Instruction CreatFromOpcode(string opcode)
		{
			Instruction newIns = null;
			switch (opcode)
			{
				case FORWARD:
					newIns = new Instruction(Instruction.FORWARD + " 500,3");
					break;
				case Instruction.LEFT:
					newIns = new Instruction(Instruction.LEFT + " 90");
					break;
				case Instruction.LED:
					newIns = new Instruction(Instruction.LED + " 10,128,128");
					break;
				case Instruction.SONG_DEF:
					newIns = new Instruction(Instruction.SONG_DEF + " 0");
					break;
				case Instruction.DEMO:
					newIns = new Instruction(Instruction.DEMO + " 0");
					break;
				case IF:
					newIns = new Instruction(IF + " 0, 0, 0");
					break;
				case ELSE:
					newIns = new Instruction(ELSE);
					break;
				case END_IF:
					newIns = new Instruction(END_IF);
					break;			
				case LOOP:
					newIns = new Instruction(LOOP + " 0, 0, 0");
					break;
				case END_LOOP:
					newIns = new Instruction(END_LOOP);
					break;
			}
			return newIns;
		}



		/// <summary>
		/// Decide if an instruction string is a valid instruction.
		/// Comment line is prefixed by "//" like C.
		/// Empty line contains only '\t' and ' '.
		/// </summary>
		/// <param name="insStr"></param>
		/// <returns>True if it is.</returns>
		public static bool IsInstructionLine(string insStr)
		{
			insStr = insStr.Trim();

			// Empty line
			if (insStr.Length == 0) return false;

			// Command line
			if (insStr[0] == '/') return false;
			else return true;
		}

		/// <summary>
		/// Get the program string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(opcode);
			if (paramList != null)
				sb.Append(" ").Append(string.Join(",", paramList));
			return sb.ToString();
		}
	}
}
