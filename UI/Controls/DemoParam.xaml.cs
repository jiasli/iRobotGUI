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
			if (Ins == null)
			{
				return;
			}

			if (comboBox.SelectedIndex != -1)
			{
				int num = comboBox.SelectedIndex;
				if (num == 10)
				{
					num = -1;
				};
				base.Ins.paramList[0] = num;
				switch (num)
				{
					case 0:
						desc.Text = "Create attempts to cover an entire\n" +
						"room using a combination of behaviors,\n" +
						"such as random bounce, wall following,\n" +
						"and spiraling";
						break;
					case 1:
						desc.Text = "Identical to the Cover demo,\n" +
						"with one exception. If Create sees\n" +
						"an infrared signal from an iRobot\n" +
						"Home Base, it uses that signal\n" +
						"to dock with the Home Base and\n" +
						"recharge itself";
						break;
					case 2:
						desc.Text = "Create covers an area around\n" +
							"its starting position by spiraling\n" +
							"outward, then inward";
						break;
					case 3:
						desc.Text = "Create drives in search of a wall.\n" +
							"Once a wall is found, Create drives\n" +
							"along the wall, traveling around\n" +
							"circumference of the room";
						break;
					case 4:
						desc.Text = "Create continuously drives in a\n" + "figure 8 pattern";
						break;
					case 5:
						desc.Text = "Create drives forward " +
							"when pushed\n" + "from behind. " +
							"If Create hits an obstacle\nwhile " +
							"driving," + "it drives away\nfrom the obstacle";
						break;
					case 6:
						desc.Text = "Create drives toward an iRobot Virtual\n" +
							"Wall as long as the back and sides of\n" +
							"the virtual wall receiver are blinded by\n" +
							"black electrical tape.\n" +
							"A Virtual Wall emits infrared signals\n" +
							"that Create sees with its Omnidirectional\n" +
							"Infrared Receiver, located on top of the\n" +
							"bumper.If you want Create to home in on a\n" +
							"Virtual Wall, cover all but a small\n" +
							"opening in the front of the infrared\n" +
							"receiver with black electrical tape.\n" +
							"Create spins to locate a virtual wall,\n" +
							"then drives toward it. Once Create hits\n" +
							"the wall or another obstacle, it stops.";
						break;
					case 7:
						desc.Text = "Identical to the Home demo, except\n" +
						"Create drives into multiple virtual walls\n" +
						"by bumping into one, turning around,\n" +
						"driving to the next virtual wall, bumping\n" +
						"into it and turning around to bump into\n" +
						"the next virtual wall.";
						break;
					case 8:
						desc.Text = "Create plays the notes of\n" +
							"Pachelbel’s Canon in sequence\n" + "when cliff sensors are activated";
						break;
					case 9:
						desc.Text = "Create plays a note of a chord for each\n" +
						"of its four cliff sensors. Select the\n" +
						"chord using the bumper, as follows:\n" +
						"- No bumper: G major.\n" +
						"- Right/left bumper: D major 7\n" +
						"- Both bumpers (center): C major\n";
						break;
					case -1:
						desc.Text = "Stops the demo that Create is\n" + "currently performing";
						break;
				}

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
			desc.Text = "Create attempts to cover an entire\n" +
					"room using a combination of behaviors,\n" +
					"such as random bounce, wall following,\n" +
					"and spiraling";
		}




	}
}
