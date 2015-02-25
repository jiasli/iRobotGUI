using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iRobotGUI;

namespace TranslatorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists(args[0]))
            {
                string input_program = File.ReadAllText(args[0]);
                string output_program = Translator.TranslateProgramString(input_program);
            }
            else
            {
                Console.WriteLine("Cannot find file {0}", args[0]);
            }
        }
    }
}
