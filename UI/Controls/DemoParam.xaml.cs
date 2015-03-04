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
//using System.Windows.Forms;
//using System.String;

namespace iRobotGUI.Controls
{
	/// <summary>
	/// Interaction logic for Demo.xaml
	/// </summary>
	public partial class DemoParam : BaseParamControl
	{

		public Instruction _ins;

		public DemoParam()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Handler for selection change
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ComboBox1_SelectionChanged(object sender, RoutedEventArgs e)
		{
			 var comboBox = sender as ComboBox;

			// get the number index of selected item
			
		
			if (comboBox.SelectedIndex != -1)
			{
				int num = comboBox.SelectedIndex;
				if (num == 10)
				{
					num = -1;
				};
				CurrentIns.parameters[0] = num;
			}             
		   
		}
		/// <summary>
		/// This sets menu item when ComboBox1 loads
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ComboBox1_Loaded(object sender, RoutedEventArgs e)
		{
			var comboBox = sender as ComboBox;
			//Make the first item selected:
			comboBox.SelectedIndex = 0;
		}
		private void OnHover(object sender, RoutedEventArgs e)
		{
		}

		public Instruction CurrentIns
		{
			get
			{
				return _ins;
			}
			set
			{
				_ins = value;

				ComboBox1.SelectedIndex = CurrentIns.parameters[0]; /// ComboBox1.FindName(value);
			}
		}

		private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}
}
