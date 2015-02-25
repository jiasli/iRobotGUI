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
        public const string IF = "IF";
        public const string ELSE = "ELSE";
        public const string END_IF = "END_IF";
        public const string LOOP = "LOOP";
        public const string END_LOOP = "END_LOOP";

        #endregion

        public readonly string[] OpCodeSet = new string[] 
        { 
            FORWARD,
            BACKWARD, 
            LEFT, 
            RIGHT, 
            LED, 
            SONG_DEF, 
            SONG_PLAY 
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

        public static string GetOperatorSymbol(string opeartorName)
        {
            switch (opeartorName)
            {
                case "NOT_EQUAL":
                    return "!=";
                case "EQUAL":
                    return "==";
                case "GREATER_THAN":
                    return ">";
                case "GRAETER_THAN_OR_EQUAL":
                    return ">=";
                case "LESS_THAN":
                    return "<";
                case "LESS_THAN_OR_EQUAL":
                    return "<=";
                default:
                    return "";
            }
        }

        public static int GetSensor(string SensorName)
        {

        }
    }
}
