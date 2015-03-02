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
                Console.WriteLine(invalidIns +": Invalid");
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
    }
}
