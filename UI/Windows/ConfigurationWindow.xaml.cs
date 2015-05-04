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
	public partial class ConfigurationWindow : Window
	{
		public static string[] ports;

		public ConfigurationWindow()
		{
			InitializeComponent();

			// COM Port
			comMC.ComPort = Settings.Default.MicrocontrollerComPort;

			// Firmware
			switch (Settings.Default.FirmwareVersion)
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
				default:
					radio1.IsChecked = true;
					break;
			}
		}

		private void buttonOk_Click(object sender, RoutedEventArgs e)
		{
			// COM Port
			Settings.Default.MicrocontrollerComPort = comMC.ComPort;

			// Firmware
			if (radio0.IsChecked ?? false)
				Settings.Default.FirmwareVersion = WinAvrConfiguation.STK500;
			else if (radio1.IsChecked ?? false)
				Settings.Default.FirmwareVersion = WinAvrConfiguation.STK500V1;
			else if (radio2.IsChecked ?? false)
				Settings.Default.FirmwareVersion = WinAvrConfiguation.STK500V2;

			Settings.Default.Save();

			DialogResult = true;
		}

	}
}
