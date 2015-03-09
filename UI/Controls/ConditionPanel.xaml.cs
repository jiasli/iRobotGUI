using iRobotGUI.Util;
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
			UpdateConditionLabel();
		}		


		private void comboBoxSensor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			condition.sensor = comboBoxSensor.SelectedIndex;
			UpdateConditionLabel();
		}

		private void comboBoxOperator_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			condition.op = comboBoxOperator.SelectedIndex;
			UpdateConditionLabel();
		}

		private void textBoxNum_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!Int32.TryParse(textBoxNum.Text, out condition.num))
			{
				MessageBox.Show("Only numbers are permitted.");
			}
			else
			{
				UpdateConditionLabel();
			}
		}

		private void UpdateConditionLabel()
		{
			if (labelCondition!=null)
			{
				labelCondition.Content = String.Format("sensor[{0}] {1} {2}",
				condition.sensor,
				Operator.GetOperatorTextSymbol(condition.op),
				condition.num);
			}		
		}
	}
}
