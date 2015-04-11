using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
	/// <summary>
	/// This class generates the text description for an <see cref="Instruction"/>.
	/// </summary>
	/// <remarks>
	/// The implementation of this class is similar to that of the Translator in TranslatorLib,
	/// since it translates the instruction to human-readable format instead of C code.
	/// </remarks>
	public static class TextDescription
	{

		private static string GetMoveDescription(Instruction ins)
		{
			if (ins.paramList[0] < 0)
				return string.Format("Move backward {0}cm in {1}s.", -ins.paramList[0], ins.paramList[1]);
			else if (ins.paramList[0] > 0)
				return string.Format("Move forward {0}cm in {1}s.", ins.paramList[0], ins.paramList[1]);
			else if (ins.paramList[0] == 0)
				return "Stop moving.";

			// For return check.
			return "";
		}

		/// <summary>
		/// Get the text description for the spefified Instruction.
		/// </summary>
		/// <param name="ins"></param>
		/// <returns></returns>
		public static string GetTextDescription(Instruction ins)
		{
			StringBuilder sb = new StringBuilder();
			switch (ins.opcode)
			{
				case Instruction.MOVE:
					return GetMoveDescription(ins);
				default:
					return "Description not implemented.";
			}

		}
	}
}
