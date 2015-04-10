using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
    public class DictionaryDef
    {
        // define fixed value of some instruction parameter range
        static ArrayList ledBitValue = new ArrayList()
        {
            0, 2, 4, 8
        };
        static ArrayList ifLoopSensorValue = new ArrayList()
        {
            101, 102
        };

        //Create a dictionary for checking length of paramter array
        public static Dictionary<string, int> paraLength = new Dictionary<string, int>()
        {
            //initialize the dictionary
            /*
            { Instruction.FORWARD, 2 },
            { Instruction.BACKWARD, 2 },
            { Instruction.LEFT, 1 },
            { Instruction.RIGHT, 1 },
            */
            { Instruction.MOVE, 2 },
            { Instruction.ROTATE, 1 },
            { Instruction.DRIVE, 2 },
            { Instruction.LED, 3 },
            { Instruction.SONG_PLAY, 1 },
            { Instruction.IF, 3 },
            { Instruction.ELSE, 0 },
            { Instruction.END_IF, 0 },
//          { Instruction.LOOP, 3 },
            { Instruction.END_LOOP, 0 },
            { Instruction.DELAY, 1 },
            { Instruction.READ_SENSOR, 0 },
        };

        public class Boundary
        {
            int flag = 0;
            public int upperBoundary;
            public int lowerBoundary;
            public ArrayList fixArguValue = new ArrayList();

            public Boundary()
            {

            }

            // constructor for initializing calss boundary
            public Boundary(int lowerInput, int upperInput)
            {
                upperBoundary = upperInput;
                lowerBoundary = lowerInput;
            }

            public Boundary(ArrayList arguValue)
            {
                fixArguValue = arguValue;
            }

            public Boundary(int lowerInput, int upperInput, ArrayList arguValue)
            {
                fixArguValue = arguValue;
                for (int i = 0; i <= (upperInput - lowerInput); i++)
                    fixArguValue.Add(i);
            }
        }

        // subdictionary used to store range of each parameter in one instruction
        public static Dictionary<int, Boundary> driveRange = new Dictionary<int, Boundary>()
        {
            { 0, new Boundary(-500, 500) },
            { 1, new Boundary(-2000, 2000) },
        };

        public static Dictionary<int, Boundary> ledRange = new Dictionary<int, Boundary>()
        {
            { 0, new Boundary(ledBitValue) },
            { 1, new Boundary(0, 255) },
            { 2, new Boundary(0, 255) },
        };

        public static Dictionary<int, Boundary> songDefRange = new Dictionary<int, Boundary>()
        {
            { 0, new Boundary(0, 15) },
            { 1, new Boundary(31, 127) },
            { 2, new Boundary(0, 255) },
        };

        public static Dictionary<int, Boundary> songRange = new Dictionary<int, Boundary>()
        {
            { 0, new Boundary(31, 127) },
            { 1, new Boundary(0, 255) },
        };

        public static Dictionary<int, Boundary> songPlayRange = new Dictionary<int, Boundary>()
        {
            { 0, new Boundary(0, 15) },
        };

        public static Dictionary<int, Boundary> ifLoopRange = new Dictionary<int, Boundary>()
        {
            { 0, new Boundary(0, 25, ifLoopSensorValue) },
            { 1, new Boundary(0, 5) },
        };

        // dictionary for each instruction
        public static Dictionary<string, Dictionary<int, Boundary>> paraRange = new Dictionary<string, Dictionary<int, Boundary>>()
        {
            { Instruction.DRIVE, driveRange },
            { Instruction.LED, ledRange },
            { Instruction.SONG_DEF, songDefRange },
            { Instruction.SONG_PLAY, songPlayRange },
            { Instruction.SONG, songRange },
            { Instruction.IF, ifLoopRange },
            { Instruction.LOOP, ifLoopRange },
        };
    }
}