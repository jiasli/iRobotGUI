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
using System.ComponentModel;

namespace iRobotGUI.Controls
{
	// Walkthrough: Building a Sweet Dial in XAML for Windows 8
	// http://blogs.msdn.com/b/jerrynixon/archive/2012/12/06/walkthrough-building-a-sweet-dial-in-xaml-for-windows-8.aspx
	// http://stackoverflow.com/questions/962974/rotate-graphic-towards-mouse-in-wpf-like-an-analog-dial

	/// <summary>
	/// Interaction logic for SteeringControl.xaml
	/// </summary>
	public partial class SteeringParam : UserControl, INotifyPropertyChanged
	{
		/// <summary>
		/// The radius of the steering wheel.
		/// </summary>
		public int Angle
		{
			get { return angle; }
			set { setProperty(ref angle, value); }
		}

		/// <summary>
		/// The radius represented by the steering wheel.
		/// </summary>
		public int Radius
		{
			get { return radius; }
			set { setProperty(ref radius, value); }
		}
	
		private const int MAX_RADIUS = 2000;
		private int radius = default(int);
		private int angle = 0;

		private enum Quadrants : int { nw = 2, ne = 1, sw = 4, se = 3 }


		public static int AngleToRadius(int angle)
		{
			// Special cases.

			// Drive straight if the angle is CLOSE to 0.
			if (Math.Abs(angle) < 5) return Instruction.STRAIGHT1;
			if (angle == 90) return -1;
			if (angle == -90) return 1;

			/*         ^ radius
			 *        /|
			 *       / |
			 *      /  |
			 *     /   |
			 *    /    |
			 * --------+------> angle
			 *   -90   |    /90
			 *         |   / 
			 *         |  /  
			 *         | /   
			 *         |/    
			 */
			double ratio = MAX_RADIUS / 90.0;
			if (angle > 0) return (int)(-MAX_RADIUS + angle * ratio);
			else return (int)(MAX_RADIUS + angle * ratio);
		}

		public static int RadiusToAngle(int radius)
		{
			// Special cases.
			if (radius == Instruction.STRAIGHT1 || radius == Instruction.STRAIGHT2)
			{
				return 0;
			}
			// If radius is 0, let it drive straight.
			if (radius == 0) return 0;

			/*         ^ angle
			 *        /|
			 *       / |
			 *      /  |
			 *     /   |
			 *    /    |
			 * --------+------> radius
			 * 	       |    /
			 * 		   |   / 
			 * 		   |  /  
			 * 		   | /   
			 * 		   |/    
			 */
			double ratio = 90.0 / MAX_RADIUS;
			if (radius > 0) return (int)(-90 + radius * ratio);
			else return (int)(90 + radius * ratio);
		}

		private int GetAngle(Point touchPoint, Size circleSize)
		{
			var _X = touchPoint.X - (circleSize.Width / 2d);
			var _Y = circleSize.Height - touchPoint.Y - (circleSize.Height / 2d);
			var _Hypot = Math.Sqrt(_X * _X + _Y * _Y);
			int _Value = (int)(Math.Asin(_Y / _Hypot) * 180 / Math.PI);
			var _Quadrant = (_X >= 0) ?
				(_Y >= 0) ? Quadrants.ne : Quadrants.se :
				(_Y >= 0) ? Quadrants.nw : Quadrants.sw;
			if (_Value >= 90)
			{
				_Value = 90;
			}
			switch (_Quadrant)
			{
				case Quadrants.ne: _Value = 90 - _Value; break;
				case Quadrants.nw: _Value = _Value - 90; break;

				// Set the angle to -90 and 90 in 3, 4 quadrants.
				case Quadrants.se: _Value = 90; break;
				case Quadrants.sw: _Value = -90; break;
			}
			return _Value;
		}

		public SteeringParam()
		{
			InitializeComponent();
			this.DataContext = this;
			this.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
			this.MouseUp += new MouseButtonEventHandler(OnMouseUp);
			this.MouseMove += new MouseEventHandler(OnMouseMove);
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
				// Get the current mouse position relative to the control
				Point currentLocation = Mouse.GetPosition(this);
				// Calculate the angle
				this.Angle = GetAngle(currentLocation, this.RenderSize);
				this.Radius = AngleToRadius(this.Angle);
			}
		}


		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		void setProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
		{
			if (!object.Equals(storage, value))
			{
				storage = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}

		}

	}
}
