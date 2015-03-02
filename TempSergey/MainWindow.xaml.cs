using iRobotGUI;
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

namespace TempSergey
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowDemoDialog(Instruction ins)
        {
            DemoWindow dlg = new DemoWindow();
            dlg.Owner = Window.GetWindow(this);
            dlg.Ins = ins;
            dlg.ShowDialog();
        }

       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newIns = new Instruction("DEMO" + " 0");
            ShowDemoDialog(newIns);
        }
    }
}
