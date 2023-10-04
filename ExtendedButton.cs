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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Java_Bytecode_Toolkit
{
    public class ExtendedButton : Button
    {
        private Border border = null;

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(ExtendedButton)
        );

        public CornerRadius CornerRadius
        {
            get
            {
                if (this.border == null)
                {
                    return new CornerRadius();
                }

                return this.border.CornerRadius;
            }
            set
            {
                if (this.border == null)
                {
                    return;
                }

                this.border.CornerRadius = value;
            }
        }

        static ExtendedButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedButton), new FrameworkPropertyMetadata(typeof(ExtendedButton)));
        }

        public ExtendedButton()
        {

        }

        public override void OnApplyTemplate()
        {
            this.border = this.Template.FindName(
                "Border",
                this
            ) as Border;
        }
    }
}
