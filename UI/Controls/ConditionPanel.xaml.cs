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
	public partial class ConditionPanel : UserControl
	{
		InsCondition condition;

		public ConditionPanel()
		{
			condition = new InsCondition();

			InitializeComponent();
			comboBoxSensor.ItemsSource = Sensor.sensorList;

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
				comboBoxSensor.SelectedIndex = condition.sensor;
				comboBoxOperator.SelectedIndex = condition.op;
				textBoxNum.Text = condition.num.ToString();
			}
			get
			{
				return condition;
			}
		}


		private void comboBoxSensor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			condition.sensor = (comboBoxSensor.SelectedItem as Sensor).Number;
			UpdateConditionLabel();
		}

		private void comboBoxOperator_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			condition.op = comboBoxOperator.SelectedIndex;
			UpdateConditionLabel();
		}

		private void textBoxNum_TextChanged(object sender, TextChangedEventArgs e)
		{
			condition.num = Convert.ToInt32(textBoxNum.Text);
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

		private void textBoxNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed(e.Text);
		}

		private static bool IsTextAllowed(string text)
		{
			Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
			return !regex.IsMatch(text);
		}
	}
}
