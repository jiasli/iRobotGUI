using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
	public class InstructionExpection : Exception
	{
		public int Line;
		public string InsStr;

	

		public InstructionExpection(int line, string ins)
		{
			this.Line = line;
			this.InsStr = ins;
		}

	}

	public class InvalidOpcodeException : InstructionExpection
	{
		public InvalidOpcodeException(int line, string ins)
			:base(line, ins)
		{

		}
	}

	public class NonNumericParameterException : InstructionExpection
	{
		public NonNumericParameterException(int line, string ins)
			:base(line, ins)
		{

		}
	}
}
