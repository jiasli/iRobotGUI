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
	
		public LedParam()
		{
			InitializeComponent();
		}


		private void CheckBoxPlay_Checked(object sender, RoutedEventArgs e)
		{
			// Colors: https://msdn.microsoft.com/en-us/library/system.windows.media.colors(v=vs.110).aspx
			EllipsePlay.Fill = new SolidColorBrush(onColor);
		}

		private void CheckBoxPlay_Unchecked(object sender, RoutedEventArgs e)
		{
			EllipsePlay.Fill = new SolidColorBrush(offColor);            
		}

		private void CheckBoxAdvance_Checked(object sender, RoutedEventArgs e)
		{
			EllipseAdvance.Fill = new SolidColorBrush(onColor);
		}

		private void CheckBoxAdvance_Unchecked(object sender, RoutedEventArgs e)
		{
			EllipseAdvance.Fill = new SolidColorBrush(offColor);
		}

		// Separate event handler for Sliders, or error will happen at initialization - Original value will be modified with uninitialized Slider value
		private void SliderColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			// To make sure that both controls have been initialized.
			if (SliderColor != null && SliderIntensity!= null)
			{
				EllipsePower.Fill = new SolidColorBrush(GetLedRgbColor(SliderColor.Value, SliderIntensity.Value)); 
			}
		}

		private void SliderIntensity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (SliderColor != null && SliderIntensity != null)
			{
				EllipsePower.Fill = new SolidColorBrush(GetLedRgbColor(SliderColor.Value, SliderIntensity.Value));
			}
		}

		/// <summary>
		/// Calculate the color of Power LED
		/// </summary>
		/// <param name="color"></param>
		/// <param name="intensity"></param>
		/// <returns></returns>
		private Color GetLedRgbColor(double color, double intensity)
		{
			// 0, 255, 0 Green
			// 255, 255, 0 Yellow
			// 255, 0, 0 Red
			Debug.WriteLine(color + " " + intensity);
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


	}
}
