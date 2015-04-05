using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	/// Interaction logic for LEDControl.xaml
	/// </summary>
	public partial class LedParam : BaseParamControl
	{
		Color onColor = Colors.Lime;
		Color offColor = Colors.Black;

		private const int PLAY_BIT = 1;
		private const int ADVANCE_BIT = 3;

		public LedParam()
		{
			InitializeComponent();
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

				SliderColor.Value = Ins.paramList[1];
				SliderIntensity.Value = Ins.paramList[2];

				// Set checkbox status according to 4th and 2nd bits      
				CheckBoxPlay.IsChecked = GetBit(Ins.paramList[0], PLAY_BIT);
				CheckBoxAdvance.IsChecked = GetBit(Ins.paramList[0], ADVANCE_BIT);
			}
		}

		private void CheckBoxPlay_Checked(object sender, RoutedEventArgs e)
		{
			// Colors: https://msdn.microsoft.com/en-us/library/system.windows.media.colors(v=vs.110).aspx
			EllipsePlay.Fill = new SolidColorBrush(onColor);

			Ins.paramList[0] = SetBit(Ins.paramList[0], PLAY_BIT, true);
		}

		private void CheckBoxPlay_Unchecked(object sender, RoutedEventArgs e)
		{
			EllipsePlay.Fill = new SolidColorBrush(offColor);
			Ins.paramList[0] = SetBit(Ins.paramList[0], PLAY_BIT, false);
		}

		private void CheckBoxAdvance_Checked(object sender, RoutedEventArgs e)
		{
			EllipseAdvance.Fill = new SolidColorBrush(onColor);
			Ins.paramList[0] = SetBit(Ins.paramList[0], ADVANCE_BIT, true);
		}

		private void CheckBoxAdvance_Unchecked(object sender, RoutedEventArgs e)
		{
			EllipseAdvance.Fill = new SolidColorBrush(offColor);
			Ins.paramList[0] = SetBit(Ins.paramList[0], ADVANCE_BIT, false);
		}

		// Separate event handler for Sliders, or error will happen at initialization - Original value will be modified with uninitialized Slider value
		private void SliderColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			EllipsePower.Fill = new SolidColorBrush(GetLedRgbColor(SliderColor.Value, SliderIntensity.Value));
			Ins.paramList[1] = (int)e.NewValue;
		}

		private void SliderIntensity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			EllipsePower.Fill = new SolidColorBrush(GetLedRgbColor(SliderColor.Value, SliderIntensity.Value));
			Ins.paramList[2] = (int)e.NewValue;
		}

		#region Util Methods

		/// <summary>
		/// Get Color from the given color and intensity. The Color controls the color of power LED.
		/// </summary>
		/// <param name="color">A value from 0 to 255, green to red.</param>
		/// <param name="intensity">A value from 0 to 255, completely off to completely on.</param>
		/// <returns></returns>
		private Color GetLedRgbColor(double color, double intensity)
		{
			// 0, 255, 0 Green
			// 255, 255, 0 Yellow
			// 255, 0, 0 Red
			double intensityRatio = intensity / 255.0;

			Color rgbColor;
			if (color < 128)
				rgbColor = Color.FromRgb(
					(byte)(color * 2 * intensityRatio),
					(byte)(255 * intensityRatio),
					0);
			else rgbColor = Color.FromRgb(
				(byte)(255 * intensityRatio),
				(byte)((255 - (color - 128) * 2) * intensityRatio),
				0);

			return rgbColor;
		}


		/// <summary>
		/// Set the specific bit of an integer, starting from the small end.
		/// </summary>
		/// <param name="n">The integer being set. </param>
		/// <param name="index">The index from the small end. </param>
		/// <param name="value">0 or 1 </param>
		/// <returns>The integer after modification. </returns>
		private int SetBit(int n, int index, bool value)
		{
			// clear index-th bit.
			n = n & ~(1 << index);

			// set index-th bit.
			n = n | ((value ? 1 : 0) << index);
			return n;
		}

		/// <summary>
		/// Get the specific bit of an integer, starting from the small end.
		/// </summary>
		/// <param name="n">The integer being set. </param>
		/// <param name="index">The index from the small end. </param>
		/// <returns></returns>
		private bool GetBit(int n, int index)
		{
			return (n & (1 << index)) > 0;
		}

		#endregion


	}
}
