using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iRobotGUI;

namespace iRobotGUI.Tests
{
	[TestClass]
	public class HLProgramTest
	{
		[TestMethod()]
		public void TranslateIns(string ins, string expectedSourceResult)
		{
			string actualResult = Translator.TranslateInstruction(new Instruction(ins)).Trim();
			string expectedResult = expectedSourceResult.Trim();
			Assert.AreEqual<string>(expectedResult, actualResult);
		}

		[TestMethod()]
		public void TestMoveTranslation()
		{
			string input = "MOVE 500,5";
			string expectedResult = @"distance = 0;
byteTx(CmdDrive);
byteTx(0);
byteTx(100);
byteTx(128);
byteTx(0);
while(distance < 500)
{
	delaySensors(100);
}
byteTx(CmdDrive);
byteTx(0);
byteTx(0);
byteTx(128);
byteTx(0);";

			TranslateIns(input, expectedResult);
		}

		[TestMethod()]
		public void TestRotateTranslation()
		{
			string input = "ROTATE 90";
			string expectedResult = @"angle = 0;
byteTx(CmdDrive);
byteTx(0);
byteTx(128);
byteTx(0);
byteTx(1);
while(angle < 90)
{
	delaySensors(100);
}
byteTx(CmdDrive);
byteTx(0);
byteTx(0);
byteTx(128);
byteTx(0);";
			TranslateIns(input, expectedResult);
		}

		[TestMethod()]
		public void TestDriveTranslation()
		{
			string input = "DRIVE 100,100";
			string expectedResult = @"byteTx(CmdDrive);
byteTx(0);
byteTx(100);
byteTx(0);
byteTx(100);";
			TranslateIns(input, expectedResult);
		}

		[TestMethod()]
		public void TestLedTranslation()
		{
			string input = "LED 10,255,255";
			string expectedResult = @"byteTx(CmdLeds);
byteTx(10);
byteTx(255);
byteTx(255);";
			TranslateIns(input, expectedResult);
		}

		[TestMethod()]
		public void TestSongTranslation()
		{
			string input = "SONG 35,64,50,64";
			string expectedResult = @"byteTx(CmdSong);
byteTx(0);
byteTx(2);
byteTx(35);
byteTx(64);
byteTx(50);
byteTx(64);
byteTx(CmdPlay);
byteTx(0);";
			TranslateIns(input, expectedResult);
		}

		[TestMethod()]
		public void TestIfTranslation()
		{
			string input = "IF 2,2,2";
			string expectedResult = @"if (delaySensors(100), sensors[2] > 2)
{";
			TranslateIns(input, expectedResult);
		}

		[TestMethod()]
		public void TestElseTranslation()
		{
			string input = "ELSE";
			string expectedResult = @"}
else
{";
			TranslateIns(input, expectedResult);
		}

		[TestMethod()]
		public void TestEndIfTranslation()
		{
			string input = "END_IF";
			string expectedResult = "}";
			TranslateIns(input, expectedResult);
		}

		[TestMethod()]
		public void TestLoopTranslation()
		{
			string input = "LOOP 2,2,2";
			string expectedResult = @"while (delaySensors(0), sensors[2] > 2)
{";
			TranslateIns(input, expectedResult);
		}

		[TestMethod()]
		public void TestEndLoopTranslation()
		{
			string input = "END_LOOP";
			string expectedResult = "}";
			TranslateIns(input, expectedResult);
		}

		[TestMethod()]
		public void TestDemoTranslation()
		{
			string input = "DEMO 1";
			string expectedResult = @"byteTx(136);
byteTx(1);";
			TranslateIns(input, expectedResult);
		}
	}
}
