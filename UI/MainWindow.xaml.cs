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
using iRobotGUI.WinAvr;
using System.Diagnostics;

namespace iRobotGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static RoutedCommand OpenSourceCmd = new RoutedUICommand("Open Source File", "srcfile", typeof(Window));

		private string cFile = "mc_o.c";
		private string igpFile;
		private HLProgram program;

		public MainWindow()
		{
			InitializeComponent();

			// Set the current folder to cprogram
			Directory.SetCurrentDirectory(@".");

			program = new HLProgram();
			programList1.Program = program;

			textBlockStatus.Text = "new file";
		}

		#region Commands

		// WPF use command binding to handle shortcuts, 
		// See: http://stackoverflow.com/questions/4682915/defining-menuitem-shortcuts

		// Create(New), Save and Load. Traceability: WC_3305: As an ESS, I can create, save and load program files.

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

		/// <summary>
		/// Create a new HLProgram and pass it to ProgramList
		/// </summary>
		/// <param name="target"></param>
		/// <param name="e"></param>
		void NewCmdExecuted(object target, ExecutedRoutedEventArgs e)
		{
			igpFile = null;
			program = new HLProgram();
			programList1.Program = program;
		}

		/// <summary>
		/// Show a open dialog to let user choose the igp file to be loaded
		/// </summary>
		/// <param name="target"></param>
		/// <param name="e"></param>
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
				igpFile = dlg.FileName;
				OpenProgram(igpFile);
				textBlockStatus.Text = igpFile;
			}
		}


		/// <summary>
		/// Save the program as an igp file.
		/// Traceability: WC_3305, As an ESS, I can create, save and load program files.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveAsCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			// Configure save file dialog box
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.InitialDirectory = Directory.GetCurrentDirectory();
			dlg.FileName = igpFile; // Default file name
			dlg.DefaultExt = ".igp"; // Default file extension
			dlg.Filter = "Text documents|*.igp"; // Filter files by extension 

			// Show save file dialog box
			Nullable<bool> result = dlg.ShowDialog();

			// Process save file dialog box results 
			if (result == true)
			{
				// Save document 
				string filename = dlg.FileName;
				SaveProgram(filename);
			}
		}

		/// <summary>
		/// Save the program to an igp file.
		/// Traceability: WC_3305, As an ESS, I can create, save and load program files.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(igpFile))
			{
				// If no file is currently opened, call SaveAs.
				SaveAsCmdExecuted(sender, e);
			}
			else
			{
				SaveProgram(igpFile);
			}
		}
		#endregion



		#region Private Methods
		/// <summary>
		/// Load program from file.
		/// </summary>
		/// <param name="filePath">Path of file.</param>
		private void OpenProgram(string filePath)
		{
			try
			{
				string proStr = File.ReadAllText(filePath);
				program = new HLProgram(proStr);
				programList1.Program = program;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void OpenSrcCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (!String.IsNullOrEmpty(igpFile))
			{
				e.CanExecute = true;
			}
			else
			{
				e.CanExecute = false;
			}
		}

		private void OpenSrcCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Process.Start(igpFile);
		}

		/// <summary>
		/// Save program from file.
		/// </summary>
		/// <param name="filePath"></param>
		private void SaveProgram(string filePath)
		{
			try
			{
				string proStr = programList1.Program.ToString();
				program = programList1.Program;
				File.WriteAllText(filePath, proStr);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void ShowWinAvrError()
		{
			MessageBox.Show("Fail to execute. Check if WinAVR is installed correctly.", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		#endregion



		#region Control Callbacks

		/// <summary>
		/// Load specified igp file [Debug use only] 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonLoadExampleCode_Click(object sender, RoutedEventArgs e)
		{
			OpenProgram("song.igp");
		}

		private void buttonRefreshSource_Click(object sender, RoutedEventArgs e)
		{
			HLProgram program = programList1.Program;
			textBoxSrc.Text = program.ToString();
		}
		#endregion


		#region Menu callbacks
		/// <summary>
		/// Translate igp file to C file and compile it to executable program using WinAVR and load it to iRobot.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemTranslateBuildLoad_Click(object sender, RoutedEventArgs e)
		{
			MenuItemTranslate_Click(sender, e);
			MenuItemBuild_Click(sender, e);
			MenuItemLoad_Click(sender, e);
		}

		private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Mission Science iRobots\nUSC CSCI-577 Team 07");
		}

		private void MenuItemBuild_Click(object sender, RoutedEventArgs e)
		{
			//Window errorWin = new OutputWindow();
			//errorWin.Show();
			//  WinAvrConnector.Clean();

			try
			{
				WinAvrConnector.Make();
			}
			catch (Exception ex)
			{
				ShowWinAvrError();
			}

		}

		private void MenuItemClean_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				File.Delete("C_result.c");
				WinAvrConnector.Clean();
			}
			catch (Exception)
			{
				ShowWinAvrError();
			}
		}

		private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				WinAvrConnector.Load();
			}
			catch (Exception)
			{
				ShowWinAvrError();
			}
		}

		private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
		{
			SettingsWindow sw = new SettingsWindow();

			sw.Owner = this;
			sw.ShowDialog();
		}

		private void MenuItemShowCCode_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(cFile);
		}
		private void MenuItemShowSrcFolder_Click(object sender, RoutedEventArgs e)
		{
			// Open current folder in explorer.exe
			// http://stackoverflow.com/questions/1132422/open-a-folder-using-process-start
			Process.Start("explorer.exe", ".");
		}

		private void MenuItemTranslate_Click(object sender, RoutedEventArgs e)
		{
			string cCode = Translator.TranslateProgram(program);

			Translator.GenerateCSource(Translator.SourceType.Microcontroller, cCode);
			Translator.GenerateCSource(Translator.SourceType.Emulator, cCode);

			if (Properties.Settings.Default.OpenCCode) Process.Start(cFile);
		}
		#endregion
		// textbox input form validation function
		private void number_validation(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}


	}
}

