using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
    public class Instruction
    {
        public string opcode;
        public List<int> parameters;
        public string _string;

        #region OpCode
        public const string FORWARD = "FORWARD";
        public const string BACKWARD = "BACKWARD";
        public const string LEFT = "LEFT";
        public const string RIGHT = "RIGHT";
        public const string LED = "LED";
        public const string SONG_DEF = "SONG_DEF";
        public const string SONG_PLAY = "SONG_PLAY";
        public const string DEMO = "DEMO";
        #endregion

        public readonly string[] OpCodeSet = new string[] 
        { 
            FORWARD,
            BACKWARD, 
            LEFT, 
            RIGHT, 
            LED, 
            SONG_DEF, 
            SONG_PLAY,
            DEMO
        };

        public Instruction(string opcode, string[] parameters)
        {
            setFields(opcode, parameters);
        }

        public Instruction(string instructionString)
        {
            string[] insSplitted = instructionString.Split(new char[] { ' ' });

            setFields(insSplitted[0], insSplitted[1].Split(new char[] { ',' }));
        }


        private void setFields(string opcode, string[] parameters)
        {
            if (OpCodeSet.Contains(opcode))
            {
                // opcode
                this.opcode = opcode;
            }
            else
            {
                throw new InvalidOpcodeException();
            }
            // parameters
            this.parameters = new List<int>();
            foreach (string para in parameters)
            {
                this.parameters.Add(Convert.ToInt32(para));
            }


        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(opcode);
            sb.Append(" ").Append(string.Join(",", parameters));
            return sb.ToString();
        }
    }
}
