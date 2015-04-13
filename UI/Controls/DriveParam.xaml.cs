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
using System.Text.RegularExpressions;

namespace iRobotGUI.Controls
{
	/// <summary>
	/// Interaction logic for RotateParam.xaml
	/// </summary>
	public partial class DriveParam : BaseParamControl
	{
		public DriveParam()
		{
			InitializeComponent();
		}

		public override Instruction Ins
		{
			get
			{
				base.Ins.paramList[0] = (int)(sliderVelocity.Value * 10);
				base.Ins.paramList[1] = steer.Radius;
				return base.Ins;
			}
			set
			{
				base.Ins = value;

				// Set velocity slider
				sliderVelocity.Value = (value.paramList[0]) / 10;

				// Set radius for steering wheel.
				steer.Radius = value.paramList[1];
				steer.Angle = SteeringParam.RadiusToAngle(value.paramList[1]);
			}
		}


		private void buttonStraight_Click(object sender, RoutedEventArgs e)
		{
			steer.Angle = 0;
			steer.Radius = Instruction.STRAIGHT1;
		}
	}
}
