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
	[Obsolete("To simplify the condition, the user only changes the sensor. Use SensorSelector instead. IF and LOOP use different condition expression.", true)]
	/// <summary>
	/// Interaction logic for ConditionPanel.xaml
	/// </summary>
	public partial class ConditionPanel : UserControl
	{
		InsCondition condition;

		public ConditionPanel()
		{
			condition = new InsCondition();
			InitializeComponent();
			UpdateConditionLabel();
		}

		/// <summary>
		/// Set the condition instruction of the panel and update the controls. This is the ony way
		/// to pass information between ConditionPanel and its owner.
		/// </summary>
		public InsCondition Condition
		{
			set
			{
				condition = value;

				comboBoxSensor.SelectedIndex = Array.IndexOf(comboBoxIndexSensorMapping, value.sensor);
				if (condition.num == 1)
				{
					radioButtonTrue.IsChecked = true;
				}
				else
				{
					radioButtonFalse.IsChecked = true;
				}
			}
			get
			{
				condition.op = Operator.EQUAL;
				return condition;
			}
		}

		private int[] comboBoxIndexSensorMapping = new int[]
		{
			0,1,2,3,4,5,6,16
		};

		private void comboBoxSensor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			condition.sensor = comboBoxIndexSensorMapping[comboBoxSensor.SelectedIndex];
			UpdateConditionLabel();
		}

		private void UpdateConditionLabel()
		{
			// label may not be initialized.
			if (labelCondition != null)
			{
				labelCondition.Content = String.Format("sensor[{0}] {1} {2}",
					condition.sensor,
					Operator.GetOperatorTextSymbol(condition.op),
					condition.num);
			}
		}

		private void radioButtonTrue_Checked(object sender, RoutedEventArgs e)
		{
			condition.num = 1;
			UpdateConditionLabel();
		}

		private void radioButtonFalse_Checked(object sender, RoutedEventArgs e)
		{
			condition.num = 0;
			UpdateConditionLabel();
		}
	}
}
