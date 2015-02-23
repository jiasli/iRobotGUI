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
        public InstructionPanel()
        {
            InitializeComponent();
        }

        private void MouseMove_General(object sender, MouseEventArgs e)
        {
            Image dragSource = sender as Image;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(sender as Image, dragSource.Tag, DragDropEffects.Copy);
            }
        }
    }
}
