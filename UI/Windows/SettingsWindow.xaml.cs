using iRobotGUI.Properties;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace iRobotGUI
{
	/// <summary>
	/// Interaction logic for SettingWindow.xaml
	/// </summary>
	/// <remarks>
	/// How to bind to WPF application resources and settings?
	/// http://khason.net/blog/quick-wpf-tip-how-to-bind-to-wpf-application-resources-and-settings/
	/// Where are the Properties.Default.Settings stored?
	/// http://stackoverflow.com/questions/982354/where-are-the-properties-default-settings-stored
	/// </remarks>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
		
			emulatorCom.ComPort = Settings.Default.EmulatorComPort;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			Settings.Default.EmulatorComPort = emulatorCom.ComPort;
			Settings.Default.Save();
			base.OnClosing(e);
		}

		private void buttonBrowse_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			dialog.Description = "Please select the Emulator root folder";
			dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
			if ((bool)dialog.ShowDialog(this))
				textBoxEmulatorPath.Text = dialog.SelectedPath;
		}

		private void buttonReset_Click(object sender, RoutedEventArgs e)
		{
			Settings.Default.Reset();
			SetDefaultEmulatorPath();
		}

		private void buttonShowInExplorer_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("explorer.exe", Settings.Default.EmulatorPath);
		}

		/// <summary>
		/// Set the default emualtor path. Since the emualtor is shipped with the release package,
		/// this method gets the directory where the exe file locates and combines it with MCEmulator.
		/// </summary>
		public static void SetDefaultEmulatorPath()
		{
			Settings.Default.EmulatorPath =
				System.IO.Path.Combine(
				System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
				"MCEmulator");
			Settings.Default.Save();
		}
	}
}
