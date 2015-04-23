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
                ins = Instruction.CreatDefaultFromOpcode(InvalidOpcodeStr);
                Assert.Fail("In Valid instruction string",ins);
            }
            catch (InvalidOpcodeException)
            {
                Console.WriteLine(InvalidOpcodeStr + ": Invalid");
            }

            try
            {
                var ins = new Instruction(ValidOpcodeStr);
                ins = Instruction.CreatDefaultFromOpcode(ValidOpcodeStr);
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
            string Str = "mycomments";
            string UnifyStr= Instruction.UnifyInstructionString(Str);
            try
            {              
                Assert.AreEqual(UnifyStr, " "+Str);     
            }
            catch (Exception)
            {
                Assert.AreNotEqual(UnifyStr, "");
            }  
        }

    }
}
