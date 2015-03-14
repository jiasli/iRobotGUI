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
    public class ValidatorTests
    {
        [TestMethod()]
        public void Check_IF_ELSE_Instruction(string Str_OpCode)
        {
            string IF_Str = Str_OpCode;
            bool Valid_Ins = Validator.ValidateIf(IF_Str);
            Console.WriteLine(Valid_Ins);
            Assert.IsNotNull(Valid_Ins);
        }

        [TestMethod()]
        public void Validator_IF_ELSE_Instruction()
        {
            string ValidOpCode_Ins = Instruction.IF;
            string InvalidOpCode_Ins = Instruction.LED;
            Check_IF_ELSE_Instruction(ValidOpCode_Ins);
            Check_IF_ELSE_Instruction(InvalidOpCode_Ins);   
        }

        [TestMethod()]
        public void Check_LOOP_Instruction(string Str_OpCode)
        {
            string LOOP_Str = Str_OpCode;
            bool Valid_Ins = Validator.ValidateLoop(LOOP_Str);
            Console.WriteLine(Valid_Ins);
            Assert.IsNotNull(Valid_Ins);
        }
        [TestMethod()]
        public void Validator_LOOP_Intruction()
        {
            string ValidOpCode_Ins = Instruction.LOOP;
            string InvalidOpCode_Ins = Instruction.LEFT;
            Check_LOOP_Instruction(ValidOpCode_Ins);
            Check_LOOP_Instruction(InvalidOpCode_Ins);  
        }

        [TestMethod()]
        public void ValidateInstructionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateParaLengthTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateParaRangeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateInstructionTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateProgramTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateProgramTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateSongDefTest()
        {
            Assert.Fail();
        }
    }
}
