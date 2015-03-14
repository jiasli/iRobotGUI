using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using iRobotGUI.Util;


namespace iRobotGUI.Controls
{
    /// <summary>
    /// Interaction logic for ProgramList.xaml
    /// </summary>
    public partial class ProgramList : UserControl
    {
        #region data

        private ListViewDragDropManager<Image> dragMgr;
        private HLProgram program;
        private ProgramViewModel pvm;

        #endregion // data

        #region constructor

        /// <summary>
        /// Initializes a new Instance of ProgramList
        /// </summary>
        public ProgramList()
        {
            InitializeComponent();
            this.Loaded += ListView1_Loaded;

            program = new HLProgram();
            pvm = new ProgramViewModel(program);
        }

        #endregion // constructor

        #region HLProgram

        /// <summary>
        /// Gets/sets the program so that the program can be updated when some function is dropped
        /// </summary>
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

        #endregion // HLProgram

        #region ProgramViewModel

        public ProgramViewModel ProgramViewModel
        {
            get
            {
                return pvm;
            }
            set
            {
                pvm = value;
                UpdateContent();
            }
        }

        #endregion

        #region ListView1_Loaded

        /// <summary>
        /// Load event handler
        /// </summary>
        void ListView1_Loaded(object sender, RoutedEventArgs e)
        {
            this.dragMgr = new ListViewDragDropManager<Image>(ListviewProgram);
            ListviewProgram.PreviewMouseLeftButtonDown += NewlistView_PreviewMouseLeftButtonDown;
            ListviewProgram.PreviewMouseRightButtonDown += listView_PreviewMouseRightButtonDown;
            ListviewProgram.Drop -= dragMgr.listView_Drop;
            ListviewProgram.Drop += NewlistView_Drop;
        }

        #endregion // ListView1_Loaded

        #region event handler

        #region NewlistView_PreviewMouseLeftButtonDown

        /// <summary>
        /// Open the dialog when an item is double clicked
        /// </summary>
        void NewlistView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount == 2)
            {
                int index = ListviewProgram.SelectedIndex;
                Instruction selectedIns = program.GetInstructionList().ElementAt(pvm[index]);
                DialogInvoker.ShowDialog(selectedIns, Window.GetWindow(this));
                UpdateContent();
            }

        }

        #endregion // NewlistView_PreviewMouseLeftButtonDown

        #region listView_PreviewMouseRightButtonDown

        /// <summary>
        ///  delete item when right button is clicked
        /// </summary>
        void listView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = this.dragMgr.IndexUnderDragCursor;

            int startIndex = pvm[index];
            int endIndex = startIndex;

            if (program[startIndex].opcode == Instruction.IF)
            {
                int endIf = program.FindEndIf(startIndex);
                if (endIf > 0)
                    endIndex = endIf + 1;
            }
            else if (program[startIndex].opcode == Instruction.LOOP)
            {
                int endLoop = program.FindEndLoop(startIndex);
                if (endLoop > 0)
                    endIndex = endLoop + 1;
            }

            pvm.Remove(pvm[index]);

            for (int i = startIndex; i < endIndex; i++)
            {
                Instruction selectedIns = program.GetInstructionList().ElementAt(i);
                if (selectedIns == null)
                    return;
                program.Remove(selectedIns);
            }
            
            UpdateContent();
        }

        #endregion // listView_PreviewMouseRightButtonDown

        #region NewlistView_Drop

        /// <summary>
        /// handler for drop event
        /// </summary>
        void NewlistView_Drop(object sender, DragEventArgs e)
        {
            int newIndex = this.dragMgr.IndexUnderDragCursor;

            // drag inside program list
            if (this.dragMgr.IsDragInProgress)
            {
                Image data = e.Data.GetData(typeof(Image)) as Image;

                if (data == null)
                    return;

                int oldIndex = this.ListviewProgram.Items.IndexOf(data);

                Instruction ins = program.GetInstructionList().ElementAt(pvm[oldIndex]);

                if (newIndex < 0)
                    return;

                if (oldIndex == newIndex)
                    return;

                int item = pvm[oldIndex];
                pvm.Remove(item);
                pvm.Insert(newIndex, item);

                UpdateContent();
                e.Effects = DragDropEffects.Move;
            }
            // drag from instruction panel to program list
            else
            {
                if (newIndex < 0)
                    newIndex = pvm.Count;

                string op = (string)e.Data.GetData(DataFormats.StringFormat);
                Instruction newIns = Instruction.CreatFromOpcode(op);

                if (newIns != null)
                {
                    pvm.Insert(newIndex, program.Count);
                    program.Add(newIns);
                }

                UpdateContent();
                ListviewProgram.SelectedItem = newIns;
            }
        }

        #endregion // NewlistView_Drop

        #endregion // event handler

        #region UpdateContent

        /// <summary>
        /// Update content in ProgramList accordint to the contents in program
        /// </summary>
        public void UpdateContent()
        {
            ListviewProgram.Items.Clear();

            for (int i = 0; i < pvm.Count; i++)
            {
                Instruction ins = program[pvm[i]];
                Image im = GetImageFromInstruction(ins);
                if (im != null)
                    ListviewProgram.Items.Add(GetImageFromInstruction(ins));
            }

        }

        #endregion

        #region GetImageFromInstruction

        /// <summary>
        /// Get the corresponding image from a specific instruction
        /// </summary>
        /// <param name="ins"> The Instruction </param>
        private Image GetImageFromInstruction(Instruction ins)
        {
            string picPath = "/iRobotGUI;component/pic/";
            Image im = new Image();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            switch (ins.opcode)
            {
                case Instruction.FORWARD:
                    bi.UriSource = new Uri(picPath + "forward.png", UriKind.Relative);
                    // im.Tag = "FORWARD";
                    break;
                case Instruction.BACKWARD:
                    bi.UriSource = new Uri(picPath + "backward.png", UriKind.Relative);
                    // im.Tag = "BACKWARD";
                    break;
                case Instruction.LEFT:
                    bi.UriSource = new Uri(picPath + "left.png", UriKind.Relative);
                    //im.Tag = "LEFT";
                    break;
                case Instruction.RIGHT:
                    bi.UriSource = new Uri(picPath + "right.png", UriKind.Relative);
                    //im.Tag = "RIGHT";
                    break;
                case Instruction.LED:
                    bi.UriSource = new Uri(picPath + "led.jpg", UriKind.Relative);
                    break;
                case Instruction.SONG_DEF:
                    bi.UriSource = new Uri(picPath + "song.png", UriKind.Relative);
                    break;
                case Instruction.DEMO:
                    bi.UriSource = new Uri(picPath + "demo.jpg", UriKind.Relative);
                    break;
                case Instruction.IF:
                    bi.UriSource = new Uri(picPath + "if.png", UriKind.Relative);
                    break;
                case Instruction.LOOP:
                    bi.UriSource = new Uri(picPath + "loop.png", UriKind.Relative);
                    break;
                default:
                    bi.UriSource = null;
                    break;
            }
            if (bi.UriSource == null) return null;
            bi.EndInit();
            im.Stretch = Stretch.Fill;
            im.Source = bi;
            im.Width = 50;
            im.Height = 50;
            return im;
        }

        #endregion
    }
}
