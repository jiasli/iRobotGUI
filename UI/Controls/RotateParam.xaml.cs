using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace iRobotGUI.Controls
{

	/// <summary>
	/// Interaction logic for RightParam.xaml
	/// </summary>
	public partial class RotateParam : BaseParamControl
	{
		private double Angle;
		private enum Quadrants : int { nw = 2, ne = 1, sw = 4, se = 3 }
		//public string retAngle { get { return this.Angle.ToString("F");}}
		private double GetAngle(Point touchPoint, Size circleSize)
		{
			var _X = touchPoint.X - (circleSize.Width / 2d);
			var _Y = circleSize.Height - touchPoint.Y - (circleSize.Height / 2d);
			var _Hypot = Math.Sqrt(_X * _X + _Y * _Y);
			var _Value = Math.Asin(_Y / _Hypot) * 180 / Math.PI;
			var _Quadrant = (_X >= 0) ?
				(_Y >= 0) ? Quadrants.ne : Quadrants.se :
				(_Y >= 0) ? Quadrants.nw : Quadrants.sw;
			switch (_Quadrant)
			{
				// Allow roate greater than 90 degrees.
				case Quadrants.ne: _Value = 090 - _Value; break;
				case Quadrants.nw: _Value = _Value - 90; break;
				case Quadrants.se: _Value = 90 - _Value; break;
				case Quadrants.sw: _Value = -90 + _Value; break;
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

				this.Angle = -Ins.paramList[0];
				labelDegree.Content = Ins.paramList[0].ToString();
				///rotate the control image specified number of degrees:
				RotateTransform rotateTransform1 = new RotateTransform(this.Angle);
				rotateTransform1.CenterX = RotateGrid.ActualWidth / 2;
				rotateTransform1.CenterY = RotateGrid.ActualHeight / 2;
				RotateGrid.RenderTransform = rotateTransform1;

			}
		}
		public RotateParam()
		{
			InitializeComponent();
			/// DataContext = this;			
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
				/// Get the current mouse position relative to the control
				Point currentLocation = Mouse.GetPosition(this);
				txbDebug.Text = currentLocation.ToString();
				/// Calculate an angle
				this.Angle = GetAngle(currentLocation, RotateGrid.RenderSize);
				Ins.paramList[0] = -(int)this.Angle;
				labelDegree.Content = Ins.paramList[0].ToString();
				RotateTransform rotateTransform1 = new RotateTransform((int)this.Angle);
				rotateTransform1.CenterX = (RotateGrid.ActualWidth) / 2;
				rotateTransform1.CenterY = (RotateGrid.ActualHeight) / 2;
				RotateGrid.RenderTransform = rotateTransform1;
			}
		}



	}
}
