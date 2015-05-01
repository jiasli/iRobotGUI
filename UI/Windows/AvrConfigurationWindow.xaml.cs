using iRobotGUI.Properties;
using iRobotGUI.WinAvr;
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
	/// Interaction logic for ComWindow.xaml
	/// </summary>
	public partial class AvrConfigurationWindow : Window
	{
		private WinAvrConfiguation _config;
		public static string[] ports;

		public WinAvrConfiguation Config
		{
			set
			{
				_config = value;

				switch (value.firmwareVersion)
				{
					case WinAvrConfiguation.STK500:
						radio0.IsChecked = true;
						break;
					case WinAvrConfiguation.STK500V1:
						radio1.IsChecked = true;
						break;
					case WinAvrConfiguation.STK500V2:
						radio2.IsChecked = true;
						break;
				}
			}
			get
			{
				return _config;
			}
		}

		public AvrConfigurationWindow()
		{
			ports = System.IO.Ports.SerialPort.GetPortNames();

			InitializeComponent();

			// https://msdn.microsoft.com/en-us/library/system.windows.controls.primitives.selector.selectedindex(v=vs.110).aspx
			// If you set SelectedIndex to a value equal or greater than the number of child elements, the value is ignored.
			// Try to select the first one.
			if (comboBoxCom.SelectedIndex == -1) comboBoxCom.SelectedIndex = 0;

		}

		private void buttonOk_Click(object sender, RoutedEventArgs e)
		{
			_config.comPort = comboBoxCom.Text;

			if (radio0.IsChecked ?? false)
				_config.firmwareVersion = radio0.Content.ToString();
			else if (radio1.IsChecked ?? false)
				_config.firmwareVersion = radio1.Content.ToString();
			else if (radio2.IsChecked ?? false)
				_config.firmwareVersion = radio2.Content.ToString();

			DialogResult = true;
		}

		private void ButtonDeviceManager_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("devmgmt.msc");
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			Settings.Default.Save();
			base.OnClosing(e);
		}

		private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
		{
			ports = System.IO.Ports.SerialPort.GetPortNames();

			int previousIndex = comboBoxCom.SelectedIndex;
			comboBoxCom.ItemsSource = ports;

			// Select the previously selected item again.
			if (previousIndex < 0) comboBoxCom.SelectedIndex = 0;
			else if (previousIndex < ports.Length) comboBoxCom.SelectedIndex = previousIndex;
			else comboBoxCom.SelectedIndex = ports.Length - 1;
		}
	}
}
