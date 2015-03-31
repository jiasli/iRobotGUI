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
         private double Angle;
		 private int radius;
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
			 double d = (double)radius / (double) MAX_RADIUS;
			 double rad_angle = Math.Asin(d);

			/* if (radius > 0)
			 {
				 return (rad_angle * 180.0) / Math.PI; /// return positive angle(in centigrade)
			 }
			 * */
			
			 return (rad_angle*180.0)/Math.PI; /// return negative angle
		 }
		 private int angleToRadius(double angle)
		 {
			 if (angle > -1.0 && angle < 1.0)
			 {
				 return STRAIGHT;
			 }
			 double rad_angle = (angle * Math.PI) / 180; ///convert angle from centigrade to radians
														 ///
			 return roundToInt((Math.Sin(rad_angle) * MAX_RADIUS)); ///(int)Math.Round((Math.Cos(rad_angle) * MAX_RADIUS), 0, MidpointRounding.AwayFromZero);
		 }
        private enum Quadrants : int { nw = 2, ne = 1, sw = 4, se = 3 }
        public DriveParam()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            this.MouseUp += new MouseButtonEventHandler(OnMouseUp);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
        }
        private double GetAngle(Point touchPoint, double Height, Point center)
        {
            var _X = touchPoint.X -center.X- (Height / 2d);
            var _Y = Height - touchPoint.Y +center.Y- (Height / 2d);
            var _Hypot = Math.Sqrt(_X * _X + _Y * _Y);
			
           /* var _Value = Math.Asin(_Y / _Hypot) * 180 / Math.PI;
            var _Quadrant = (_X >= 0) ?
                (_Y >= 0) ? Quadrants.ne : Quadrants.se :
                (_Y >= 0) ? Quadrants.nw : Quadrants.sw;
			*/
			var _Value = Math.Asin(_Y / _Hypot) * 180 / Math.PI;
			var _Quadrant = (touchPoint.X >= center.X) ?
				(touchPoint.Y <= center.Y) ? Quadrants.ne : Quadrants.se :
				(touchPoint.Y <= center.Y) ? Quadrants.nw : Quadrants.sw;
            switch (_Quadrant)
            {
               /// case Quadrants.ne: _Value = 090 - _Value; break;
				case Quadrants.nw: _Value = -_Value; break;
                case Quadrants.se: _Value = 090 /*- _Value*/; break;
                case Quadrants.sw: _Value =  -90 /* _Value*/; break;
            }
            return _Value;
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
				this.Angle = radiusToAngle(this.radius);
                ///rotate the control image by specified number of degrees:
                RotateTransform rotateTransform1 = new RotateTransform(roundToInt(this.Angle));
				ellipse.RenderTransformOrigin = new Point(0.5, 0.5); 
                /*rotateTransform1.CenterX = 75;
				rotateTransform1.CenterY = 75; */
                ellipse.RenderTransform = rotateTransform1;

            }
        }
    
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.Captured == this)
            {
                /// Get the current mouse position relative to the control
                Point currentLocation = Mouse.GetPosition(this);
				Point relativePoint = ellipse.TransformToAncestor(grid).Transform(new Point(0, 0));
				Point center =ellipse.PointToScreen(new Point(0d, 0d)); //new Point(relativePoint.X + ellipse.ActualWidth / 2, relativePoint.Y + ellipse.ActualHeight / 2);
                /// Calculate the angle
               this.Angle = GetAngle(currentLocation, (ellipse.Width), center);           
               Ins.paramList[1] =  angleToRadius(this.Angle); /// update rotation radius in instruction parameter list
               RotateTransform rotateTransform1 = new RotateTransform(roundToInt(this.Angle));
			   ellipse.RenderTransformOrigin = new Point(0.5, 0.5);
            /*   rotateTransform1.CenterX = 75;
               rotateTransform1.CenterY = 75;*/
               ellipse.RenderTransform = rotateTransform1; 
				///update textbox text:
			   textbox1.Text = Ins.paramList[1].ToString();
            }
        }
      /*  private void txtbox1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RotateTransform rotateTransform1 = new RotateTransform((int)this.Angle);
            rotateTransform1.CenterX = 75;
            rotateTransform1.CenterY = 75;
            RotateGrid.RenderTransform = rotateTransform1;
            Ins.paramList[1] = (int)e.NewValue;
        } */
		private void SliderVelocity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Ins.paramList[0] = (int)e.NewValue;
		}

		private void textbox1_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (textbox1.Text.Length != 0 && textbox1.Text!= "-")
			{
				
			int new_radius = int.Parse(textbox1.Text);
				if(Math.Abs(new_radius) <= 2000 || new_radius == STRAIGHT) {
				Ins.paramList[1] = new_radius; /// update rotation radius
				this.Angle = radiusToAngle(new_radius); ///update the angle
				RotateTransform rotateTransform1 = new RotateTransform(roundToInt(this.Angle)); /// now lets rotate wheel control
				ellipse.RenderTransformOrigin = new Point(0.5, 0.5);
			/*		rotateTransform1.CenterX = 75;
				rotateTransform1.CenterY = 75;*/
				ellipse.RenderTransform = rotateTransform1;
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
