using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI.Util
{
	/// <summary>
	/// Gets the picture name of an instruction.
	/// </summary>
	public static class InstructionPicture
	{
		/// <summary>
		/// Gets the picture name of an instruction.
		/// </summary>
		/// <param name="ins"></param>
		/// <returns></returns>
		public static string GetPictureName(Instruction ins)
		{
			// TODO use correct picture
			switch (ins.opcode)
			{
				// TODO picture for delay
				case Instruction.MOVE:
					return "forward.png";
				case Instruction.ROTATE:
					return "backward.png";
				case Instruction.DRIVE:
					return "drive.png";
				case Instruction.LED:
					return "led.jpg";
				case Instruction.SONG_DEF:
					return "song.png";
				case Instruction.DEMO:
					return "demo.jpg";
				case Instruction.IF:
					return "if.png";
				case Instruction.LOOP:
					return "loop.png";
			}

			// TODO return an picture name for invalid/unimplemented ins 
			return null;
		}
	}
}
