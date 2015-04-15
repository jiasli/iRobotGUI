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
				return string.Format("Move backward {0}cm in {1}s.", -ins.paramList[0]/10, ins.paramList[1]);
			else if (ins.paramList[0] > 0)
				return string.Format("Move forward {0}cm in {1}s.", ins.paramList[0]/10, ins.paramList[1]);
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
			string direction = System.String.Empty;
			string rotation = System.String.Empty;

			if (ins.paramList[0] < 0)
				direction = "backward ";
			else if (ins.paramList[0] > 0)
				direction = "forward ";
			else
				return string.Format("Stop driving");

			if (ins.paramList[1] == 32767)
				rotation = "straightly ";
			else if (ins.paramList[1]  > 0)
				rotation = "while turning left ";
			else if (ins.paramList[1] < 0)
				rotation = "while turning right ";

			return string.Format("Drive " + direction + "at {0}cm/s " + rotation, Math.Abs(ins.paramList[0])/10);
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
			return string.Format("Play a song.");
		}

		private static string GetIfDescription(Instruction ins)
		{
			string[] sensors = new string[] { "Bump", "Wall", "Cliff Left", "Cliff Front Left", "Cliff Front Right", "Cliff Right", "Virtual Wall" };
			
			if (ins.paramList[0] < 10)
				return string.Format("If the sensor " + sensors[ins.paramList[0]] + " is detected...");
			else
				return string.Format("If the sensor Charging State is detected...");
		}

		private static string GetLoopDescription(Instruction ins)
		{
			string[] sensors = new string[] { "Bump", "Wall", "Cliff Left", "Cliff Front Left", "Cliff Front Right", "Cliff Right", "Virtual Wall"};
			
			if (ins.paramList[0] < 10)
				return string.Format("Loop when the sensor " + sensors[ins.paramList[0]] + " is not detected...");
			else
				return string.Format("Loop when the sensor Charging State is not detected...");
		}

		private static string GetDemoDescription(Instruction ins)
		{
			string[] demoName = new string[] { "Cover", "Cover and Dock", "Spot Cover", "Mouse", "Drive Figure Eight", "Wimp", "Home", "Tag", "Pachelbel", "Banjo" };
			if (ins.paramList[0] >= 0)
			{
				return string.Format("Play the demo " + demoName[ins.paramList[0]] + ".");
			}
			else
			{
				return string.Format("Stop the demo.");
			}
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
