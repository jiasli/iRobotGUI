using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI.Util
{
    public class ProgramViewModel : List<int>
    {

        #region constructor

        public ProgramViewModel(HLProgram program)
        {
            for (int i = 0; i < program.Count; i++)
            {
                if (program[i].opcode == Instruction.IF)
                {
                    base.Add(i);
                    i = program.FindEndIf(i);
                }
                else if (program[i].opcode == Instruction.LOOP)
                {
                    base.Add(i);
                    i = program.FindEndLoop(i);
                }
                else
                {
                    base.Add(i);
                }
            }   
        }

        #endregion


    }
}
