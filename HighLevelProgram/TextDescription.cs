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
				return string.Format("Turn right by {0} degree(s).", -ins.paramList[0]);
			else if (ins.paramList[0] > 0)
				return string.Format("Turn left by {0} degree(s).", ins.paramList[0]);
			else if (ins.paramList[0] == 0)
				return "Stop rotating.";

			// For return check.
			return "";
		}

		private static string GetDriveDescription(Instruction ins)
		{
			if (ins.paramList[0] < 0 && ins.paramList[1] == 32767)
				return string.Format("Drive forward straightly at {0}mm/s.", -ins.paramList[0]);
			else if (ins.paramList[0] > 0 && ins.paramList[1] == 32767)
				return string.Format("Drive backward straightly at {0}mm/s.", ins.paramList[0]);
			else if (ins.paramList[0] < 0 && ins.paramList[1] < 0)
				return string.Format("Drive backward while turning right at {0}mm/s with radius {1}mm.", -ins.paramList[0], -ins.paramList[1]);
			else if (ins.paramList[0] < 0 && ins.paramList[1] > 0)
				return string.Format("Drive backward while turning left at {0}mm/s with radius {1}mm.", ins.paramList[0], ins.paramList[1]);
			else if (ins.paramList[0] > 0 && ins.paramList[1] < 0)
				return string.Format("Drive forward while turning right at {0}mm/s with radius {1}mm.", -ins.paramList[0], -ins.paramList[1]);
			else if (ins.paramList[0] > 0 && ins.paramList[1] > 0)
				return string.Format("Drive forward while turning left at {0}mm/s with radius {1}mm.", ins.paramList[0], ins.paramList[1]);
			else if (ins.paramList[0] == 0)
				return "Stop driving.";

			// For return check.
			return "";
		}

		private static string GetLedDescription(Instruction ins)
		{
			if (ins.paramList[0] == 0)
				return string.Format("Turn off both Play LED and Advance LED.");
			else if (ins.paramList[0] == 2)
				return string.Format("Turn on Play LED.");
			else if (ins.paramList[0] == 8)
				return "Turn on Advance LED.";
			else if (ins.paramList[0] == 10)
				return "Turn on both Play LED and Advance LED.";

			// For return check.
			return "";
		}

		private static string GetSongDescription(Instruction ins)
		{
			return string.Format("Song");
		}

		private static string GetIfDescription(Instruction ins)
		{
			return string.Format("If");
		}

		private static string GetLoopDescription(Instruction ins)
		{
			return string.Format("Loop");
		}

		private static string GetDemoDescription(Instruction ins)
		{
			switch (ins.paramList[0])
			{
				case 0:
					return string.Format("Play demo Cover");
				case 1:
					return string.Format("Play demo Cover and Dock");
				case 2:
					return string.Format("Play demo Spot Cover");
				case 3:
					return string.Format("Play demo Mouse");
				case 4:
					return string.Format("Play demo Drive Figure Eight");
				case 5:
					return string.Format("Play demo Wimp");
				case 6:
					return string.Format("Play demo Home");
				case 7:
					return string.Format("Play demo Tag");
				case 8:
					return string.Format("Play demo Pachelbel");
				case 9:
					return string.Format("Play demo Banjo");
				case -1:
					return string.Format("Stop the demo");
			}

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
