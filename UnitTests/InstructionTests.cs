using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRobotGUI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace iRobotGUI.Tests
{
    [TestClass()]
    public class InstructionTests
    {
       

        [TestMethod()]
        public void CreatFromOpcodeTest()
        {
            string ValidOpcodeStr = "DRIVE";
            string InvalidOpcodeStr = "MOV";
            try
            {
                var ins = new Instruction(InvalidOpcodeStr);
                ins = Instruction.CreatFromOpcode(InvalidOpcodeStr);
                Assert.Fail("In Valid instruction string",ins);
            }
            catch (InvalidOpcodeException)
            {
                Console.WriteLine(InvalidOpcodeStr + ": Invalid");
            }

            try
            {
                var ins = new Instruction(ValidOpcodeStr);
                ins = Instruction.CreatFromOpcode(ValidOpcodeStr);
                Console.WriteLine(ValidOpcodeStr + ": valid");
            }
            catch (InvalidOpcodeException)
            {
                Assert.Fail("In Valid instruction string");
            }

        }

        [TestMethod()]
        public void IsInstructionLineTest()
        {
            bool pass = true;
            string ValidOpcodeStr = "BACKWARD";
            string InvalidOpcodeStr = "BACK";
            try
            {
               // pass = Instruction.UnifyInstructionString(InvalidOpcodeStr);
            }
            catch (Exception)
            {
                Console.WriteLine(InvalidOpcodeStr + ": Invalid");
            }

            try
            {
               // pass = Instruction.UnifyInstructionString(ValidOpcodeStr);
                Console.WriteLine(ValidOpcodeStr + ": valid");
            }
            catch (InvalidOpcodeException)
            {
                pass = false;
            }

            Assert.IsTrue(pass);
        }

    }
}
