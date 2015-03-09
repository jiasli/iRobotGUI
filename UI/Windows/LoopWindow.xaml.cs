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
using iRobotGUI.Util;

namespace iRobotGUI.Windows
{

	/// <summary>
	/// Interaction logic for LoopWindow.xaml
	/// </summary>
	public partial class LoopWindow : Window
	{
		InsCondition condition;

		public LoopWindow()
		{
			condition = new InsCondition();
			InitializeComponent();
			UpdateConditionWindow();
		}

		private void ComboBoxSensor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			condition.sensor = ComboBoxSensor.SelectedIndex;
			UpdateConditionWindow();
		}

		private void ComboBoxOperator_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			condition.op = ComboBoxOperator.SelectedIndex;
			UpdateConditionWindow();
		}

		private void TextBoxNum_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!Int32.TryParse(TextBoxNum.Text, out condition.num))
			{
				MessageBox.Show("Only numbers are permitted.");
			}
			else
			{
				UpdateConditionWindow();
			}
		}

		private void UpdateConditionWindow()
		{
			if (LabelCondition!=null)
			{
				LabelCondition.Content = String.Format("sensor[{0}] {1} {2}",
				condition.sensor,
				Operator.GetOperatorTextSymbol(condition.op),
				condition.num);
			}		
		}
	}
}
