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
         private double Angle = default(double);
		 private int radius = default(int);
		 private const int STRAIGHT = 32768;
		 private const int MAX_RADIUS = 2000;
		
		 private int roundToInt(double val)
		 {
			 return (int)Math.Round(val, 0, MidpointRounding.AwayFromZero);
		 }
		 private double radiusToAngle(int radius)
		 {
			 if (radius == STRAIGHT)
			 {
				 return 0.0;
			 }
			 double d = (double)radius / (double)MAX_RADIUS;
			 double rad_angle = Math.Acos(d);
			 if (radius >= 0)
			 {
				 return -(rad_angle * 180.0) / Math.PI; ; // ((rad_angle - Math.PI) * 180.0) / Math.PI;  
			 }
			 return 180-((rad_angle * 180.0) / Math.PI); /// return centigrade angle
		 }
		 private int angleToRadius(double angle)
		 {
			 double rad_angle = (angle * Math.PI) / 180; ///convert angle from centigrade to radians
			 if (angle <= 0)
			 {
				 return roundToInt((Math.Cos(rad_angle) * MAX_RADIUS)); ///(int)Math.Round((Math.Cos(rad_angle) * MAX_RADIUS), 0, MidpointRounding.AwayFromZero);
			 };
			 return -roundToInt((Math.Cos(rad_angle) * MAX_RADIUS));
		 }
       
        public DriveParam()
        {
            InitializeComponent();
			this.DataContext = this;
        }
       
        public override Instruction Ins
        {
            get
            {
                return base.Ins;
            }
            set
            {
                base.Ins = value;
				///set velocity slider:
				SliderVelocity.Value = Ins.paramList[0];
				///set radius displayed in a textbox
				textbox1.Text = Ins.paramList[1].ToString(); 
                this.radius = Ins.paramList[1];
				///set the angle of the steering wheel
				steer.Angle = radiusToAngle(this.radius);
				steer.Radius = this.radius;
            }
        }
		private void SliderVelocity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Ins.paramList[0] = (int)e.NewValue;
		}

		private void textbox1_TextChanged(object sender, TextChangedEventArgs e)
	{
			if (textbox1.Text.Length > 0 && textbox1.Text.Length <6 && textbox1.Text!= "-")
			{
				
			int new_radius = int.Parse(textbox1.Text);
				if(Math.Abs(new_radius) <= 2000 || new_radius == STRAIGHT) {
				Ins.paramList[1] = new_radius;
				this.radius = new_radius;/// update rotation radius
				this.Angle = radiusToAngle(new_radius); ///update the angle		
				steer.Angle = this.Angle;
				steer.Radius = new_radius;///
				}
			}
		}
		private bool isTextAllowed(string txt)
		{
			Regex regex = new Regex("[-0-9]"); //regex that matches allowed text
			if (!regex.IsMatch(txt))
			{
				return false;
			};
			if(txt == "-") {
				return true;
			}
			if (int.Parse(txt) > MAX_RADIUS)
			{
				return false;
			}
			return true;
		}
		private void textbox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !isTextAllowed(e.Text);
		}

		
        
    }
        
    
}
