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
    public class InstructionPictureTests
    {
        [TestMethod()]
        public void GetPictureNameTest()
        {
            string ValidOpcodeStr = "LED";
            string InvalidOpcodeStr = "MOV";
            try
            {
                var ins = new Instruction(InvalidOpcodeStr);
                string  actualResult = PictureDiscription.GetPictureName(ins);
                Assert.IsNull(actualResult);
            }
            catch (InvalidOpcodeException)
            {
                Console.WriteLine(InvalidOpcodeStr + ": Invalid");
            }

            try
            {
                var ins = new Instruction(ValidOpcodeStr);
                string actualResult = PictureDiscription.GetPictureName(ins);
                Assert.IsNotNull(actualResult);
            }
            catch (InvalidOpcodeException)
            {
                Assert.Fail("In Valid instruction string");
            }

        }
    }
}
