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
	public static class PictureDiscription
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
					if (ins.paramList[0] > 0) return "move_forward.png";
					else if (ins.paramList[0] < 0) return "move_backward.png";
					else return "stop.png";
				case Instruction.ROTATE:
					if (ins.paramList[0] > 0) return "rotate_left.png";
					else if (ins.paramList[0] < 0) return "rotate_right.png";
					else return "stop.png";
				case Instruction.DRIVE:
					if (ins.paramList[0] != 0) return "drive.png";
					return "stop.png";
				case Instruction.LED:
					return "led.png";
				case Instruction.SONG:
					return "song.png";
				case Instruction.DEMO:
					return "demo.png";
				case Instruction.DELAY:
					return "delay.png";
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
