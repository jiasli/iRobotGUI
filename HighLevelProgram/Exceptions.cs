using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
	public class InstructionException : Exception
	{
		public int Line;
		public string InsStr;

		public InstructionException(int line, string ins)
			: base("line " + line + " :" + ins)
		{
			this.Line = line;
			this.InsStr = ins;
		}
	}

	public class IfUnmatchedException : InstructionException
	{
		public IfUnmatchedException(int line, string ins)
			: base(line, ins)
		{
		}
	}

	public class LoopUnmatchedException : InstructionException
	{
		public LoopUnmatchedException(int line, string ins)
			: base(line, ins)
		{
		}
	}

	public class InvalidOpcodeException : InstructionException
	{
		public InvalidOpcodeException(int line, string ins)
			: base(line, ins)
		{

		}
	}

	public class NonNumericParameterException : InstructionException
	{
		public NonNumericParameterException(int line, string ins)
			: base(line, ins)
		{

		}
	}

	public class ParameterCountInvalidException : InstructionException
	{
		public ParameterCountInvalidException(int line, string ins)
			: base(line, ins)
		{

		}
	}

	public class ParameterRangeInvalidException : InstructionException
	{
		public ParameterRangeInvalidException(int line, string ins)
			: base(line, ins)
		{

		}
	}
}
