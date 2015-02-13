using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace iRobotPrototypeWpf
{
    public class WinAvrConnector
    {
        public static Configuation config= new Configuation();

        public static void CustomizeMakefile()
        {
             string makefile = File.ReadAllText("makefile_template");
            makefile = makefile.Replace("{COM}", config.comPort);
            makefile = makefile.Replace("{FIRMWARE_VERSION}", config.firmwareVersion);
            File.WriteAllText("makefile", makefile);
        }

        public static void Make()
        {
            CustomizeMakefile();
            string d = Directory.GetCurrentDirectory();
            System.Diagnostics.Process.Start("make", "all");
        }

        public static void Load()
        {         
            //string d = Directory.GetCurrentDirectory();

            CustomizeMakefile();
            System.Diagnostics.Process.Start("make","program");
        }

        public static void Clean()
        {
            System.Diagnostics.Process.Start("make", "clean");
        }
    }
}
