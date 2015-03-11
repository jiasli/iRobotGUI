using iRobotGUI;
using iRobotGUI.Windows;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace TempJiashuo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		static string loopTestString =
@"LOOP 0, 1, 1
	// Read the sensor again
	READ_SENSOR
	DELAY 100
END_LOOP";

		public MainWindow()
		{
			InitializeComponent();

			
		
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				HLProgram pro = new HLProgram(File.ReadAllText("find_if_endif.igp"));
				int elseIndex = pro.FindElse(0);
				MessageBox.Show(elseIndex.ToString());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			openIfWindow();
		}

		private void openLoopWindow()
		{
			LoopWindow lw = new LoopWindow();
			//lw.Owner = this;
			lw.SubProgram = new HLProgram(loopTestString);
			lw.ShowDialog();

			MessageBox.Show(lw.SubProgram.ToString());
		}

		private void openIfWindow()
		{
			IfWindow lw = new IfWindow();
			//lw.Owner = this;
			//lw.SubProgram = new HLProgram(loopTestString);
			lw.ShowDialog();

			MessageBox.Show(lw.SubProgram.ToString());
		}
	}
}
