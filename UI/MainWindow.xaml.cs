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
using iRobotGUI.Properties;

namespace iRobotGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// Implementing a custom WPF Command
		// http://www.wpf-tutorial.com/commands/implementing-custom-commands/
		// Defining MenuItem Shortcuts
		// http://stackoverflow.com/questions/4682915/defining-menuitem-shortcuts

		public static RoutedCommand TranslateToMicrocontrollerCCmd = new RoutedUICommand("Translate to Microcontroller C File", "emulator", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.F4) });
		public static RoutedCommand BuildCmd = new RoutedUICommand("Build", "Build", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.F5) });
		public static RoutedCommand CleanCmd = new RoutedUICommand("Clean", "Clean", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.F7) });
		public static RoutedCommand LoadCmd = new RoutedUICommand("Load", "Load", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.F6) });

		public static RoutedCommand TranslateToEmulatorCCmd = new RoutedUICommand("Translate to Emulator C File", "emulator", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.F10) });
		public static RoutedCommand BuildEmulatorCmd = new RoutedUICommand("Build Emulator", "emulator", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.F11) });
		public static RoutedCommand RunEmulatorCmd = new RoutedUICommand("Run Emulator", "emulator", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.F12) });

		public static RoutedCommand OpenSourceCmd = new RoutedUICommand("Open Source File", "srcfile", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift) });
		public static RoutedCommand SettingCmd = new RoutedUICommand("Preferences and Emulator", "Setting", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });
		public static RoutedCommand WinAvrConfigCmd = new RoutedUICommand("WinAVR Configuation", "avrconfig", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Shift) });

		public static RoutedCommand LoadToMicrocontrollerCmd = new RoutedUICommand("Load to Microcontroller", "micro", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.R, ModifierKeys.Control) });
		public static RoutedCommand RunOnEmulatorCmd = new RoutedUICommand("Run on Emulator", "avrconfig", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.R, ModifierKeys.Control | ModifierKeys.Shift) });




		private string igpFile = "";

		public MainWindow()
		{
			this.CommandBindings.Add(new CommandBinding(TranslateToMicrocontrollerCCmd, TranslateToMicrocontrollerCCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(BuildCmd, BuildCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(CleanCmd, CleanCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(LoadCmd, LoadCmdExecuted));

			this.CommandBindings.Add(new CommandBinding(TranslateToEmulatorCCmd, TranslateToEmulatorCCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(BuildEmulatorCmd, BuildEmulatorCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(RunEmulatorCmd, RunEmulatorCmdExecuted));

			this.CommandBindings.Add(new CommandBinding(LoadToMicrocontrollerCmd, LoadToMicrocontrollerCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(RunOnEmulatorCmd, RunOnEmulatorCmdExecuted));

			this.CommandBindings.Add(new CommandBinding(OpenSourceCmd, OpenSrcCmdExecuted, OpenSrcCmdCanExecute));
			this.CommandBindings.Add(new CommandBinding(WinAvrConfigCmd, WinAvrConfigCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(SettingCmd, SettingCmdExecuted));



			InitializeComponent();

			// http://stackoverflow.com/questions/837488/how-can-i-get-the-applications-path-in-a-net-console-application
			// If emulator's path is not set, use the default path.
			if (Properties.Settings.Default.EmulatorPath == "")
			{
				SettingsWindow.SetDefaultEmulatorPath();
			}


			if (!Directory.Exists(@"cprogram\"))
			{
				MessageBox.Show("The C template is missing, try re-install the program.", "Program Broken", MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
			}
			else
			{
				// Set the current folder to cprogram
				Directory.SetCurrentDirectory(@"cprogram\");
			}


			programList.Program = new HLProgram();

			// Status bar
			textBlockStatus.Text = Directory.GetCurrentDirectory();

			UpdateStatusBarComPort();
		}


		#region Commands

		#region Microcontroller

		private void TranslateToMicrocontrollerCCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			WinAvrConnector.TranslateToC(programList.Program);

			if (Properties.Settings.Default.OpenCCode) Process.Start(WinAvrConnector.MicrocontrollerOutputFile);
		}

		// Create(New), Save and Load. Traceability: WC_3305: As an ESS, I can create, save and load program files.
		private void BuildCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				WinAvrConnector.Make();
			}
			catch (ComPortException)
			{
				MessageBox.Show("COM port is not selected. Unable to build.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (Exception)
			{
				ShowWinAvrError();
			}
		}

		private void LoadCmdExecuted(object sender, ExecutedRoutedEventArgs e)
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

		private void CleanCmdExecuted(object sender, ExecutedRoutedEventArgs e)
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

		#endregion

		#region Emulator

		private void TranslateToEmulatorCCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				EmulatorConnector.TranslateToC(programList.Program);
			}
			catch (Exception)
			{
				MessageBox.Show("Emulator path or COM Port invalid. Use Build -> Preferences and Emulator to select the correct path or COM Port.",
					"Invalid Parameters", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private void BuildEmulatorCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				EmulatorConnector.Build();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to build the emulator. Check if the emulator path is correct. \n" + ex.ToString(),
					"Unable to build the emulator.", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private void RunEmulatorCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				EmulatorConnector.Run();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to run the emulator. Check if the emulator path is correct and if the emulator is built correctly. \n" + ex.ToString(),
					"Unable to run the emulator.", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		#endregion

		#region All-in-one commands


		/// <summary>
		/// Translate igp file to C file and compile it to executable program using WinAVR and load it to iRobot.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LoadToMicrocontrollerCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			TranslateToMicrocontrollerCCmd.Execute(null, this);
			BuildCmd.Execute(null, this);
			LoadCmd.Execute(null, this);
		}

		private void RunOnEmulatorCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			TranslateToEmulatorCCmd.Execute(null, this);
			BuildEmulatorCmd.Execute(null, this);
			RunEmulatorCmd.Execute(null, this);
		}


		#endregion

		/// <summary>
		/// Create a new HLProgram and pass it to ProgramList
		/// </summary>
		/// <param name="target"></param>
		/// <param name="e"></param>
		void NewCmdExecuted(object target, ExecutedRoutedEventArgs e)
		{
			igpFile = null;
			programList.Program = new HLProgram();
			textBlockStatus.Text = Directory.GetCurrentDirectory();
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
				OpenProgram(dlg.FileName);
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

		/// <summary>
		/// Open the SettingsWindow
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SettingCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SettingsWindow sw = new SettingsWindow();

			sw.Owner = this;
			sw.ShowDialog();

			UpdateStatusBarComPort();
		}

		/// <summary>
		/// Show Configuration Window
		/// </summary>
		private void WinAvrConfigCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			// Instantiate the dialog box
			ConfigurationWindow dlg = new ConfigurationWindow();


			// Configure the dialog box
			dlg.Owner = this;

			// Open the dialog box modally 
			dlg.ShowDialog();
			if (dlg.DialogResult == true)
			{
				UpdateStatusBarComPort();
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
				// Open document 
				igpFile = filePath;
				textBlockStatus.Text = igpFile;

				string proStr = File.ReadAllText(filePath);
				programList.Program = new HLProgram(proStr);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		/// <summary>
		/// Save program from file.
		/// </summary>
		/// <param name="filePath"></param>
		private void SaveProgram(string filePath)
		{
			try
			{
				// Set the current path to the file that is saved.
				igpFile = filePath;
				textBlockStatus.Text = igpFile;

				string proStr = programList.Program.ToString(true);
				File.WriteAllText(filePath, proStr);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void ShowWinAvrError()
		{
			MessageBox.Show("Fail to execute. Check if WinAVR is installed correctly.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void UpdateStatusBarComPort()
		{
			// Microcontroller
			string mcCom;
			if (Properties.Settings.Default.MicrocontrollerComPort == "") mcCom = "NA";
			else mcCom = Settings.Default.MicrocontrollerComPort;
			textBlockMicrocontrollerComPort.Text = "Microcontroller COM Port: " + mcCom;

			// Emulator
			string emCom;
			if (Properties.Settings.Default.EmulatorComPort == "") emCom = "NA";
			else emCom = Settings.Default.EmulatorComPort;
			textBlockEmulatorComPort.Text = "Emulator COM Port: " + emCom;

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
			HLProgram program = programList.Program;
			textBoxSource.Text = program.ToString();
		}

		private void buttonLoadIntoGraph_Click(object sender, RoutedEventArgs e)
		{
			programList.Program = new HLProgram(textBoxSource.Text);
		}

		#endregion


		#region Menu callbacks

		private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Mission Science iRobots\nUSC CSCI-577 Team 07");
		}

		private void MenuItemShowCCode_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(WinAvrConnector.MicrocontrollerOutputFile);
		}

		private void MenuItemShowDebugPanel_Checked(object sender, RoutedEventArgs e)
		{
			if (columnDefinitionDebug != null)
				columnDefinitionDebug.Width = new GridLength(250);
			this.Width = 700;
		}

		private void MenuItemShowDebugPanel_Unchecked(object sender, RoutedEventArgs e)
		{
			if (columnDefinitionDebug != null)
				columnDefinitionDebug.Width = new GridLength(0);
			this.Width = 450;
		}
		private void MenuItemRevealInExplorer_Click(object sender, RoutedEventArgs e)
		{
			// Open current folder in explorer.exe
			// http://stackoverflow.com/questions/1132422/open-a-folder-using-process-start
			// Windows Explorer Command-Line Options
			// http://support.microsoft.com/en-us/kb/152457
			if (igpFile == null) Process.Start("explorer.exe", ".");
			else Process.Start("explorer.exe", "/select," + igpFile);
		}


		#endregion Menu callbacks

		#region Window callbacks
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// No file is open
			if (string.IsNullOrEmpty(igpFile))
			{
				if (programList.Program.ToString() == "") return;
			}
			else
			{
				if (programList.Program.ToString(true) == File.ReadAllText(igpFile)) return;
			}

			MessageBoxResult result = MessageBox.Show("Save the program?", "Exit", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				ApplicationCommands.Save.Execute(null, this);
			}
			else if (result == MessageBoxResult.Cancel)
			{
				e.Cancel = true;
			}
		}
		#endregion Window callbacks

		#region Control Callbacks
		private void InstructionPanel_AddNewInstruction(string opcode)
		{
			programList.AddNewInstruction(opcode);
		}

		// For debugging
		private void programList_ProgramChanged()
		{
			HLProgram program = programList.Program;
			textBoxSource.Text = program.ToString();
		}

		private void programList_ClipboardChanged()
		{
			textBoxClipboard.Text = Clipboard.GetText();
		}

		private void programList_SelectedInstructionChanged(string insString)
		{
			textBoxSelectedInstruction.Text = insString;
		}
		#endregion

		private void StatusBarItemMC_MouseDown(object sender, MouseButtonEventArgs e)
		{
			WinAvrConfigCmd.Execute(null, this);
		}

		private void StatusBarItemEM_MouseDown(object sender, MouseButtonEventArgs e)
		{
			SettingCmd.Execute(null, this);
		}
	}
}