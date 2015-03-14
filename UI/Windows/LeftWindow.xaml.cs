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
    /// Interaction logic for LeftWindow.xaml
    /// </summary>
    public partial class LeftWindow : BaseParamWindow
    {
        public LeftWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           LeftControlInstance.Ins = Ins;
        }
    }
}
