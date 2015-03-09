using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
	/// <summary>
	/// High-level program
	/// </summary>
	public class HLProgram
	{
		public static int CurrentLine;

		private List<Instruction> _program;

		public const string driveExample =@"FORWARD 10,3";

		public const string driveExampleSerial =@"137 0 10 127 255
137 0 0 127 255";

		public HLProgram()
		{
			_program = new List<Instruction>();
		}

		public HLProgram(String programString)
		{
			_program = new List<Instruction>();

			string[] insStrArray = programString.Split(new char[] { '\n','\r' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < insStrArray.Length; i++ )
			{
				CurrentLine = i;

				// Ignore comment line
				if (!Instruction.IsCommentLine(insStrArray[i]))
					_program.Add(new Instruction(insStrArray[i]));
			}
		}

		public List<Instruction> GetInstructionList()
		{
			return _program;
		}

		public void Add(Instruction ins)
		{
			_program.Add(ins);
		}

		public void Add(String ins)
		{
			_program.Add(new Instruction(ins));
		}

		public override string ToString()
		{            
			return string.Join("\n", _program);
		}

		public void Rearrange(Instruction ins, int index)
		{
			_program.Remove(ins);
			_program.Insert(index, ins);
		}
	}
}
