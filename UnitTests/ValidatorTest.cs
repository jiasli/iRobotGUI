using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
	[TestClass]
	public class ValidatorTest
	{
		[TestMethod]
		public void TestOpCode()
		{
			bool pass = true;
			string invalidIns = "LDE 12,13,13";
			string validIns = "LED 12,13,13";

			try
			{
				var ins = new Instruction(invalidIns);
				pass = false;
			}
			catch (InvalidOpcodeException)
			{
				Console.WriteLine(invalidIns + ": Invalid");
			}

			try
			{
				var ins = new Instruction(validIns);
				Console.WriteLine(validIns + ": valid");
			}
			catch (InvalidOpcodeException)
			{
				pass = false;
			}

			Assert.IsTrue(pass);
		}


		/// <summary>
		/// To test if the wrong parameter count can be detected.
		/// </summary>
		[TestMethod]
		public void TestParamCount()
		{
			string[] validInstrucsitons = new string[]
			{
				"MOVE 300,100",
				"DELAY 100"
			};

			string[] invalidInstructions = new string[]
			{				
				"DELAY",
				"DELAY 100,0"
			};

            foreach (string insStr in validInstrucsitons)
			{
				Validator.ValidateInstruction(new Instruction(insStr));
			}

			foreach (string insStr in invalidInstructions)
			{
				try
				{
					Instruction ins = new Instruction(insStr);

					// This line should throw a ParameterLengthException
					Validator.ValidateInstruction(ins);

					// Assert false if ParameterLengthException is not thrown
					Assert.Fail();
				}
				catch (ParameterCountInvalidException paramEx)
				{
                    Console.WriteLine(paramEx.ToString());
				}
			}
		}

		/// <summary>
		/// To test if the wrong parameter count can be detected.
		/// </summary>
		[TestMethod]
		public void TestIfMatchValidation()
		{
			string validIfBlock = @"IF 0,0,0
ELSE
	IF 0,0,0
	ELSE
	END_IF
END_IF";

			string inValidIfBlock = @"IF 0,0,0
ELSE
	IF 0,0,0
	END_IF
END_IF";

			try
			{
				Validator.ValidateProgram(new HLProgram(validIfBlock));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Assert.Fail();
			}

			try
			{
				Validator.ValidateProgram(new HLProgram(inValidIfBlock));
				Assert.Fail();
			}
			catch (IfUnmatchedException ex)
			{
				Console.WriteLine(ex.ToString());
			}
			
			
		}

		/// <summary>
		/// To test if the wrong parameter count can be detected.
		/// </summary>
		[TestMethod]
		public void TestLoopMatchValidation()
		{
			string validBlock = @"LOOP 0,0,0
LOOP 0,0,0
END_LOOP
END_LOOP";

			string inValidBlock = @"LOOP 0,0,0
LOOP 0,0,0
END_LOOP";

			try
			{
				Validator.ValidateProgram(new HLProgram(validBlock));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Assert.Fail();
			}

			try
			{
				Validator.ValidateProgram(new HLProgram(inValidBlock));
				Assert.Fail();
			}
			catch (LoopUnmatchedException ex)
			{
				Console.WriteLine(ex.ToString());
			}


		}
        /// <summary>
		/// To test if the wrong parameter count can be detected.
		/// </summary>
		[TestMethod]
        public void TestValidateSongDef()
        {
            bool pass = true;
           Instruction ValidSongIns = new Instruction ("SONG_DEF 1,52,32,33,32");
           Instruction InValidSongIns = new Instruction("SONG_DEF 1");
           try
           {
               pass= Validator.ValidateSongDef (InValidSongIns);
               pass = false;
           }
           catch (IfUnmatchedException ex)
           {
               Console.WriteLine(InValidSongIns + ex.ToString() + ": Invalid");
           }

           try
           {
               pass= Validator.ValidateSongDef(ValidSongIns);
               Console.WriteLine(ValidSongIns + ": valid");
           }
           catch (Exception ex)
           {
               Console.WriteLine(InValidSongIns + ex.ToString() + ": Invalid");
               pass = false;
           }

           Assert.IsTrue(pass);
        }


	}
}
