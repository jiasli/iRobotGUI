using iRobotGUI.Properties;
using Ookii.Dialogs.Wpf;
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
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			Settings.Default.Save();
			base.OnClosing(e);
		}

		private void buttonBrowse_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			dialog.Description = "Please select a folder.";
			dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
			if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
				MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
			if ((bool)dialog.ShowDialog(this))
				MessageBox.Show(this, "The selected folder was: " + dialog.SelectedPath, "Sample folder browser dialog");           
		}
	}
}
