using System;
using System.IO;
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
using System.Text.RegularExpressions;

namespace iRobotGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand ComPortCmd = new RoutedUICommand("Load Configuration", "comn", typeof(Window));

        private string fileName;
        private HLProgram program;

        private int selectedInstructionIndex;
        private Instruction selectedInstruction;

        public MainWindow()
        {

            InitializeComponent();
            Directory.SetCurrentDirectory(@".");
            program = new HLProgram();

        }

        #region Commands
        void OpenCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = Directory.GetCurrentDirectory();
            dlg.DefaultExt = ".igp"; // Default file extension
            dlg.Filter = "iRobot Graphical Program|*.igp"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                fileName = dlg.FileName;
                LoadProgram(fileName);
            }
        }


        /// <summary>
        /// Show Configuration Window
        /// </summary>
        private void ConfigCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // Instantiate the dialog box
            ConfigurationWindow dlg = new ConfigurationWindow();


            // Configure the dialog box
            dlg.Owner = this;
            dlg.Config = WinAvrConnector.config;

            // Open the dialog box modally 
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                // MessageBox.Show(WinAvrConnector.config.ToString());

            }
        }

        private void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Instruction"; // Default file name
            dlg.DefaultExt = ".ins"; // Default file extension
            dlg.Filter = "Text documents (.ins)|*.ins"; // Filter files by extension 

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results 
            if (result == true)
            {
                // Save document 
                string filename = dlg.FileName;
            }
        }


        #endregion



        #region Private Methods

        private void LoadProgram(string fileName)
        {
            program = new HLProgram(File.ReadAllText(fileName));
            UpdateProgramPanel();
        }

        private void UpdateProgramPanel()
        {
            TextBoxCode.Text = program.ToString();
            ListboxProgram.Items.Clear();
            foreach (Instruction i in program.GetInstructionList())
            {
                ListboxProgram.Items.Add(i);
            }
        }

        private void ChangeParameterPanel(Instruction ins)
        {
            ForwardPanel.Visibility = Visibility.Collapsed;
            RotatePanel.Visibility = Visibility.Collapsed;
           
            if (ins != null)
            {
                string op = ins.opcode;

                if (op == Instruction.FORWARD || op == Instruction.BACKWARD)
                {
                    ForwardPanel.Visibility = Visibility.Visible;
                    textBoxDistance.Text = ins.parameters[0].ToString();
                    textBoxTime.Text = ins.parameters[1].ToString();
                }

                else if (op == Instruction.LEFT || op == Instruction.RIGHT)
                {
                    RotatePanel.Visibility = Visibility.Visible;
                    textBoxAngle.Text = ins.parameters[0].ToString();
                }

                else if(op ==Instruction.LED)
                {
                    ShowLedDialog(ins);
                }

                else if (op == Instruction.SONG_DEF)
                {
                    ShowSongDialog(ins);
                }
               
            }
        }

        private void ShowSongDialog(Instruction ins)
        {
            SongWindow dlg = new SongWindow();
            dlg.Owner = this;
            dlg.Ins = ins;
            dlg.ShowDialog();
        }

        private void ShowLedDialog(Instruction ins)
        {
            LedWindow dlg = new LedWindow();
            dlg.Owner = this;
            dlg.Ins = ins;
            dlg.ShowDialog();
            UpdateProgramPanel();

        }


        #endregion



        #region Drag and Drop

        

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
                }
                if (newIns != null)
                {
                    program.Add(newIns);
                }

                UpdateProgramPanel();
                ListboxProgram.SelectedItem = newIns;
                ChangeParameterPanel(newIns);
            }
        }

        #endregion


        #region Control Callbacks

        private void ListBoxProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListboxProgram.SelectedItem != null)
            {
                selectedInstructionIndex = ListboxProgram.SelectedIndex;
                selectedInstruction = ListboxProgram.SelectedItem as Instruction;               
            }
        }

       
        private void TextBoxDistance_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateParamFromTextBox(sender as TextBox, selectedInstruction, 0);
        }

        private void TextBoxTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateParamFromTextBox(sender as TextBox, selectedInstruction, 1);
        }

        private void TextBoxAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateParamFromTextBox(sender as TextBox, selectedInstruction, 0);
        }

        /// <summary>
        /// Update a parameter of Instruction according to the data in a TextBox
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="ins"></param>
        /// <param name="index"></param>
        private void UpdateParamFromTextBox(TextBox tb, Instruction ins, int index)
        {
            if (ins == null) return;
            // Default
            if (tb != null)
            {
                if (!string.IsNullOrEmpty(tb.Text))
                {
                    ins.parameters[index] = Convert.ToInt32(tb.Text);
                }
                ListboxProgram.Items.Refresh();
            }
        }



        #endregion

        private void BuildAndLoad(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(System.IO.Directory.GetCurrentDirectory());
            string template = File.ReadAllText(@"template.c");
            File.WriteAllText(@"output.c", template);
            MessageBox.Show("Compiling and loading succeeded");
        }



        private void MenuItem_ShowCCode(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"output.c");
        }



        private void ButtonLoadExampleCode_Click(object sender, RoutedEventArgs e)
        {
            LoadProgram("navi.igp");

        }


        private void MenuItemBuild_Click(object sender, RoutedEventArgs e)
        {
            //Window errorWin = new OutputWindow();
            //errorWin.Show();
            //  WinAvrConnector.Clean();


            WinAvrConnector.Make();
        }


        // textbox input form validation function
        private void number_validation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            WinAvrConnector.Load();
        }

        private void MenuItemClean_Click(object sender, RoutedEventArgs e)
        {
            File.Delete("C_result.c");
            WinAvrConnector.Clean();
        }

        private void MenuItemTranslate_Click(object sender, RoutedEventArgs e)
        {
            string cCode = Translator.TranslateProgram(program);

            MessageBox.Show(cCode);
            Translator.GenerateCSource(Translator.SourceType.Microcontroller, cCode);
            Translator.GenerateCSource(Translator.SourceType.Emulator, cCode);
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Mission Science iRobots\nUSC CSCI-577 Team 07");
        }


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
            }
        }








    }
}

