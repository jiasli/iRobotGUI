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
			string[] validInstrucitons = new string[]
			{
				"FORWARD 300,200",
				"DELAY 100"
			};

			string[] invalidInstructions = new string[]
			{				
				"DELAY",
				"DELAY 100,0"
			};

			foreach (string insStr in validInstrucitons)
			{
				Assert.IsTrue(Validator.ValidateInstruction(new Instruction(insStr)));
			}

			foreach (string insStr in invalidInstructions)
			{
				try
				{
					Instruction ins = new Instruction(insStr);

					// This line should throw a ParameterLengthException
					bool result = Validator.ValidateInstruction(ins);

					// Assert false if ParameterLengthException is not thrown
					Assert.Fail();
				}
				catch (ParameterLengthException paramEx)
				{

				}
			}
		}
	}
}
