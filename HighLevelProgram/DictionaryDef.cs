using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
    public class DictionaryDef
    {
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
    
    }
}
