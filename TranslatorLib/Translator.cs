using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
	/// <summary>
	/// The class translate high level instructions to C language.
	/// Trace to WC_3299
	/// WC_3299: The system shall generate instructions for iRobot in C 
	/// which is then later compiled for deployment on the microcontroller 
	/// using the APIs of iRobot.
	/// </summary>
	public static class Translator
	{
		[Flags]
		public enum SourceType
		{
			Microcontroller,
			Emulator
		};

		private const string MicrocontrollerTemplate   = "mc_t.c";
		private const string MicrocontrollerOutputFile = "mc_o.c";
		private const string EmulatorTemplate          = "em_t.cpp";
		private const string EmulatorOutputFile        = "em_o.cpp";

		//Define C snippet
		public const string FORWARD_BACKWARD_SNIPPET = @"distance = 0;
byteTx(CmdDrive);
byteTx(#velo_high);
byteTx(#velo_low);
byteTx(128);
byteTx(0);
while(distance #operator #distance)
{
	delaySensors(100);
}
byteTx(CmdDrive);
byteTx(0);
byteTx(0);
byteTx(128);
byteTx(0);
";
		public const string LEFT_RIGHT_SNIPPET = @"angle = 0;
byteTx(CmdDrive);
byteTx(0);
byteTx(128);
byteTx(#direction_high);
byteTx(#direction_low);
while(angle #operator #angle)
{
	delaySensors(100);
}
byteTx(CmdDrive);
byteTx(0);
byteTx(0);
byteTx(128);
byteTx(0);";

		public const string DRIVE_SNIPPET = @"byteTx(CmdDrive);
byteTx(#velo_high);
byteTx(#velo_low);
byteTx(#angle_high);
byteTx(#angle_low);";

		public const string LED_SNIPPET = @"byteTx(CmdLeds);
byteTx(#bit);
byteTx(#color);
byteTx(#intensity);";

		public const string SONG_DEF_SNIPPET = @"byteTx(CmdSong);
byteTx(#song_number);
byteTx(#song_duration);";

		public const string SONG_PLAY_SNIPPET = @"byteTx(CmdPlay);
byteTx(#song_number);";

		public const string READ_SENSOR_SNIPPET = @"delaySensors(0);";

		public const string IF_SNIPPET = @"if (#condition)
{";

		public const string ELSE_SINPPET = @"}
else
{";
		public const string END_IF_SINPPET = @"}";

		public const string LOOP_SNIPPET = @"while (#condition)
{";
		public const string END_LOOP_SNIPPET = @"}";
		public const string DELAY_SNIPPET = @"delay(#time);";

		public const string PLACEHOLDER_MAIN_PROGRAM = "##main_program##";


		// Remember not to include linebreak in the end.


		/// <summary>
		/// The function translate DRIVE instruction
		/// </summary>
		/// <param name="ins">DRIVE instruction input</param>
		/// <returns>DRIVE instruction in C language</returns>
		private static string SubTransDrive(Instruction ins)
		{
			StringBuilder driveBuilder = new StringBuilder();
			driveBuilder.Append(DRIVE_SNIPPET
						.Replace("#velo_high", ((byte)(ins.paramList[0] >> 8) & 0x00FF).ToString())
						.Replace("#velo_low", ((byte)ins.paramList[0] & 0x00FF).ToString())
						.Replace("#angle_high", ((byte)(ins.paramList[1] >> 8) & 0x00FF).ToString())
						.Replace("#angle_low", ((byte)ins.paramList[1] & 0x00FF).ToString()));
			return driveBuilder.ToString();
		}

		/// <summary>
		/// The function translate FORWARD and BACKWARD instruction
		/// </summary>
		/// <param name="ins">FORWARD or BACKWARD instruction input</param>
		/// <returns>FORWARD or BACKWARD instruction in C language</returns>
		private static string SubTransForwardBackward(Instruction ins)
		{
			StringBuilder builder = new StringBuilder();
			string command;

			builder.Append(FORWARD_BACKWARD_SNIPPET
				.Replace("#velo_high", (((byte)((ins.paramList[0] / ins.paramList[1]) >> 8)) & 0x00FF).ToString())
				.Replace("#velo_low", ((byte)(ins.paramList[0] / ins.paramList[1]) & 0x00FF).ToString())
				.Replace("#distance", ins.paramList[0].ToString()));
			command = builder.ToString();

			if (ins.paramList[0] > 0)
				command = command.Replace("#operator", "<");
			else
				command = command.Replace("#operator", ">");
/*
			switch(ins.opcode)
			{
				case Instruction.FORWARD:
					command = command.Replace("#operator", "<");
					break;

				case Instruction.BACKWARD:
					command = command.Replace("#operator", ">");
					break;
			}
*/ 
			return command;
		}

		/// <summary>
		/// The function translate LEFT and RIGHT instruction
		/// </summary>
		/// <param name="ins">LEFT or RIGHT instruction input</param>
		/// <returns>LEFT or RIGHT instruction in C language</returns>
		private static string SubTransLeftRight(Instruction ins)
		{
			StringBuilder builder = new StringBuilder();
			String command;

			builder.Append(LEFT_RIGHT_SNIPPET.Replace("#angle", ins.paramList[0].ToString()));
			command = builder.ToString();
			if (ins.paramList[0] > 0)
			{
				command = command.Replace("#direction_high", "0")
						.Replace("#direction_low", "1")
						.Replace("#operator", "<");
			}
			else
			{
				command = command.Replace("#direction_high", "255")
						.Replace("#direction_low", "255")
						.Replace("#operator", ">");
			}
/*
			switch (ins.opcode)
			{
				case Instruction.LEFT:
					command = command.Replace("#direction_high", "0")
						.Replace("#direction_low", "1")
						.Replace("#operator", "<");
					break;

				case Instruction.RIGHT:
					command = command.Replace("#direction_high", "255")
						.Replace("#direction_low", "255")
						.Replace("#operator", ">");

					break;

			}
*/ 
			return command;
		}

		/// <summary>
		/// The function translate the condition part in IF and LOOP instruction
		/// </summary>
		/// <param name="para0">Sensor index in condition</param>
		/// <param name="opSymbol">operator in condition</param>
		/// <param name="para2">Sensor value in condition</param>
		/// <returns>condition in C language</returns>
		private static string SubTransCondition(string para0, string opSymbol, string para2)
		{
			string condition = String.Format("sensors[{0}] {1} {2}", para0, opSymbol, para2);
			return condition;
		}

		/// <summary>
		/// The function translate IF and LOOP instruction
		/// </summary>
		/// <param name="ins">IF or LOOP instruction input</param>
		/// <returns>IF or LOOP instruction in C language</returns>
		private static string SubTransIfLoop(Instruction ins)
		{
			string condition = "Condition Unassigned";
			string operatorSymbol;
			List<int> paramList = ins.paramList;
			StringBuilder builder = new StringBuilder();

			operatorSymbol = Operator.GetOperatorTextSymbol(paramList[1]);

			// To check if the sensor is built-in or compound
			if (Sensor.GetSensorType(paramList[0]) == Sensor.SensorType.BuiltIn)
				condition = SubTransCondition(paramList[0].ToString(), operatorSymbol, paramList[2].ToString());
			else
				condition = SubTransCondition(Sensor.GetCompoundSensorName(paramList[0]), operatorSymbol, paramList[2].ToString());

			if (ins.opcode == Instruction.IF)
				builder.Append(IF_SNIPPET.Replace("#condition", condition));
			else 
				builder.Append(LOOP_SNIPPET.Replace("#condition", condition));

			return builder.ToString();
		}

		/// <summary>
		/// Translate one single instruction.
		/// </summary>
		/// <param name="instruction"></param>
		/// <returns>The instruction string.</returns>
		public static string TranslateInstruction(Instruction instruction)
		{
			// C program builder
			StringBuilder cBuilder = new StringBuilder();
//			string operatorSymbol;
//			string condition = "Condition Unassigned";

			switch (instruction.opcode)
			{
				// Navigation
				// Trace to WC_3303
				// As an ESS, I can drag and drop the built-in functions of the iRobot to control its behavior/movement. 
				case Instruction.MOVE:
					cBuilder.AppendLine(SubTransForwardBackward(instruction));
					break;
				case Instruction.ROTATE:
					cBuilder.AppendLine(SubTransLeftRight(instruction));
					break;
				case Instruction.DRIVE:
					cBuilder.AppendLine(SubTransDrive(instruction));
					break;

				// LED
				// Trace to WC_3291
				// As an ESS, I can use the sounds & light module so that I can turn the LEDs on and off. 
				case Instruction.LED:
					cBuilder.AppendLine(LED_SNIPPET
						.Replace("#bit", instruction.paramList[0].ToString())
						.Replace("#color", instruction.paramList[1].ToString())
						.Replace("#intensity", instruction.paramList[2].ToString()));
					break;

				// SONG
				// Trace to WC_3290
				// As an ESS, I can drag & drop the musical notes from the sounds & light module so that I can create a song. 
				case Instruction.SONG_DEF:
					cBuilder.AppendLine(SONG_DEF_SNIPPET
						.Replace("#song_number", instruction.paramList[0].ToString())
						.Replace("#song_duration", (instruction.paramList.Count / 2).ToString()));
					for (int i = 1; i < instruction.paramList.Count; i++)
					{
						cBuilder.AppendLine("byteTx(" + instruction.paramList[i].ToString() + ");");
					}
					break;
				case Instruction.SONG_PLAY:
					cBuilder.AppendLine(SONG_PLAY_SNIPPET.Replace("#song_number", instruction.paramList[0].ToString()));
					break;

				// DELAY
				// Trace to WC_3296
				// As an ESS, I can drag and drop a wait condition in which I can further drag and drop the instructions/loop constructs.
				case Instruction.DELAY:
					cBuilder.AppendLine(DELAY_SNIPPET.Replace("#time", instruction.paramList[0].ToString()));
					break;

				// SENSOR
				case Instruction.READ_SENSOR:
					cBuilder.AppendLine(READ_SENSOR_SNIPPET);
					break;

				// IF ELSE END_IF
				// Trace to WC_3295
				// As an ESS, I can drag and drop if-then-else and for/while construct in which I can further drag 
				// and drop the instructions/loop constructs. 
				case Instruction.IF:
					cBuilder.AppendLine(SubTransIfLoop(instruction));
					break;
				case Instruction.ELSE:
					cBuilder.AppendLine(ELSE_SINPPET);
					break;
				case Instruction.END_IF:
					cBuilder.AppendLine(END_IF_SINPPET);
					break;

				// LOOP END_LOOP
				// Trace to WC_3295
				// As an ESS, I can drag and drop if-then-else and for/while construct in which I can further drag 
				// and drop the instructions/loop constructs. 
				case Instruction.LOOP:
					cBuilder.AppendLine(SubTransIfLoop(instruction));                    
					break;
				case Instruction.END_LOOP:
					cBuilder.AppendLine(END_LOOP_SNIPPET);
					break;

			}
			return cBuilder.ToString();
		}

		/// <summary>
		/// Translate high-level program
		/// </summary>
		/// <param name="program">High level igp program input</param>
		/// <returns>C code of igp program</returns>
		public static string TranslateProgram(HLProgram program)
		{

			StringBuilder cBuilder = new StringBuilder();

			foreach (Instruction ins in program.GetInstructionList())
			{
				// The instruction itself as comment
				cBuilder.AppendLine("//" + ins.ToString());
				cBuilder.AppendLine(TranslateInstruction(ins));
			}

			return cBuilder.ToString();
		}

		/// <summary>
		/// Translate high-level program string
		/// </summary>
		/// <param name="programString">High level igp program string input</param>
		/// <returns>C code of igp program</returns>
		public static string TranslateProgramString(string programString)
		{
			return TranslateProgram(new HLProgram(programString));
		}

		/// <summary>
		/// This function translate high level igp program Emulator program
		/// </summary>
		/// <param name="program">High level igp program input</param>
		public static void TranlateProgramAndWrite(HLProgram program)
		{
			string cCode = Translator.TranslateProgram(program);
			GenerateCSource(SourceType.Emulator, cCode);

		}

		/// <summary>
		/// The function translate a high level instruction string to C code
		/// </summary>
		/// <param name="instructionString">Instruction string input</param>
		/// <returns>Corresponding C code of instruction string</returns>
		public static string TranslateInstructionString(string instructionString)
		{
			return TranslateInstruction(new Instruction(instructionString));
		}

		/// <summary>
		/// The function put generated C code instruction into C file can be compiled
		/// </summary>
		/// <param name="st">Decide it is a Microcontroller program or an Emulator program</param>
		/// <param name="code">Generated C code instruction</param>
		public static void GenerateCSource(SourceType st, string code)
		{
			string template;
			if (st.HasFlag(SourceType.Microcontroller))
			{
				template = File.ReadAllText(MicrocontrollerTemplate);
				if (!String.IsNullOrEmpty(template))
				{
					File.WriteAllText(MicrocontrollerOutputFile,
						template.Replace("##main_program##", code));
				}
			}

			if (st.HasFlag(SourceType.Emulator))
			{
				template = File.ReadAllText(EmulatorTemplate);
				if (!String.IsNullOrEmpty(template))
				{
					File.WriteAllText(EmulatorOutputFile,
						template.Replace("##main_program##", code));
				}
			}

		}
	}
}

