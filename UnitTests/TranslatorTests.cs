using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRobotPrototypeWpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iRobotPrototypeWpf.Tests
{
    [TestClass]
    public class TranslatorTests
    {
        [TestMethod]
        public void TranslateTest()
        {
            string navProgram = "LED 10,125,125";
            string Actual_program = Translator.TranslateProgram(new HLProgram(navProgram));
            Console.WriteLine(Actual_program);
            Assert.IsNotNull(Actual_program);
        }
    }
}
