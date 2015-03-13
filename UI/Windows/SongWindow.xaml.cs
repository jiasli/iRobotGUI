using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace iRobotGUI
{
	/// <summary>
	/// Interaction logic for Song.xaml
	/// API of Song is in iRobot Open API p.11.
	/// </summary>
	public partial class SongWindow : BaseParamWindow
	{
		public string songInsStr;
		private ObservableCollection<Note> noteList;
		private OutputDevice outDevice;

		public SongWindow()
		{
			InitializeComponent();			
		}

		public override iRobotGUI.Instruction Ins
		{
			get
			{
				return base.Ins;
			}
			set
			{

				base.Ins = value;

				comboBoxSongNo.SelectedIndex = Ins.paramList[0];
				noteList = new ObservableCollection<Note>();
				int i = 1;
				while (i<Ins.paramList.Count)
				{
					noteList.Add(new Note(Ins.paramList[i], Ins.paramList[i + 1]));
					i += 2;
				}
				listBoxNotes.ItemsSource = noteList;
				
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			if (outDevice != null)
			{
				outDevice.Dispose();
			}
			base.OnClosed(e);
		}

		private void buttonMoveUp_Click(object sender, RoutedEventArgs e)
		{			
			noteList.Move(listBoxNotes.SelectedIndex, listBoxNotes.SelectedIndex - 1);
		}

		private void buttonMoveDown_Click(object sender, RoutedEventArgs e)
		{
			noteList.Move(listBoxNotes.SelectedIndex, listBoxNotes.SelectedIndex + 1);
		}

		private void ButtonDelete_Click(object sender, RoutedEventArgs e)
		{
			noteList.RemoveAt(listBoxNotes.SelectedIndex);
		}

		private void buttonNew_Click(object sender, RoutedEventArgs e)
		{
			noteList.Clear();			
		}

		private void ButtonOk_Click(object sender, RoutedEventArgs e)
		{
			songInsStr = GetInsString();
			DialogResult = true;
		}

		/// <summary>
		/// Get the string of SONG_DEF
		/// </summary>
		private string GetInsString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Instruction.SONG_DEF + " " + comboBoxSongNo.SelectedItem.ToString());
			foreach (Note note in listBoxNotes.Items)
			{
				sb.Append("," + note.ToString());
			}
			return sb.ToString();
		}

		private void pianoKeyboard_PianoKeyDown(object sender, PianoKeyEventArgs e)
		{
			Note note = new Note(e.NoteID);
			// Insert a note to current position
			if (listBoxNotes.SelectedIndex >= 0)
				listBoxNotes.Items.Insert(listBoxNotes.SelectedIndex, note);
			else listBoxNotes.Items.Add(note);
			outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, e.NoteID, 127));

		}

		private void pianoKeyboard_PianoKeyUp(object sender, PianoKeyEventArgs e)
		{

			outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, e.NoteID, 0));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i <= 15; i++)
			{
				ComboBoxItem item = new ComboBoxItem();
				item.Content = i.ToString();
				comboBoxSongNo.Items.Add(item);
			}

			if (!string.IsNullOrEmpty(songInsStr))
				if (Validator.ValidateInstruction(songInsStr))
					Ins = new Instruction(songInsStr);

			outDevice = new OutputDevice(0);
		}

		

		private void sliderDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (listBoxNotes.SelectedItem != null)
			{
				(listBoxNotes.SelectedItem as Note).Duration = (int)sliderDuration.Value;
			}
		}

	

		
	}


	/// <summary>
	/// A class representing the musical note.
	/// </summary>
	public class Note : INotifyPropertyChanged
	{
		private int number;
		private int duration;

		// This method is called by the Set accessor of each property. 
		// The CallerMemberName attribute that is applied to the optional propertyName 
		// parameter causes the property name of the caller to be substituted as an argument. 
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Note number.
		/// </summary>
		public int Number
		{
			get
			{
				return number;
			}
			set
			{
				number = value;
				NotifyPropertyChanged();
			}
		}

		/// <summary>
		/// Note duration.
		/// </summary>
		public int Duration
		{
			get
			{
				return duration;
			}
			set
			{
				duration = value;
				NotifyPropertyChanged();
			}
		}



		public Note(int number, int duration)
		{
			this.Number = number;
			this.Duration = duration;
		}

		public Note(int nubmer)
			: this(nubmer, 32)
		{

		}

		public override string ToString()
		{
			return Number.ToString() + "," + Duration.ToString();
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
