using iRobotGUI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace iRobotGUI.Controls
{	
	/// <summary>
	/// Interaction logic for ConditionPanel.xaml
	/// </summary>
	public partial class SensorSelector : UserControl
	{
		public SensorSelector()
		{
			InitializeComponent();
		}


		private int[] comboBoxIndexSensorMapping = new int[]
		{
			Sensor.SenBumpDrop,
			Sensor.SenWall,
			Sensor.SenCliffL,
			Sensor.SenCliffFL,
			Sensor.SenCliffFR,
			Sensor.SenCliffR,
			Sensor.SenVWall,
			Sensor.SenState
		};

		public int SelectedSensor
		{
			get
			{
				return comboBoxIndexSensorMapping[comboBoxSensor.SelectedIndex];
			}
			set
			{
				comboBoxSensor.SelectedIndex = Array.IndexOf(comboBoxIndexSensorMapping, value);
			}
		}

	}
}
