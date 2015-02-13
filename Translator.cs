using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotPrototypeWpf
{
    public class Translator
    {
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
byteTx(1);
byteTx(#note_number);
byteTx(#note_duration);
";
        public const string SONG_PLAY_SNIPPET = @"
byteTx(CmdPlay);
byteTx(#song_number);
";

        public const string PLACEHOLDER_MAIN_PROGRAM = "##main_program##";

        public static string Translate(Program program)
        {
            CodeGenerator cg = new CodeGenerator();
            StringBuilder cBuilder = new StringBuilder();

            foreach (Instruction ins in program.GetInstructionList())
            {
                cBuilder.AppendLine("//" + ins.ToString());

                switch (ins.opcode)
                {
                    case Instruction.FORWARD:
                        cBuilder.AppendLine(FORWARD_SNIPPET
                            .Replace("#velocity", (ins.parameters[0] / ins.parameters[1]).ToString())
                            .Replace("#distance", ins.parameters[0].ToString()));
                        break;

                    case Instruction.LEFT:
                        cBuilder.AppendLine(
                            LEFT_SNIPPET.Replace("#angle", ins.parameters[0].ToString()));
                        break;

                    case Instruction.LED:
                        cBuilder.AppendLine(LED_SNIPPET
                            .Replace("#bit", ins.parameters[0].ToString())
                            .Replace("#color", ins.parameters[1].ToString())
                            .Replace("#intensity", ins.parameters[2].ToString()));
                        break;

                    case Instruction.SONG_DEF:
                        cBuilder.AppendLine(SONG_DEF_SNIPPET
                            .Replace("#song_number", ins.parameters[0].ToString())
                            .Replace("#note_number", ins.parameters[1].ToString())
                            .Replace("#note_duration", ins.parameters[2].ToString()));
                        break;

                    case Instruction.SONG_PLAY:
                        cBuilder.AppendLine(SONG_DEF_SNIPPET.Replace("#song_number", ins.parameters[0].ToString()));
                        break;


                    //case Instruction.BACKWARD:
                    //    cg.AddByte("137");
                    //    cg.AddByte("0");
                    //    cg.AddByte(Math.Round(-(double)ins._parameters[0] / ins._parameters[1]).ToString());
                    //    cg.AddByte("127");
                    //    cg.AddByte("255");
                    //    break;
                }
            }

            return cBuilder.ToString();
            //return cg.ToCode();
        }
    }
}
