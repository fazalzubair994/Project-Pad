using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace projectPad.Controls;


    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Border
    {
        public event EventHandler<Brush> ColorSelected;

        public ColorPicker()
        {
            InitializeComponent();
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                var selectedColor = toggleButton.Background as SolidColorBrush;
                if (selectedColor != null && toggleButton.IsChecked == true)
                {
                    ColorSelected?.Invoke(this, selectedColor);
                }
            }
        }
        ToggleButton tb = new();
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            tb.IsChecked = false;
            tb = (ToggleButton)sender;
            tb.Content = "\ue73e";
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

            tb.Content = " ";
        }
    }

