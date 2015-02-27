using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iRobotGUI.Controls
{
    /// <summary>
    /// Interaction logic for ProgramList.xaml
    /// </summary>
    public partial class ProgramList : UserControl
    {
        private HLProgram program;

        public HLProgram Program
        {
            get
            {
                return program;
            }
            set
            {
                program = value;
                UpdateContent();
            }
        }

        private int selectedInstructionIndex;
        private Instruction selectedInstruction;

        public ProgramList()
        {
            InitializeComponent();
        }

        #region private methods

        private void ShowSongDialog(Instruction ins)
        {
            SongWindow dlg = new SongWindow();
            dlg.Owner = Window.GetWindow(this);
            dlg.Ins = ins;
            dlg.ShowDialog();
        }

        private void ShowLedDialog(Instruction ins)
        {
            LedWindow dlg = new LedWindow();
            dlg.Owner = Window.GetWindow(this);
            dlg.Ins = ins;
            dlg.ShowDialog();
            UpdateContent();

        }

        private void ShowDemoDialog(Instruction ins)
        {
            DemoWindow dlg = new DemoWindow();
            dlg.Owner = Window.GetWindow(this);
            dlg.Ins = ins;
            dlg.ShowDialog();
            UpdateContent();
        }

        #endregion
        


        private void ListboxProgram_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            switch (selectedInstruction.opcode)
            {
                case Instruction.FORWARD:

                    break;
                case Instruction.LEFT:
                    break;
                case Instruction.LED:
                    ShowLedDialog(selectedInstruction);
                    break;
                case Instruction.SONG_DEF:
                    ShowSongDialog(selectedInstruction);
                    break;
                case Instruction.DEMO:
                    ShowDemoDialog(selectedInstruction);
                    break;
            }
        }

        private void ListBox_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string op = (string)e.Data.GetData(DataFormats.StringFormat);

                Instruction newIns = null;
                switch (op)
                {
                    case Instruction.FORWARD:
                        newIns = new Instruction(Instruction.FORWARD + " 500,3");
                        break;
                    case Instruction.LEFT:
                        newIns = new Instruction(Instruction.LEFT + " 90");
                        break;
                    case Instruction.LED:
                        newIns = new Instruction(Instruction.LED + " 10,128,128");
                        break;
                    case Instruction.SONG_DEF:
                        newIns = new Instruction(Instruction.SONG_DEF + " 0");
                        break;
                    case Instruction.DEMO:
                        newIns = new Instruction(Instruction.DEMO + " 0");
                        break;
                }
                if (newIns != null)
                {
                    program.Add(newIns);
                }

                UpdateContent();
                ListboxProgram.SelectedItem = newIns;
            }
        }

        private void ListBoxProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListboxProgram.SelectedItem != null)
            {
                selectedInstructionIndex = ListboxProgram.SelectedIndex;
                selectedInstruction = ListboxProgram.SelectedItem as Instruction;
            }
        }

        public void UpdateContent()
        {
            ListboxProgram.Items.Clear();
            foreach (Instruction i in program.GetInstructionList())
            {
                ListboxProgram.Items.Add(i);
            }
        }


    }
}
