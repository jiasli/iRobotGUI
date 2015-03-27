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
    /// Interaction logic for ForwardParam.xaml
    /// </summary>
    public partial class MoveParam : BaseParamControl
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
                    
                SliderDistance.Value = Ins.paramList[0];
             
                SliderDuration.Value = Ins.paramList[1];
            }
        }
        public MoveParam()
        {
            InitializeComponent();
        }

        private void SliderDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Ins.paramList[1] = (int)e.NewValue;
        }

        private void SliderDistance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int slide_val = (int)e.NewValue;
           
            Ins.paramList[0] = slide_val;
        }

    }
}
