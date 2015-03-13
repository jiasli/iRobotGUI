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
        [TestProperty("Opcode", "FORWARD")]
        //New code: Trying to create light weight data driven unit test
        public void TranslateInstructionTestHelper()
        {
            try
            {      
                string op = TestContext.Properties["Opcode"] as string;
                Console.WriteLine("op = {0}",op);
                Instruction a = new Instruction(op);
                TranslateInstructionTest(a);
            }
            catch(NullReferenceException)
            {
                Console.WriteLine("null reference exception");
            }            
        }
        public TestContext TestContext
        {
            get;
            set;
        }
        //End of new code
        public void TranslateInstructionTest(Instruction instruction)
        {
            string validOpCodeSrting = "FORWARD";
            string invalidOpCodeString = "INFORNTOF";
            bool pass = true;

            try
            {
                if (instruction.opcode.Equals(validOpCodeSrting))
                {
                    pass = true;
                    Console.WriteLine(validOpCodeSrting+": Valid");
                }
                else
                if (instruction.opcode.Equals(invalidOpCodeString))
                {
                    pass = false;
                    Console.WriteLine(invalidOpCodeString+": Invalid");
                }
                else
                {
                    pass = true;
                    Console.WriteLine(invalidOpCodeString+": Valid");
                }
            }
            catch (NullReferenceException)
            {
                pass = false;
            }
            Assert.IsTrue(pass);
        }

        [TestMethod()]
        public void TranslateProgramTest()
        {
            string navProgram = "FORWARD 10,3\nLED 10,125,125";
            string Actual_program = Translator.TranslateProgram(new HLProgram(navProgram));
            Console.WriteLine(Actual_program);
            Assert.IsNotNull(Actual_program);
           
        }

        [TestMethod()]
        public void TranslateProgramStringTest()
        {
            string ProgramString = "FORWARD 50,5\nLED 10,125,125";
            string Actual_program = Translator.TranslateProgram(new HLProgram(ProgramString));
            Console.WriteLine(Actual_program);
            Assert.IsNotNull(Actual_program);
        }

        [TestMethod()]
        public void TranslateInstructionStringTest()
        {
            string instructionString = "LED 10,125,125";
            string Actual_Ins= Translator.TranslateInstruction(new Instruction(instructionString));
            Console.WriteLine(Actual_Ins);
            Assert.IsNotNull(Actual_Ins);
        }

        [TestMethod()]
        public void TranslateDrive()
        {
            string driveInstruction = "DRIVE 10,20";
            string Actual_Ins = Translator.TranslateInstruction(new Instruction(driveInstruction));
            Console.WriteLine(Actual_Ins);
            Assert.IsNotNull(Actual_Ins);
        }

        [TestMethod()]
        public void TranslateForwardBackward()
        {
            string ProgramInstruction = "FORWARD 50,5\nBACKWARD 10,3";
            string Actual_program = Translator.TranslateProgram(new HLProgram(ProgramInstruction));
            Console.WriteLine(Actual_program);
            Assert.IsNotNull(Actual_program);
        }

        [TestMethod()]
        public void TranslateLeftRight()
        {
            string ProgramInstruction = "LEFT 50,5\nRIGHT 10,3";
            string Actual_Program = Translator.TranslateProgram(new HLProgram(ProgramInstruction));
            Console.WriteLine(Actual_Program);
            Assert.IsNotNull(Actual_Program);
        }

        [TestMethod()]
        public void TranslateLED()
        {
            string ledInstruction = "LED 10,125,125";
            string Actual_Ins = Translator.TranslateInstruction(new Instruction(ledInstruction));
            Console.WriteLine(Actual_Ins);
            Assert.IsNotNull(Actual_Ins);
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