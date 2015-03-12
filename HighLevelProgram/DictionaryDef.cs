using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
    public class DictionaryDef
    {
        static int paraIndex = -1;

        //Create a dictionary for checking length of paramter array
        public static Dictionary<string, int> paraLength = new Dictionary<string, int>()
        {
            //initialize the dictionary
            { Instruction.FORWARD, 2 },
            { Instruction.BACKWARD, 2 },
            { Instruction.LEFT, 1 },
            { Instruction.RIGHT, 1 },
            { Instruction.DRIVE, 2 },
            { Instruction.LED, 3 },
            { Instruction.SONG_PLAY, 1 },
            { Instruction.IF, 3 },
            { Instruction.ELSE, 0 },
            { Instruction.END_IF, 0 },
            { Instruction.LOOP, 3 },
            { Instruction.END_LOOP, 0 },
            { Instruction.DELAY, 1 },
            { Instruction.READ_SENSOR, 0 },
        };

        public class Boundary
        {
            int upperBoundary;
            int lowerBoundary;

            public Boundary(int lowerInput, int upperInput)
            {
                upperBoundary = upperInput;
                lowerBoundary = lowerInput;
            }
        }

        public static Dictionary<int, Boundary> driveRange = new Dictionary<int, Boundary>()
        {
            { paraIndex++, new Boundary(-500, 500) },
            { paraIndex++, new Boundary(-2000, 2000) },
        };

        public static Dictionary<string, Dictionary<int, Boundary>> paraRange = new Dictionary<string, Dictionary<int, Boundary>>();


        private static void SetDefault()
        {
            paraIndex = -1;
        }
    }
}
