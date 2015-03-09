using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
    public class StackCheck
    {
        public static Stack IfStack = new Stack();
        public static Stack LoopStack = new Stack();
        public static int IfAmount = 0;
        public static int ElseAmount = 0;
        public static int EndIfAmount = 0;
        public static int LoopAmount = 0;
        public static int EndLoopAmount = 0;
    }
}
