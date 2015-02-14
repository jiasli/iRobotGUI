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

        private void ConfigCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion



        #region private method

        private void LoadProgram(string fileName)
        {
            program = new HLProgram(File.ReadAllText(fileName));
            UpdateProgramPanel();
        }

        private void UpdateProgramPanel()
        {
            textBoxCode.Text = program.ToString();
            listbox.Items.Clear();
            foreach (Instruction i in program.GetInstructionList())
            {
                listbox.Items.Add(i);
            }

        }


        #endregion






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
                        newIns = new Instruction(Instruction.LED + " 0,128,128");
                        break;
                    case Instruction.SONG_DEF:
                        newIns = showSongDialog(true);
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
                listbox.SelectedItem = newIns;
                ChangeParameterPanel(newIns);
            }
        }

        #endregion


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listbox.SelectedItem != null)
            {
                selectedInstructionIndex = listbox.SelectedIndex;
                selectedInstruction = listbox.SelectedItem as Instruction;
                ChangeParameterPanel(selectedInstruction);
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
                    ColorSlider.Value = ins.parameters[1];
                    BrightSlider.Value = ins.parameters[2];
                    switch (ins.parameters[0])
                    {
                        case 0:
                            led1.IsChecked = false;
                            led2.IsChecked = false;
                            break;
                        case 2:
                            led1.IsChecked = true;
                            led2.IsChecked = false;
                            break;
                        case 8:
                            led1.IsChecked = false;
                            led2.IsChecked = true;
                            break;
                        case 10:
                            led1.IsChecked = true;
                            led2.IsChecked = true;
                            break;
                    }
                    //textBoxDistance.Text = ins._parameters[0].ToString();
                    //textBoxTime.Text = ins._parameters[1].ToString();
                }
            }
        }



        private void BuildAndLoad(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(System.IO.Directory.GetCurrentDirectory());
            string template = File.ReadAllText(@"template.c");
            File.WriteAllText(@"output.c", template);
            MessageBox.Show("Compiling and loading succeeded");
        }



        private void buttonForward_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.Write("Click");
            MessageBox.Show("click");
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            if (e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(image, "Forward", DragDropEffects.Copy);
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

        //handler for the color led slider:
        private void ColorSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte color = (byte)ColorSlider.Value;
            if (Color_val != null)
            {
                Color_val.Text = color.ToString();
                if (selectedInstruction != null)
                    selectedInstruction.parameters[1] = color;
                listbox.Items.Refresh();
            };

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

        private void BrightSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var r = (byte)BrightSlider.Value;
            if (brightness_val != null)
            {
                brightness_val.Text = r.ToString();
                if (selectedInstruction != null)
                    selectedInstruction.parameters[2] = r;
                listbox.Items.Refresh();
            };
        }

        //hadlers for LED checkboxes:
        private void led_check_handler(object sender, RoutedEventArgs e)
        {
            if (selectedInstruction != null)
            {
                if (led1.IsChecked.Value && led2.IsChecked.Value)
                {
                    selectedInstruction.parameters[0] = 10;
                }
                if (led1.IsChecked.Value && !led2.IsChecked.Value)
                {
                    selectedInstruction.parameters[0] = 2;
                }
                if (!led1.IsChecked.Value && led2.IsChecked.Value)
                {
                    selectedInstruction.parameters[0] = 8;
                }
                if (!led1.IsChecked.Value && !led2.IsChecked.Value)
                {
                    selectedInstruction.parameters[0] = 0;
                }
            }
            listbox.Items.Refresh();

        }

        // led uncheck handler
        private void led_uncheck_handler(object sender, RoutedEventArgs e)
        {
            if (selectedInstruction != null)
            {
                if (led1.IsChecked.Value && led2.IsChecked.Value)
                {
                    selectedInstruction.parameters[0] = 10;
                }
                if (led1.IsChecked.Value && !led2.IsChecked.Value)
                {
                    selectedInstruction.parameters[0] = 2;
                }
                if (!led1.IsChecked.Value && led2.IsChecked.Value)
                {
                    selectedInstruction.parameters[0] = 8;
                }
                if (!led1.IsChecked.Value && !led2.IsChecked.Value)
                {
                    selectedInstruction.parameters[0] = 0;
                }
            };

            listbox.Items.Refresh();
        }

        private void ButtonSong_Click(object sender, RoutedEventArgs e)
        {
            showSongDialog(true);
        }

        private Instruction showSongDialog(bool isNewIns)
        {
            // Instantiate the dialog box
            SongWindow dlg = new SongWindow();

            // Configure the dialog box
            dlg.Owner = this;
            Instruction result = null;

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

        private void textBoxDistance_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateField(sender as TextBox, selectedInstruction, 0);
        }

        private void textBoxTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateField(sender as TextBox, selectedInstruction, 1);
        }

        private void textBoxAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateField(sender as TextBox, selectedInstruction, 0);
        }

        /// <summary>
        /// Update a parameter of Instruction according to the data in a TextBox
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="ins"></param>
        /// <param name="index"></param>
        private void updateField(TextBox tb, Instruction ins, int index)
        {
            if (ins == null) return;
            // Default
            if (tb != null)
            {
                if (!string.IsNullOrEmpty(tb.Text))
                {
                    ins.parameters[index] = Convert.ToInt32(tb.Text);
                }
                listbox.Items.Refresh();
            }
        }



    }
}

