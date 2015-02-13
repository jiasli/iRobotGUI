using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.UI;
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
using System.Windows.Shapes;

namespace iRobotPrototypeWpf
{
    /// <summary>
    /// Interaction logic for Song.xaml
    /// </summary>
    public partial class SongWindow : Window
    {
        private OutputDevice outDevice;
        public string resultIns;

        private StringBuilder noteSerial = new StringBuilder();

        public SongWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the interop host control.
            System.Windows.Forms.Integration.WindowsFormsHost host =
                new System.Windows.Forms.Integration.WindowsFormsHost();

            // Create the MaskedTextBox control.
            PianoControl mtbDate = new PianoControl();

            // Assign the MaskedTextBox control as the host control's child.
            host.Child = mtbDate;

            outDevice = new OutputDevice(0);

            // Add the interop host control to the Grid 
            // control's collection of child controls. 
           // this.grid1.Children.Add(host);
           
        }

        private void pianoKeyboard_PianoKeyDown(object sender, PianoKeyEventArgs e)
        {
           
            if (noteSerial.Length != 0)
                noteSerial.Append(",");

            noteSerial.Append(e.NoteID.ToString() + ",32");
            textBoxSong.Text = noteSerial.ToString();
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
            resultIns = "SONG_DEF 1," + noteSerial.ToString();
            DialogResult = true;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxSong.Text = "";
            
            noteSerial.Clear();
        }
    }
}
