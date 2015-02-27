using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
    public static class Translator
    {
        [Flags]
        public enum SourceType
        {
            Microcontroller,
            Emulator
        };

        private const string MicrocontrollerTemplate = "mc_t.c";
        private const string MicrocontrollerOutputSource = "mc_o.c";
        private const string EmulatorTemplate = "em_t.cpp";
        private const string EmulatorOutputSource = "em_o.cpp";

        public const string FORWARD_SNIPPET = @"
byteTx(CmdDrive);
byteTx(#velo_high);
byteTx(#velo_low);
byteTx(128);
byteTx(0);
while(distance < #distance)
{
    delaySensors(100);
}
byteTx(CmdDrive);
byteTx(0);
byteTx(0);
byteTx(128);
byteTx(0);
";
        public const string LEFT_SNIPPET = @"
byteTx(CmdDrive);
byteTx(0);
byteTx(0);
byteTx(0);
byteTx(1);
while(angle < #angle)
{
    delaySensors(100);
}
byteTx(CmdDrive);
byteTx(0);
byteTx(0);
byteTx(128);
byteTx(0);
";
        public const string DRIVE_SNIPPET = @"
byteTx(CmdDrive);
byteTx(#velo_high);
byteTx(#velo_low);
byteTx(#angle_high);
byteTx(#angle_low);
";
        public const string LED_SNIPPET = @"
byteTx(CmdLeds);
byteTx(#bit);
byteTx(#color);
byteTx(#intensity);
";
        public const string SONG_DEF_SNIPPET = @"
byteTx(CmdSong);
byteTx(#song_number);
byteTx(#song_duration);
";
        public const string SONG_PLAY_SNIPPET = @"
byteTx(CmdPlay);
byteTx(#song_number);
";
        public const string IF_SNIPPET = "if (#condition) {";
        public const string ELSE_SINPPET = "} else {";
        public const string END_IF_SINPPET = "}";
        public const string LOOP_SNIPPET = "while (#condition) {";
        public const string END_LOOP_SNIPPET = @"
byteTx(CmdSensors);
byteTx(0);
}
";
        public const string DELAY_SNIPPET = @"
delay(#time);
";

        public const string PLACEHOLDER_MAIN_PROGRAM = "##main_program##";

        public static string TranslateInstruction(Instruction instruction)
        {
            // C program builder
            StringBuilder cBuilder = new StringBuilder();
            string operatorSymbol;
            string condition;

            switch (instruction.opcode)
            {
                case Instruction.FORWARD:
                    cBuilder.AppendLine(FORWARD_SNIPPET
                        .Replace("#velo_high", (((byte)((instruction.parameters[0] / instruction.parameters[1]) >> 8)) & 0x00FF).ToString())
                        .Replace("#velo_low", ((byte)(instruction.parameters[0] / instruction.parameters[1]) & 0x00FF).ToString())
                        .Replace("#distance", instruction.parameters[0].ToString()));
                    break;

                case Instruction.LEFT:
                    cBuilder.AppendLine(
                        LEFT_SNIPPET.Replace("#angle", instruction.parameters[0].ToString()));
                    break;

                case Instruction.DRIVE:
                    cBuilder.AppendLine(DRIVE_SNIPPET
                        .Replace("#velo_high", ((byte)(instruction.parameters[0] >> 8) & 0x00FF).ToString())
                        .Replace("#velo_low", ((byte)instruction.parameters[0] & 0x00FF).ToString())
                        .Replace("#angle_high", ((byte)(instruction.parameters[1] >> 8) & 0x00FF).ToString())
                        .Replace("#angle_low", ((byte)instruction.parameters[1] & 0x00FF).ToString()));
                    break;

                case Instruction.LED:
                    cBuilder.AppendLine(LED_SNIPPET
                        .Replace("#bit", instruction.parameters[0].ToString())
                        .Replace("#color", instruction.parameters[1].ToString())
                        .Replace("#intensity", instruction.parameters[2].ToString()));
                    break;

                case Instruction.SONG_DEF:
                    cBuilder.AppendLine(SONG_DEF_SNIPPET
                        .Replace("#song_number", instruction.parameters[0].ToString())
                        .Replace("#song_duration", (instruction.parameters.Count / 2).ToString()));
                    for (int i = 1; i < instruction.parameters.Count; i++)
                    {
                        cBuilder.AppendLine("byteTx(" + instruction.parameters[i].ToString() + ");");
                    }
                    break;

                case Instruction.SONG_PLAY:
                    cBuilder.AppendLine(SONG_PLAY_SNIPPET.Replace("#song_number", instruction.parameters[0].ToString()));
                    break;

                case Instruction.IF:
                    operatorSymbol = Instruction.GetOperatorSymbol((byte)(instruction.parameters[1]));
                    condition = "sensors[" + instruction.parameters[0].ToString() + "] " 
                        + operatorSymbol + " " 
                        + instruction.parameters[2].ToString();
                    cBuilder.AppendLine("byteTx(CmdSensors)");
                    cBuilder.AppendLine("byteTx(0)");
                    cBuilder.AppendLine(IF_SNIPPET.Replace("#condition", condition));
                    break;

                case Instruction.ELSE:
                    cBuilder.AppendLine(ELSE_SINPPET);
                    break;

                case Instruction.END_IF:
                    cBuilder.AppendLine(END_IF_SINPPET);
                    break;

                case Instruction.LOOP:
                    operatorSymbol = Instruction.GetOperatorSymbol((byte)(instruction.parameters[1]));
                    condition = "sensors[" + instruction.parameters[0].ToString() + "] "
                        + operatorSymbol + " "
                        + instruction.parameters[2].ToString();
                    cBuilder.AppendLine("byteTx(CmdSensors)");
                    cBuilder.AppendLine("byteTx(0)");
                    cBuilder.AppendLine(LOOP_SNIPPET.Replace("#condition", condition));
                    break;

                case Instruction.END_LOOP:
                    cBuilder.AppendLine(END_LOOP_SNIPPET);
                    break;

                case Instruction.DELAY:
                    cBuilder.AppendLine(DELAY_SNIPPET.Replace("#time", instruction.parameters[0].ToString()));
                    break;
            }
            return cBuilder.ToString();
        }

        public static string TranslateProgram(HLProgram program)
        {

            StringBuilder cBuilder = new StringBuilder();

            foreach (Instruction ins in program.GetInstructionList())
            {
                cBuilder.AppendLine("//" + ins.ToString());
                cBuilder.AppendLine(TranslateInstruction(ins));
            }

            return cBuilder.ToString();
        }

        public static void TranlateProgramAndWrite(HLProgram program)
        {
            string cCode = Translator.TranslateProgram(program);
            GenerateCSource(SourceType.Emulator, cCode);

        }

        public static string TranslateInstructionString(string instructionString)
        {
            return TranslateInstruction(new Instruction(instructionString));
        }

        public static string TranslateProgramString(string programString)
        {
            return TranslateProgram(new HLProgram(programString));
        }

        public static void GenerateCSource(SourceType st, string code)
        {
            string template;
            if (st.HasFlag(SourceType.Microcontroller))
            {
                template = File.ReadAllText(MicrocontrollerTemplate);
                if (!String.IsNullOrEmpty(template))
                {
                    File.WriteAllText(MicrocontrollerOutputSource,
                        template.Replace("##main_program##", code));
                }
            }

            if (st.HasFlag(SourceType.Emulator))
            {
                template = File.ReadAllText(EmulatorTemplate);
                if (!String.IsNullOrEmpty(template))
                {
                    File.WriteAllText(EmulatorOutputSource,
                        template.Replace("##main_program##", code));
                }
            }
           
        }
    }
}

