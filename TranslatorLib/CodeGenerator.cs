using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
	static public class CodeGenerator
	{
		public const string PLACEHOLDER_MAIN_PROGRAM = @"/**main_program**/";

		/// <summary>
		/// The function put generated C code instruction into C file can be compiled
		/// </summary>
		/// <param name="st">Decide it is a Microcontroller program or an Emulator program</param>
		/// <param name="code">Generated C code instruction</param>
		public static void GenerateCSource(string templateFilePath, string outputFilePath, string code)
		{
			string template;

			template = File.ReadAllText(templateFilePath);
			if (!String.IsNullOrEmpty(template))
			{
				File.WriteAllText(outputFilePath,
					template.Replace(PLACEHOLDER_MAIN_PROGRAM, code));
			}
		}

	}
}
