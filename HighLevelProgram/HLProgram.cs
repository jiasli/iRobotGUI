using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{

    /// <summary>
    /// High-level program
    /// </summary>
    public class HLProgram : IEnumerable
    {
        public static int CurrentLine;

        private List<Instruction> program;
        public HLProgram()
        {
            program = new List<Instruction>();
        }

        public HLProgram(String programString)
        {
            program = new List<Instruction>();

            string[] insStrArray = programString.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < insStrArray.Length; i++)
            {
                CurrentLine = i;

                // Ignore comment line
                if (Instruction.IsInstructionLine(insStrArray[i]))
                    program.Add(new Instruction(insStrArray[i]));
            }
        }

        /// <summary>
        /// Get the count of Instructions.
        /// </summary>
        public int Count
        {
            get
            {
                return program.Count();
            }
        }


        /// <summary>
        /// Gets or sets the Instruction at the specified index. Can be used as program[2].
        /// </summary>
        /// <param name="index">The zero-based index of the Instruction to get or set.</param>
        /// <returns></returns>
        public Instruction this[int index]
        {
            get
            {
                return program[index];
            }
            set
            {
                program[index] = value;
            }
        }

        public void Add(String ins)
        {
            program.Add(new Instruction(ins));
        }

        public void Add(HLProgram subProgram)
        {
            program.AddRange(subProgram.program);
        }

        /// <summary>
        /// Add an Instruction to the end of the list.
        /// </summary>
        /// <param name="ins"></param>
        public void Add(Instruction ins)
        {
            this.program.Add(ins);
        }

        /// <summary>
        /// Find the corresponding ELSE location given the index of an IF instruction.
        /// </summary>
        /// <param name="ifIndex"></param>
        /// <returns></returns>
        public int FindElse(int ifIndex)
        {
            int ifCount = 0;
            int currentIns = ifIndex;

            while (currentIns < program.Count)
            {
                if (program[currentIns].opcode == Instruction.IF)
                    ifCount++;
                else if (program[currentIns].opcode == Instruction.ELSE)
                    ifCount--;

                if (ifCount == 0)
                    return currentIns;

                currentIns++;
            }
            return -1;
        }

        /// <summary>
        /// Find the corresponding END_IF location given the index of an IF instruction.
        /// </summary>
        /// <param name="ifIndex"> the line number of IF </param>
        /// <returns> the line number of END_IF </returns>
        public int FindEndIf(int ifIndex)
        {
            int ifCount = 0;
            int currentIns = ifIndex;

            while (currentIns < program.Count)
            {
                if (program[currentIns].opcode == Instruction.IF)
                    ifCount++;

                if (program[currentIns].opcode == Instruction.END_IF)
                    ifCount--;

                if (ifCount == 0)
                    return currentIns;

                currentIns++;
            }

            return -1;
        }

        /// <summary>
        /// Find the corresponding END_LOOP location given the index of an LOOP instruction.
        /// </summary>
        /// <param name="loopIndex"> the line number of LOOP </param>
        /// <returns> the line numer of END_LOOP</returns>
        public int FindEndLoop(int loopIndex)
        {
            int loopCount = 0;
            int currentIns = loopIndex;

            while (currentIns < program.Count)
            {
                if (program[currentIns].opcode == Instruction.LOOP)
                    loopCount++;

                if (program[currentIns].opcode == Instruction.END_LOOP)
                    loopCount--;

                if (loopCount == 0)
                    return currentIns;

                currentIns++;
            }

            return -1;
        }

        /// <summary>
        /// HLProgram can be used as
        /// foreach (Instruction ins in hlProgram)
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (Instruction ins in program)
            {
                yield return ins;
            }
        }

        public List<Instruction> GetInstructionList()
        {
            return program;
        }

        /// <summary>
        /// Insert specified Instruction to index position
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ins"></param>
        public void Insert(int index, Instruction ins)
        {
            program.Insert(index, ins);
        }

        public void Insert(int index, HLProgram subProgram)
        {
            program.InsertRange(index, subProgram.program);
        }

        /// <summary>
        /// Remove a instruction
        /// </summary>
        /// <param name="ins">the instruction to be removed</param>
        public void Remove(Instruction ins)
        {
            program.Remove(ins);
        }

        /// <summary>
        /// Remove a set of instructions
        /// </summary>
        /// <param name="subProgram"></param>
        public void Remove(int index, int count)
        {
            program.RemoveRange(index, count);
        }


        /// <summary>
        /// Get the sub-program specified by a range.
        /// </summary>
        /// <param name="startIndex">the index of the first Instruction.</param>
        /// <param name="endIndex">the index of the last Instruction.</param>
        /// <returns></returns>
        public HLProgram SubProgram(int startIndex, int endIndex)
        {
            HLProgram hlp = new HLProgram();

            for (int i = startIndex; i <= endIndex; i++)
            {
                hlp.Add(program[i]);
            }
            return hlp;
        }

        public override string ToString()
        {
            return string.Join("\n", program);
        }
    }
}
