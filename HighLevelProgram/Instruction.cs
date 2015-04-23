using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iRobotGUI
{
	// Related classes:
	// Translator
	// Validator
	// DialogInvoker
	// InstructionPicture
	// InstructionPanel's Tags

	/// <summary>
	/// High-level Instruction.
	/// </summary>
	public class Instruction
	{
		public string opcode;
		public List<int> paramList;

		public readonly string[] OpCodeSet = new string[] 
		{ 
			/*
			FORWARD,
			BACKWARD, 
			LEFT, 
			RIGHT, 
			*/ 
			MOVE,
			ROTATE,
			DRIVE,
			LED, 
			DEMO,
			SONG,
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
		/*
		public const string FORWARD     = "FORWARD";
		public const string BACKWARD    = "BACKWARD";
		public const string LEFT        = "LEFT";
		public const string RIGHT       = "RIGHT";
		*/
		public const string MOVE = "MOVE";
		public const string ROTATE = "ROTATE";
		public const string DRIVE = "DRIVE";

		// LED Song
		public const string LED = "LED";
		public const string SONG = "SONG";
		public const string SONG_DEF = "SONG_DEF";
		public const string SONG_PLAY = "SONG_PLAY";

		// IF LOOP
		public const string IF = "IF";
		public const string ELSE = "ELSE";
		public const string END_IF = "END_IF";
		public const string LOOP = "LOOP";
		public const string END_LOOP = "END_LOOP";

		// Other
		public const string DELAY = "DELAY";
		public const string READ_SENSOR = "READ_SENSOR";
		public const string DEMO = "DEMO";

		#endregion


		#region Constants

		public const int STRAIGHT1 = 0x7FFF;	// 32767
		public const int STRAIGHT2 = 0x8000;	// 32768
		public const int TURN_IN_PLACE_CLOCKWISE = 0xFFFF;
		public const int TURN_IN_PLACE_COUNTER_CLOCKWISE = 0x0001;

		#endregion


		/// <summary>
		/// To construct the Instruction. The instruction string must be unified using UnifyInstructionString first.
		/// </summary>
		/// <param name="insStr"></param>
		public Instruction(string insStr)
		{
			// The string is already unified.

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
		public static Instruction CreatDefaultFromOpcode(string opcode)
		{
			Instruction newIns = null;
			switch (opcode)
			{
				case MOVE:
					newIns = new Instruction(Instruction.MOVE + " 1500,3");
					break;
				case ROTATE:
					newIns = new Instruction(Instruction.ROTATE + " 90");
					break;
				case DRIVE:
					newIns = new Instruction(DRIVE + " 100,32767");
					break;
				case Instruction.LED:
					newIns = new Instruction(Instruction.LED + " 10,128,128");
					break;
				case SONG:
					newIns = new Instruction(SONG);
					break;
				case SONG_DEF:
					// SONG_DEF with the song number
					newIns = new Instruction(SONG_DEF + " 0");
					break;
				case SONG_PLAY:
					newIns = new Instruction(SONG_PLAY + " 0");
					break;
				case Instruction.DEMO:
					newIns = new Instruction(Instruction.DEMO + " 0");
					break;
				case DELAY:
					newIns = new Instruction(DELAY + " 3000");
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
		/// Unify the string. Remove the comment and trim the leading and trailing empty characters.
		/// 
		/// Comment is prefixed by "//" like C, which can be put in a single line or at the end of
		/// the line.
		/// 
		/// Empty line contains only '\t' and ' '.
		/// </summary>
		/// <param name="insStr">The string to unify.</param>
		/// <returns>Unified instruction string.</returns>
		public static string UnifyInstructionString(string insStr)
		{
			// Remove comment
			// https://msdn.microsoft.com/en-us/library/az24scfc(v=vs.110).aspx
			// This pattern matches all substrings starting with // and all following chars.
			string commentPattern = "//.*";
			Regex rgx = new Regex(commentPattern);
			insStr = rgx.Replace(insStr, string.Empty);

			// Trim the '\t' and ' '
			insStr = insStr.Trim(new char[] { '\t', ' ' });

			return insStr;
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

		/// <summary>
		/// Get the instruction string with the description.
		/// </summary>
		/// <param name="withDescription">If to have the description attached as comment.</param>
		/// <returns></returns>
		public string ToString(bool withDescription)
		{
			if (withDescription)
				return this.ToString() + "  // " + this.GetTextDescription();
			else return this.ToString();
		}

		/// <summary>
		/// Get the human-readable text description of the Instruction.
		/// </summary>
		/// <returns></returns>
		public string GetTextDescription()
		{
			return TextDescription.GetTextDescription(this);
		}
	}
}
