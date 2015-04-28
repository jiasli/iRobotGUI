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
	/// Interaction logic for InstructionPanel.xaml
	/// </summary>
	public partial class InstructionPanel : UserControl
	{

		public delegate void AddNewInstructionEventHandler(string opcode);

		/// <summary>
		/// A instruction image is double clicked and should be added to the program list.
		/// </summary>
		public event AddNewInstructionEventHandler AddNewInstruction;

		public InstructionPanel()
		{
			InitializeComponent();
		}

		private void Image_MouseMove(object sender, MouseEventArgs e)
		{
			Image dragSource = sender as Image;
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				DragDrop.DoDragDrop(dragSource, dragSource.Tag, DragDropEffects.Copy);
			}

		}

		private void Image_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Image dragSource = sender as Image;
			if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
			{
				if (AddNewInstruction != null)
					AddNewInstruction(dragSource.Tag.ToString());
			}
		}
	}
}
