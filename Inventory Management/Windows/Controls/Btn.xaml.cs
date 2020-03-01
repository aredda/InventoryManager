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

namespace Inventory_Management.Windows.Controls
{
    /// <summary>
    /// Interaction logic for Btn.xaml
    /// </summary>
    public partial class Btn : UserControl
    {
        private Brush defaultColor;

        public Btn()
        {
            InitializeComponent();

            this.DataContext = this;
            this.IsEnabledChanged += Btn_IsEnabledChanged;
        }

        void Btn_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.Opacity = IsEnabled ? 1 : 0.5;
        }

        public Brush Backcolor
        {
            get 
            {
                return this.defaultColor;
            }
            set 
            {
                if (this.defaultColor == null)
                    this.defaultColor = value;
            }
        }
        public string BtnText {
            get { return txt_text.Text; }
            set { txt_text.Text = value; } 
        }
        public string ImgSource { get; set; }
    }
}
