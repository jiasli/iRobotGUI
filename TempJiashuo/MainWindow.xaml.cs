using iRobotGUI;
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

		static string ifTestString =
@"IF 0,0,0
	IF 0,0,0
		IF 0,0,0
		ELSE
		END_IF
	ELSE
	END_IF	
	IF 0,0,0
	ELSE
	END_IF
ELSE
	FORWARD 100,100
END_IF
";

		public MainWindow()
		{
			InitializeComponent();

			
			
		
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				openSongWindow();
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.ToString());
			}
			

			/*
			try
			{
				HLProgram pro = new HLProgram(File.ReadAllText("find_if_endif.igp"));
				int elseIndex = pro.FindElse(0);
				//MessageBox.Show(elseIndex.ToString());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			openIfWindow();
			*/
		}

		private void openLoopWindow()
		{
			LoopWindow lw = new LoopWindow();
			//lw.Owner = this;
			lw.SubProgram = new HLProgram(loopTestString);
			lw.ShowDialog();

			MessageBox.Show(lw.SubProgram.ToString());
		}

		private void openSongWindow()
		{
			SongWindow lw = new SongWindow();
			//lw.Owner = this;
			lw.Ins = new Instruction("SONG_DEF 3,60,32,62,32,64,32");
			lw.ShowDialog();
			
		}

		private void openIfWindow()
		{
			IfWindow lw = new IfWindow();
			//lw.Owner = this;
			lw.SubProgram = new HLProgram(ifTestString);
			lw.ShowDialog();

			MessageBox.Show(lw.SubProgram.ToString());
		}
	}
}
