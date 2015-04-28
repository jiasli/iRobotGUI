using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRobotGUI.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace iRobotGUI.Util.Tests
{
    [TestClass()]
    public class ProgramViewModelTests
    {
        [TestMethod()]
        public void GetHLProgramTest()
        {
            HLProgram hp = new HLProgram();
            ProgramViewModel pvm = new ProgramViewModel(new HLProgram());
            hp = pvm.GetHLProgram();
            Assert.IsNotNull(hp);     
        }

        [TestMethod()]
        public void GetPointerTypeTest()
        {
			//var ifExpectPointer = ProgramViewModel.PointerType.IF;
			//ProgramViewModel pvm = new ProgramViewModel(new HLProgram());
			//pvm.InsertInstruction(0, Instruction.CreatDefaultFromOpcode(Instruction.IF));
			//var ifActualPointer= pvm.GetPointerType(0);
			//Assert.AreEqual(ifExpectPointer,ifActualPointer);
            
        }

        [TestMethod()]
        public void GetSubProgramTest()
        {
            ProgramViewModel pvm = new ProgramViewModel(new HLProgram());
            pvm.InsertInstruction(0, Instruction.CreatDefaultFromOpcode(Instruction.DRIVE));
            HLProgram hp = pvm.GetSubProgram(0);
            Assert.IsNull(hp);
        }

        [TestMethod()]
        public void InsertInstructionTest()
        {
            bool pass = true;
            try
            {
                ProgramViewModel pvm = new ProgramViewModel(new HLProgram());
                pvm.InsertInstruction(0, Instruction.CreatDefaultFromOpcode(Instruction.LED));
                pass = true;
            }
            catch(InstructionException ex)
			{
				Console.WriteLine(ex + ": Invalid");
                pass = false;
            }
            Assert.IsTrue(pass);
        }

        [TestMethod()]
        public void InsertSubProgramTest()
        {
            bool pass = true;
            string ValidifString = @"IF 0,0,0 
ELSE
END_IF";
            try
            {
                HLProgram hp = new HLProgram(ValidifString);
                ProgramViewModel pvm = new ProgramViewModel(new HLProgram());
                pvm.InsertSubProgram(0, hp);
                pass = true;
            }
            catch (InvalidProgramException ex)
            {
                Console.WriteLine(ex + ": Invalid");
                pass = false;
            }
            Assert.IsTrue(pass);
        }

        [TestMethod()]
        public void UpdateInstructionTest()
        {
            bool pass = true;
            try
            {
                ProgramViewModel pvm = new ProgramViewModel(new HLProgram());
                pvm.InsertInstruction(0, Instruction.CreatDefaultFromOpcode(Instruction.LED));
                pvm.UpdateInstruction(0, Instruction.CreatDefaultFromOpcode(Instruction.MOVE));
                pass = true;
            }
            catch (InstructionException ex)
            {
                Console.WriteLine(ex + ": Invalid");
                pass = false;
            }
            Assert.IsTrue(pass);
        }

        [TestMethod()]
        public void UpdateSubProgramTest()
        {
            bool pass = true;
            string ValidLOOPString = @"LOOP 0,0,0 
END_LOOP";
            string InvalidLOOPString = @"IF 0,0,0 
ELSE
END_IF";
            try
            {
                HLProgram hpValid = new HLProgram(ValidLOOPString);
                HLProgram hpInvalid = new HLProgram(InvalidLOOPString);
                ProgramViewModel pvm = new ProgramViewModel(new HLProgram());
                
                pvm.InsertSubProgram(0, hpInvalid);
                pvm.UpdateSubProgram(0, hpValid);
                pass = true;
            }
            catch (InstructionException ex)
            {
                Console.WriteLine(ex + ": Invalid");
                pass = false;
            }
            Assert.IsTrue(pass);
        }
    }
}
