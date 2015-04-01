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
	/// Interaction logic for DelayParam.xaml
	/// </summary>
	public partial class DelayParam : BaseParamControl
	{
		public override Instruction Ins
		{
			get
			{
				return base.Ins;
			}
			set
			{
				base.Ins = value;				
				SliderDuration.Value = Ins.paramList[0];
			}
		}
		private void SliderDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (Ins != null)
			{
				Ins.paramList[0] = (int)e.NewValue;
			}
		}
		public DelayParam()
		{
			InitializeComponent();
		}


	}
}
