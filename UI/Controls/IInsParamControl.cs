using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI.Controls
{
    interface IInsParamControl
    {
        Instruction CurrentIns
        {
            get;
            set;
        }

    }
}
