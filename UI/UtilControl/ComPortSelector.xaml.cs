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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iRobotGUI
{
	/// <summary>
	/// Interaction logic for ComPortSelector.xaml
	/// </summary>
	public partial class ComPortSelector : UserControl
	{
		public string ComPort
		{
			get
			{
				return comboBoxCom.SelectedItem as string;
			}
			set
			{
				comboBoxCom.SelectedItem = value;
			}
		}

		public ComPortSelector()
		{
			InitializeComponent();
			
			// Get all connected ports and select the first one.
			string[] ports = System.IO.Ports.SerialPort.GetPortNames();
			comboBoxCom.ItemsSource = ports;
			if (ports.Length > 0) comboBoxCom.SelectedIndex = 0;
		}

		private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
		{
			string[] ports = System.IO.Ports.SerialPort.GetPortNames();

			int previousIndex = comboBoxCom.SelectedIndex;
			comboBoxCom.ItemsSource = ports;

			// Select the previously selected item again.
			if (previousIndex < 0) comboBoxCom.SelectedIndex = 0;
			else if (previousIndex < ports.Length) comboBoxCom.SelectedIndex = previousIndex;
			else comboBoxCom.SelectedIndex = ports.Length - 1;
		}

		private void ButtonDeviceManager_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("devmgmt.msc");
		}

	}
}
