using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Media.Animation;

namespace Java_Bytecode_Toolkit
{
    public class Spinner : Control
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            nameof(Angle),
            typeof(double),
            typeof(Spinner),
            new FrameworkPropertyMetadata(
                0.0
            )
        );

        public double Angle
        {
            get
            {
                return (double)this.GetValue(AngleProperty);
            }
            set
            {
                this.SetValue(AngleProperty, value);
            }
        }

        static Spinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(typeof(Spinner)));
        }

        public Spinner()
        {

        }

        public override void OnApplyTemplate()
        {

        }
    }
}
