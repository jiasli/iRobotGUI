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
    public class HLProgramTests
    {
       
       
        [TestMethod()]
        public void FindElseTest()
        {
            
            string IfBlock = @"IF 0,0,0
ELSE
	IF 0,0,0
	ELSE
	END_IF
END_IF";
          
            int ValidifIndex = 0,inValidifIndex=1;
             HLProgram p = new HLProgram(IfBlock);
            try
            {
                int i=p.FindElse(ValidifIndex);
                Assert.AreNotEqual(-1, i);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Assert.Fail("Invalid if index :", inValidifIndex);
            }

            try
            {
                int i = p.FindElse(inValidifIndex);
                Assert.AreNotEqual(-1, i);
               
            }
            catch (IfUnmatchedException ex)
            {
                Console.WriteLine(ex.ToString());
                Assert.Fail("Invalid if index :", inValidifIndex);
            }  
        }

        [TestMethod()]
        public void GetIfBlockTest()
        {
            string StartifBlock = @"IF ";
            GetIfLoopBlockTest(StartifBlock);

        }

        [TestMethod()]
        public void GetLoopBlockTest()
        {
            string StartloopBlock = @"LOOP ";
            GetIfLoopBlockTest(StartloopBlock);

        }

        [TestMethod()]
        public void GetIfLoopBlockTest(string Startifloop)
        {
            string inputBlock = Startifloop.Trim();
            HLProgram ActualBlock = HLProgram.GetDefaultIfLoopBlock(new Instruction(inputBlock));
            Console.WriteLine(ActualBlock);
            Assert.IsNotNull(ActualBlock);
        }

        [TestMethod()]
        public void FindEndIfTest()
        {
            
            string IfBlock = @"IF 0,0,0
ELSE
	IF 0,0,0
	ELSE
	END_IF
END_IF";

            int ValidifIndex = 0, inValidifIndex = 0;
            HLProgram p = new HLProgram(IfBlock);
            try
            {
                int i= p.FindEndIf(ValidifIndex);
                Assert.AreNotEqual(-1, i);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Assert.Fail("Invalid if index :", inValidifIndex);
            }

            try
            {
                int i= p.FindEndIf(inValidifIndex);
                Assert.AreNotEqual(-1, i);
                
            }
            catch (IfUnmatchedException ex)
            {
                Console.WriteLine(ex.ToString());
                Assert.Fail("Invalid if index :", inValidifIndex);
            }
            
        }

        [TestMethod()]
        public void FindEndLoopTest()
        {
            
            string IfBlock = @"LOOP 0,0,0
LOOP 0,0,0
END_LOOP
END_LOOP";
            int ValidLoopIndex = 0, inValidLoopIndex = 1;
            HLProgram p = new HLProgram(IfBlock);
            try
            {
                int i = p.FindEndLoop(ValidLoopIndex);
                Assert.AreNotEqual(-1, i);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Assert.Fail("Invalid LOOP index :", inValidLoopIndex);
            }

            try
            {
                int i= p.FindEndLoop(inValidLoopIndex);
                Assert.AreNotEqual(-1, i);
                
            }
            catch (LoopUnmatchedException ex)
            {
                Console.WriteLine(ex.ToString());
                Assert.Fail("Invalid LOOP index :", inValidLoopIndex);
            }
            
        }

      
    }
}
