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
            LEDPanel.Visibility = Visibility.Collapsed;
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
                if (op == Instruction.LED)
                {
                    LEDPanel.Visibility = Visibility.Visible;
                    SliderColor.Value = ins.parameters[1];
                    SliderIntensity.Value = ins.parameters[2];

                    // Set checkbox status according to 4th and 2nd bits
                    // A variable must be used or ins.parameters[0] will be modified due to IsChecked assignment
                    int bitValue = ins.parameters[0];
                    CheckBoxLedPlay.IsChecked = (bitValue & 2) > 0;
                    CheckBoxLedAdvance.IsChecked = (bitValue & 8) > 0;
                }
            }
        }

        private Instruction ShowSongDialog(String insStr, bool isNewIns)
        {
            // Instantiate the dialog box
            SongWindow dlg = new SongWindow();

            // Configure the dialog box
            dlg.Owner = this;
            Instruction result = null;

            dlg.songInsStr = insStr;

            // Open the dialog box modally 
            if (dlg.ShowDialog() ?? false)
            {
                if (isNewIns)
                {
                    result = new Instruction(dlg.songInsStr);
                }
            }
            return result;
        }


        #endregion








        #region Drag and Drop

        private void MouseMove_General(object sender, MouseEventArgs e)
        {
            Image dragSource = sender as Image;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(sender as Image, dragSource.Tag, DragDropEffects.Copy);
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
                        newIns = ShowSongDialog(null, true);
                        break;
                }
                if (newIns != null)
                {
                    program.Add(newIns);
                    if (op == Instruction.SONG_DEF)
                    {
                        program.Add(Instruction.SONG_PLAY + " 1");
                    }
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
                ChangeParameterPanel(selectedInstruction);
            }
        }


        private void ButtonSong_Click(object sender, RoutedEventArgs e)
        {
            ShowSongDialog(null, true);
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


        private void SliderColorValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte color = (byte)SliderColor.Value;

            if (selectedInstruction != null)
                selectedInstruction.parameters[1] = color;
            ListboxProgram.Items.Refresh();

            // 0, 255, 0 Green
            // 255, 255, 0 Yellow
            // 255, 0, 0 Red

            Color RGBColor;
            if (color < 128)
                RGBColor = Color.FromRgb((byte)(color * 2), 255, 30);
            else RGBColor = Color.FromRgb(255, (byte)(255 - (color - 128) * 2), 0);

            if (rectang != null)
            {
                rectang.Fill = new SolidColorBrush(RGBColor);
            }

        }

        private void SliderBrightChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var r = (byte)SliderIntensity.Value;

            if (selectedInstruction != null)
                selectedInstruction.parameters[2] = r;
            ListboxProgram.Items.Refresh();
        }


        private void CheckBoxLed_CheckChanged(object sender, RoutedEventArgs e)
        {
            int ledBitsValue = 0;

            if (CheckBoxLedPlay.IsChecked.Value)
            {
                ledBitsValue += 2;
            }

            if (CheckBoxLedAdvance.IsChecked.Value)
            {
                ledBitsValue += 8;
            }

            selectedInstruction.parameters[0] = ledBitsValue;
            ListboxProgram.Items.Refresh();
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
            Translator.WriteSource(Translator.SourceType.Microcontroller, cCode);
            Translator.WriteSource(Translator.SourceType.Emulator, cCode);
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Mission Science iRobots\nUSC CSCI-577 Team 07");
        }








    }
}

