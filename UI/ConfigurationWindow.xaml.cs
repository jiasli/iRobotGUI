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
    /// Interaction logic for ComWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        private Configuation _config;
        public Configuation Config
        {
            set
            {
                _config = value;
                textboxCom.Text = value.comPort;

                switch (value.firmwareVersion)
                {
                    case Configuation.STK500:
                        radio0.IsChecked = true;
                        break;
                    case Configuation.STK500V1:
                        radio1.IsChecked = true;
                        break;
                    case Configuation.STK500V2:
                        radio2.IsChecked = true;
                        break;
                }
            }
            get
            {
                return _config;
            }
        }

        public ConfigurationWindow()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            _config.comPort = textboxCom.Text;

            if (radio0.IsChecked ?? false)
                _config.firmwareVersion = radio0.Content.ToString();
            else if (radio1.IsChecked ?? false)
                _config.firmwareVersion = radio1.Content.ToString();
            else if (radio2.IsChecked ?? false)
                _config.firmwareVersion = radio2.Content.ToString();

            WinAvrConnector.CustomizeMakefile();
            DialogResult = true;
        }

        private void ButtonDeviceManager_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("devmgmt.msc");
        }
    }
}
