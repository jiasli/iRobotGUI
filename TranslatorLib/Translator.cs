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
distance = 0;
drive(#velocity, RadStraight);
while(distance < #distance)
{
    delaySensors(100);
}
drive(0, RadStraight);	
";
        public const string LEFT_SNIPPET = @"
angle = 0;
drive(200, RadCCW);
while(angle < #angle)
{
    delaySensors(100);
}
drive(0, RadStraight);  
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

        public const string PLACEHOLDER_MAIN_PROGRAM = "##main_program##";

        public static string TranslateInstruction(Instruction instruction)
        {
            // C program builder
            StringBuilder cBuilder = new StringBuilder();

            switch (instruction.opcode)
            {
                case Instruction.FORWARD:
                    cBuilder.AppendLine(FORWARD_SNIPPET
                        .Replace("#velocity", (instruction.parameters[0] / instruction.parameters[1]).ToString())
                        .Replace("#distance", instruction.parameters[0].ToString()));
                    break;

                case Instruction.LEFT:
                    cBuilder.AppendLine(
                        LEFT_SNIPPET.Replace("#angle", instruction.parameters[0].ToString()));
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


                    //                            .Replace("#note_duration", ins.parameters[2].ToString()));
                    break;

                case Instruction.SONG_PLAY:
                    cBuilder.AppendLine(SONG_PLAY_SNIPPET.Replace("#song_number", instruction.parameters[0].ToString()));
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
            WriteSource(SourceType.Emulator, cCode);

        }

        public static string TranslateInstructionString(string instructionString)
        {
            return TranslateInstruction(new Instruction(instructionString));
        }

        public static string TranslateProgramString(string programString)
        {
            return TranslateProgram(new HLProgram(programString));
        }

        public static void WriteSource(SourceType st, string code)
        {
            string template;
            if (st == SourceType.Microcontroller)
            {
                template = File.ReadAllText(MicrocontrollerTemplate);
                if (!String.IsNullOrEmpty(template))
                {
                    File.WriteAllText(MicrocontrollerOutputSource,
                        template.Replace("##main_program##", code));
                }
            }
            else
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

