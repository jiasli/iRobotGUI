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
        public void SetupTranslateInstructionTest()
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
            string navProgram = "FORWARD 10,3    LED 10,125,125";
            string Actual_program = Translator.TranslateProgram(new HLProgram(navProgram));
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
        public void TranslateProgramStringTest()
        {
            string ProgramString = "FORWARD 10,3    LED 10,125,125";
            string Actual_program = Translator.TranslateProgram(new HLProgram(ProgramString));
            Console.WriteLine(Actual_program);
            Assert.IsNotNull(Actual_program);
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
