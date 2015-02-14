using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
    public class CodeGenerator
    {
        private List<string> byteStream;
        public const string SEND_BYTE_CODE = "byteTx({0});";

        public CodeGenerator()
        {
            byteStream= new List<string>();
        }

        public void AddByte(string b)
        {
            byteStream.Add(b);
        }


        public string ToCode()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string b in byteStream)
            {
                sb.AppendLine(string.Format(SEND_BYTE_CODE, b));
            }
            return sb.ToString();
        }

    }
}
