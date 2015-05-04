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
		public override Instruction Ins
		{
			get
			{
				return base.Ins;
			}
			set
			{
				base.Ins = value;
				if (value.paramList[0] == -1)
					ComboBox1.SelectedIndex = 10;
				else
					ComboBox1.SelectedIndex = value.paramList[0];
			}
		}

		public DemoParam()
		{
			InitializeComponent();

			// This demo number should be the same as the initial combobox index.
			UpdateDemoDescription(0);
		}

		private void UpdateDemoDescription(int demoNumber)
		{
			switch (demoNumber)
			{
				case 0:
					desc.Text = "Create attempts to cover an entire " +
					"room using a combination of behaviors, " +
					"such as random bounce, wall following, " +
					"and spiraling";
					break;
				case 1:
					desc.Text = "Identical to the Cover demo, " +
					"with one exception. If Create sees " +
					"an infrared signal from an iRobot " +
					"Home Base, it uses that signal " +
					"to dock with the Home Base and " +
					"recharge itself";
					break;
				case 2:
					desc.Text = "Create covers an area around " +
						"its starting position by spiraling " +
						"outward, then inward";
					break;
				case 3:
					desc.Text = "Create drives in search of a wall. " +
						"Once a wall is found, Create drives " +
						"along the wall, traveling around " +
						"circumference of the room";
					break;
				case 4:
					desc.Text = "Create continuously drives in a " + "figure 8 pattern";
					break;
				case 5:
					desc.Text = "Create drives forward " +
						"when pushed " + "from behind. " +
						"If Create hits an obstacle while " +
						"driving," + "it drives away from the obstacle";
					break;
				case 6:
					desc.Text = "Create drives toward an iRobot Virtual " +
						"Wall as long as the back and sides of " +
						"the virtual wall receiver are blinded by " +
						"black electrical tape. " +
						"A Virtual Wall emits infrared signals " +
						"that Create sees with its Omnidirectional " +
						"Infrared Receiver, located on top of the " +
						"bumper.If you want Create to home in on a " +
						"Virtual Wall, cover all but a small " +
						"opening in the front of the infrared " +
						"receiver with black electrical tape. " +
						"Create spins to locate a virtual wall, " +
						"then drives toward it. Once Create hits " +
						"the wall or another obstacle, it stops.";
					break;
				case 7:
					desc.Text = "Identical to the Home demo, except " +
					"Create drives into multiple virtual walls " +
					"by bumping into one, turning around, " +
					"driving to the next virtual wall, bumping " +
					"into it and turning around to bump into " +
					"the next virtual wall.";
					break;
				case 8:
					desc.Text = "Create plays the notes of " +
						"Pachelbel’s Canon in sequence " + "when cliff sensors are activated";
					break;
				case 9:
					desc.Text = "Create plays a note of a chord for each " +
					"of its four cliff sensors. Select the " +
					"chord using the bumper, as follows:@" +
					"- No bumper: G major.@" +
					"- Right/left bumper: D major 7@" +
					"- Both bumpers (center): C major";
					desc.Text = desc.Text.Replace("@", System.Environment.NewLine);
					break;
				case -1:
					desc.Text = "Stops the demo that Create is currently performing";
					break;
			}
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
			if (Ins == null)
			{
				return;
			}

			if (comboBox.SelectedIndex != -1)
			{
				int demoNumber = comboBox.SelectedIndex;

				if (demoNumber == 10)
				{
					demoNumber = -1;
				};

				base.Ins.paramList[0] = demoNumber;
				UpdateDemoDescription(demoNumber);

			}
		}



	}
}
