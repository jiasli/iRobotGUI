using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRobotGUI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iRobotGUI.Tests
{
   
    [TestClass]
    public class TranslatorTest
    {
        [TestMethod()]
        public void TranslateInstructionTest()
        {
            string ValidOpcodeStr = "LED 10,125,125";
            string invalidIns = "FROWARD 300,2";

            try
            {
                Instruction InvalidIns = new Instruction(invalidIns);
                string InvalidStr = Translator.TranslateInstruction(InvalidIns);
                Assert.Fail("InValid instruction string");

            }
            catch (InvalidOpcodeException)
            {
                Console.WriteLine(invalidIns + ":Invalid");
            }

            try
            {
                var Validins = new Instruction(ValidOpcodeStr);
                string ValidStr = Translator.TranslateInstruction(Validins);
                Console.WriteLine(ValidStr + ": valid");
            }
            catch (InvalidOpcodeException)
            {
                Assert.Fail("InValid instruction string");
            }

        }
        [TestMethod()]
        public void TranslatePrintInstrString(string actualInstr)
        {
            string input = actualInstr.Trim();
            string Actual_Ins = Translator.TranslateInstruction(new Instruction(input));
            Console.WriteLine(Actual_Ins);
            Assert.IsNotNull(Actual_Ins);
        }

        public void TranslatePrintProgString(string actualProg)
        {
            string input = actualProg.Trim();
            string Actual_Program = Translator.Translate(new HLProgram(input));
            Console.WriteLine(Actual_Program);
            Assert.IsNotNull(Actual_Program);
        }

        [TestMethod()]
        public void TranslateDemo()
        {
            string demoInstruction = "DEMO 0";
            TranslatePrintInstrString(demoInstruction);
        }

        [TestMethod()]
        public void TranslateDrive()
        {
            string driveInstruction = "DRIVE 10,20";
            TranslatePrintInstrString(driveInstruction);
        }
        /*
        [TestMethod()]
        public void TranslateForward()
        {
            string forwardInstruction = "FORWARD 50,5";
            TranslatePrintInstrString(forwardInstruction);
        }

        [TestMethod()]
        public void TranslateBackward()
        {
            string backwardInstruction = "BACKWARD 10,3";
            TranslatePrintInstrString(backwardInstruction);
        }
        
        [TestMethod()]
        public void TranslateLeft()
        {
            string leftInstruction = "LEFT 80";
            TranslatePrintInstrString(leftInstruction);
        }

        [TestMethod()]
        public void TranslateRight()
        {
            string rightInstruction = "RIGHT 80";
            TranslatePrintInstrString(rightInstruction);
        }
        */
        [TestMethod()]
        public void TranslateLED()
        {
            string ledInstruction = "LED 10,125,125";
            TranslatePrintInstrString(ledInstruction);
        }

        [TestMethod()]
        public void TranslateSong()
        {
            string songInstruction = "SONG_DEF 1,52,32,33,32\nSONG_PLAY 1";
            TranslatePrintProgString(songInstruction);
        }

        [TestMethod()]
        public void TranslateIfProgram()
        {
            string ifProgram = @"IF 2,0,1
DELAY 200
ELSE
DRIVE 300,32768
END_IF";
            TranslatePrintProgString(ifProgram);
        }

        [TestMethod()]
        public void TranslateLoopProgram()
        {
            string loopProgram = @"LOOP 0,1,1
DRIVE 300,32768
DELAY 300
END_LOOP
DRIVE 0,32768";
            TranslatePrintProgString(loopProgram);
        }

        [TestMethod()]
        [ExpectedException(typeof(iRobotGUI.InvalidOpcodeException))]
        public void TranslateInvalidInstruction()
        {
            string invalidIns = "FROWARD 300,2";
            TranslatePrintInstrString(invalidIns);
        }

        [TestMethod()]
        public void TranslateProgram()
        {
            string ProgramInstruction = "LED 12,13,13\nDRIVE 300,32768";
            TranslatePrintProgString(ProgramInstruction);
        }

        [TestMethod()]
        public void GenerateCSourceTest(Translator.SourceType st, string code)
        {
            bool pass = false;
            if (st.HasFlag(Translator.SourceType.Microcontroller))
                if (!String.IsNullOrEmpty(System.IO.File.ReadAllText(code)))
                    pass = true;
                else
                    pass = false;
            if (st.HasFlag(Translator.SourceType.Emulator))
                if (!String.IsNullOrEmpty(System.IO.File.ReadAllText(code)))
                    pass = true;
                else
                    pass = false;

            Assert.IsTrue(pass);
        }
   
    }
}