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

namespace iRobotGUI
{
    /// <summary>
    /// Interaction logic for RightWindow.xaml
    /// </summary>
    public partial class RotateWindow : BaseParamWindow
    {
        public RotateWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RotateControlInstance.Ins = Ins;
        }
    }
}
