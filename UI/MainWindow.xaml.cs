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

        public MainWindow()
        {

            InitializeComponent();
            Directory.SetCurrentDirectory(@".");
            program = new HLProgram();
            ProgramList1.Program = program;
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
            try
            {
                ProgramList1.Program = program = new HLProgram(File.ReadAllText(fileName));                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);              
            }          
            
        }


        /*
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
         */




        #endregion



              

        #region Control Callbacks

       
        private void TextBoxDistance_TextChanged(object sender, TextChangedEventArgs e)
        {
            //UpdateParamFromTextBox(sender as TextBox, selectedInstruction, 0);
        }

        private void TextBoxTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            //UpdateParamFromTextBox(sender as TextBox, selectedInstruction, 1);
        }

        private void TextBoxAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            //UpdateParamFromTextBox(sender as TextBox, selectedInstruction, 0);
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
            LoadProgram("song.igp");
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


      








    }
}

