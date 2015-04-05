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
		// Implementing a custom WPF Command
		// http://www.wpf-tutorial.com/commands/implementing-custom-commands/
		// Defining MenuItem Shortcuts
		// http://stackoverflow.com/questions/4682915/defining-menuitem-shortcuts

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
		public static RoutedCommand SettingCmd = new RoutedUICommand("Setting", "Setting", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });
		public static RoutedCommand WinAvrConfigCmd = new RoutedUICommand("WinAVR Configuation", "avrconfig", typeof(Window),
			new InputGestureCollection { new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Shift) });


		public const string MicrocontrollerTemplate = "mc_t.c";
		public const string MicrocontrollerOutputFile = "mc_o.c";
		public const string EmulatorTemplate = "em_t.c";
		public const string EmulatorOutputFile = "em_o.c";

		private string igpFile;

		private HLProgram program;

		/// <summary>
		/// Get or set the current HLProgram under editing. When set, the ProgramList will also be updated.
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
				programList1.Program = value;
			}
		}

		public MainWindow()
		{
			this.CommandBindings.Add(new CommandBinding(BuildCmd, BuildCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(CleanCmd, CleanCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(LoadCmd, LoadCmdExecuted));

			this.CommandBindings.Add(new CommandBinding(TranslateToEmulatorCCmd, TranslateToEmulatorCCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(BuildEmulatorCmd, BuildEmulatorCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(RunEmulatorCmd, RunEmulatorCmdExecuted));

			this.CommandBindings.Add(new CommandBinding(OpenSourceCmd, OpenSrcCmdExecuted, OpenSrcCmdCanExecute));
			this.CommandBindings.Add(new CommandBinding(WinAvrConfigCmd, WinAvrConfigCmdExecuted));
			this.CommandBindings.Add(new CommandBinding(SettingCmd, SettingCmdExecuted));

			InitializeComponent();

			// Set the current folder to cprogram
			Directory.SetCurrentDirectory(@".");

			program = new HLProgram();
			programList1.Program = program;

			textBlockStatus.Text = Directory.GetCurrentDirectory();
		}






		#region Commands

		#region Microcontroller
		// Create(New), Save and Load. Traceability: WC_3305: As an ESS, I can create, save and load program files.
		private void BuildCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				WinAvrConnector.Make();
			}
			catch (Exception ex)
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
			string emulatorPath = Properties.Settings.Default.EmulatorPath;
			if (string.IsNullOrEmpty(emulatorPath))
			{
				MessageBox.Show("Emulator path not invalid. Use Build -> Setting to select the correct path.", "Invalid Emulator Path", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			string cCode = Translator.Translate(Program);
			string templateFullPath = System.IO.Path.Combine(emulatorPath, "MCEmulatorFramework", EmulatorTemplate);
			string outputFullPath = System.IO.Path.Combine(emulatorPath, "MCEmulatorFramework", EmulatorOutputFile);

			Translator.GenerateCSource(templateFullPath, outputFullPath, cCode);

		}

		private void BuildEmulatorCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			string emulatorPath = Properties.Settings.Default.EmulatorPath;
			string buildBatName = "MSMake.bat";
			string buildBatFullPath = System.IO.Path.Combine(emulatorPath, buildBatName);
			// string solutionName = "MCEmulator.sln";
			// string solutionFullName = System.IO.Path.Combine(emulatorPath, solutionName);

			//Process.Start(@"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe", solutionFullName);

			try
			{
				Process process = Process.Start(buildBatFullPath);
				process.WaitForExit();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

		}

		private void RunEmulatorCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			string emulatorPath = Properties.Settings.Default.EmulatorPath;
			string exeName = @"Debug\MCEmulator.exe";
			string exeFullPath = System.IO.Path.Combine(emulatorPath, exeName);
			// string solutionName = "MCEmulator.sln";
			// string solutionFullName = System.IO.Path.Combine(emulatorPath, solutionName);

			//Process.Start(@"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe", solutionFullName);

			try
			{
				Process process = Process.Start(exeFullPath);
				process.WaitForExit();
			}
			catch (Exception ex)
			{
				MessageBox.Show(exeFullPath + "\n" + ex.ToString());
			}

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
			Program = new HLProgram();
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
			dlg.Config = WinAvrConnector.config;

			// Open the dialog box modally 
			dlg.ShowDialog();
			if (dlg.DialogResult == true)
			{
				// MessageBox.Show(WinAvrConnector.config.ToString());

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
				Program = new HLProgram(proStr);
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
				string proStr = programList1.Program.ToString();
				Program = programList1.Program;
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
			textBoxSource.Text = program.ToString();
		}

		private void buttonLoadIntoGraph_Click(object sender, RoutedEventArgs e)
		{
			Program = new HLProgram(textBoxSource.Text);
		}

		#endregion


		#region Menu callbacks

		private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Mission Science iRobots\nUSC CSCI-577 Team 07");
		}

		private void MenuItemShowCCode_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(MicrocontrollerOutputFile);
		}

		private void MenuItemShowDebugPanel_Checked(object sender, RoutedEventArgs e)
		{
			if (columnDefinitionDebug != null)
				columnDefinitionDebug.Width = new GridLength(250);
		}

		private void MenuItemShowDebugPanel_Unchecked(object sender, RoutedEventArgs e)
		{
			if (columnDefinitionDebug != null)
				columnDefinitionDebug.Width = new GridLength(0);
		}
		private void MenuItemShowSrcFolder_Click(object sender, RoutedEventArgs e)
		{
			// Open current folder in explorer.exe
			// http://stackoverflow.com/questions/1132422/open-a-folder-using-process-start
			Process.Start("explorer.exe", ".");
		}

		private void MenuItemTranslate_Click(object sender, RoutedEventArgs e)
		{
			string cCode = Translator.Translate(Program);

			Translator.GenerateCSource(MicrocontrollerTemplate, MicrocontrollerOutputFile, cCode);

			if (Properties.Settings.Default.OpenCCode) Process.Start(MicrocontrollerOutputFile);
		}

		/// <summary>
		/// Translate igp file to C file and compile it to executable program using WinAVR and load it to iRobot.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemTranslateBuildLoad_Click(object sender, RoutedEventArgs e)
		{
			MenuItemTranslate_Click(sender, e);
			BuildCmd.Execute(null, this);
			LoadCmd.Execute(null, this);
		}
		#endregion

	}
}

