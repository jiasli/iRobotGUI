using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PianoKeyboardDemo
{
    public partial class Form1 : Form
    {
        private OutputDevice outDevice;
        private int outDeviceID = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void pianoControl1_PianoKeyDown(object sender, Sanford.Multimedia.Midi.UI.PianoKeyEventArgs e)
        {
            textBox1.Text = e.NoteID.ToString();
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, e.NoteID, 127));
        }

        private void pianoControl1_PianoKeyUp(object sender, Sanford.Multimedia.Midi.UI.PianoKeyEventArgs e)
        {
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, e.NoteID, 0));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            outDevice = new OutputDevice(outDeviceID);
        }

        protected override void OnClosed(EventArgs e)
        {           
            if (outDevice != null)
            {
                outDevice.Dispose();
            }          

            base.OnClosed(e);
        }
    }
}
