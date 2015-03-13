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

namespace iRobotGUI.Controls
{
    /// <summary>
    /// Interaction logic for RightParam.xaml
    /// </summary>
    public partial class RightParam : BaseParamControl
    {
        public override Instruction Ins
        {
            get
            {
                return base.Ins;
            }
            set
            {
                base.Ins = value;

                SliderAngle.Value = Ins.paramList[0];
            }
        }
        public RightParam()
        {
            InitializeComponent();
        }

        private void SliderAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Ins.paramList[0] = (int)e.NewValue;
        }
    }
}
