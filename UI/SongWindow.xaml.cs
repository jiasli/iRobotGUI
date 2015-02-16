using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.UI;
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
using System.Windows.Shapes;

namespace iRobotGUI
{
    /// <summary>
    /// Interaction logic for Song.xaml
    /// </summary>
    public partial class SongWindow : Window
    {
        private OutputDevice outDevice;
        public string songInsStr;
        public Instruction Ins;

        /// <summary>
        /// A class to represent the musical note.
        /// </summary>
        private class Note
        {
            int Number;
            int Duration;

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
        }

        public SongWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(songInsStr))
            if (Validator.Validate(songInsStr))
                Ins = new Instruction(songInsStr);

            for (int i = 0; i < 15; i++)
            {
                songNoList.Items.Add(i.ToString());
            }
            songNoList.SelectedIndex = 0;

            outDevice = new OutputDevice(0);
        }

        private void pianoKeyboard_PianoKeyDown(object sender, PianoKeyEventArgs e)
        {
            Note note = new Note(e.NoteID);
            // Insert a note to current position
            if (noteList.SelectedIndex >= 0)
                noteList.Items.Insert(noteList.SelectedIndex, note);
            else noteList.Items.Add(note);
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, e.NoteID, 127));

        }

        private void pianoKeyboard_PianoKeyUp(object sender, PianoKeyEventArgs e)
        {

            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, e.NoteID, 0));
        }

        protected override void OnClosed(EventArgs e)
        {
            if (outDevice != null)
            {
                outDevice.Dispose();
            }
            base.OnClosed(e);
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            songInsStr = GetInsString();
            DialogResult = true;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            noteList.Items.Clear();

            // noteSerial.Clear();
        }

        /// <summary>
        /// Get the string of SONG_DEF
        /// </summary>
        private string GetInsString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Instruction.SONG_DEF + " " + songNoList.SelectedItem.ToString());
            foreach (Note note in noteList.Items)
            {
                sb.Append("," + note.ToString());
            }
            return sb.ToString();
        }
    }
}
