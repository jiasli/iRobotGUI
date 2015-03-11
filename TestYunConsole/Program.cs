using iRobotGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestYunConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			string proIf = @"IF 0,0,0
ELSE 
	DELAY 1000
END_IF
ELSE";

			string proLoop = @"LOOP 0,0,0 
LOOP 0,0
	DELAY 1000
END_LOOP
";
            string proLength = @"END_LOOP";

			try
			{
				Validator.ValidateProgram(proLoop);
				Console.WriteLine("Ture");
			}
			catch (IfUnmatchedException ex)
			{
				Console.WriteLine("If unmatched at {0}: {1}", ex.Line, ex.InsStr);
			}
			catch (LoopUnmatchedException ex)
			{
				Console.WriteLine("Loop unmatched at {0}: {1}", ex.Line, ex.InsStr);
			}
            catch (ParameterLengthException ex)
            {
                Console.WriteLine("Parameter Length Unmathed at {0}: {1}", ex.Line, ex.InsStr);
            }
/*
            if (Validator.ValidateInstruction(proLength))
                Console.WriteLine("True");
            else
                Console.WriteLine("False");
 */ 
            Console.Read();

		}
	}
}
