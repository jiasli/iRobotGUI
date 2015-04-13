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

		private static string GetRotateDescription(Instruction ins)
		{
			if (ins.paramList[0] < 0)
				return string.Format("Rotate {0}degree(s) to the right.", -ins.paramList[0]);
			else if (ins.paramList[0] > 0)
				return string.Format("Rotate {0}degree(s) to the left.", ins.paramList[0]);
			else if (ins.paramList[0] == 0)
				return "Stop rotating.";

			// For return check.
			return "";
		}

		private static string GetDriveDescription(Instruction ins)
		{
			if (ins.paramList[0] < 0 && ins.paramList[1] < 0)
				return string.Format("Drive backward while turning right at {0}mm/s with radius {1}mm.", -ins.paramList[0], -ins.paramList[1]);
			else if (ins.paramList[0] < 0 && ins.paramList[1] > 0)
				return string.Format("Drive backward while turning left at {0}mm/s  with radius {1}mm.", ins.paramList[0], ins.paramList[1]);
			else if (ins.paramList[0] > 0 && ins.paramList[1] < 0)
				return string.Format("Drive forward while turning right at {0}mm/s  with radius {1}mm.", -ins.paramList[0], -ins.paramList[1]);
			else if (ins.paramList[0] > 0 && ins.paramList[1] > 0)
				return string.Format("Drive forward while turning left at {0}mm/s  with radius {1}mm.", ins.paramList[0], ins.paramList[1]);
			else if (ins.paramList[0] == 0)
				return "Stop driving.";

			// For return check.
			return "";
		}

		private static string GetLedDescription(Instruction ins)
		{
			// For return check.
			return "";
		}

		private static string GetSongDescription(Instruction ins)
		{
			// For return check.
			return "";
		}

		private static string GetIfDescription(Instruction ins)
		{
			// For return check.
			return "";
		}

		private static string GetLoopDescription(Instruction ins)
		{
			// For return check.
			return "";
		}

		private static string GetDemoDescription(Instruction ins)
		{
			// For return check.
			return "";
		}

		private static string GetDelayDescription(Instruction ins)
		{
			return string.Format("Delay for {0}ms.", ins.paramList[0]);
		}

		/// <summary>
		/// Get the text description for the spefified Instruction.
		/// </summary>
		/// <param name="ins"></param>
		/// <returns></returns>
		public static string GetTextDescription(Instruction ins)
		{
			//StringBuilder sb = new StringBuilder();
			switch (ins.opcode)
			{
				case Instruction.MOVE:
					return GetMoveDescription(ins);
				case Instruction.ROTATE:
					return GetRotateDescription(ins);
				case Instruction.DRIVE:
					return GetDriveDescription(ins);
				case Instruction.LED:
					return GetLedDescription(ins);
				case Instruction.SONG:
					return GetSongDescription(ins);
				case Instruction.IF:
					return GetIfDescription(ins);
				case Instruction.LOOP:
					return GetLoopDescription(ins);
				case Instruction.DEMO:
					return GetDemoDescription(ins);
				case Instruction.DELAY:
					return GetDelayDescription(ins);
				default:
					return "Description not implemented.";
			}

		}
	}
}
