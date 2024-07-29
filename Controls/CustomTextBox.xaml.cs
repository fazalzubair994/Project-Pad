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
using System.Xml;

namespace projectPad.Controls;

/// <summary>
/// Interaction logic for CustomTextBox.xaml
/// </summary>
public partial class CustomTextBox : TextBox
{
    public CustomTextBox()
    {
        InitializeComponent();
        this.DataContext = this;
    }



    public string Placeholder
    {
        get { return (string)GetValue(PlaceholderProperty); }
        set { SetValue(PlaceholderProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Placeholder.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.Register("Placeholder", typeof(string), typeof(CustomTextBox), new PropertyMetadata("Placeholder here"));

    bool TextAdded = false;

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (!TextAdded)
        {
            this.Text = Placeholder;
            this.Foreground = Brushes.DarkGray;
        }
     
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if(!TextAdded)
        {
            this.Text = "";
        }

    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        this.SetResourceReference(ForegroundProperty, "TextColor");
        if (this.Text != Placeholder && this.Text != string.Empty)
        {
            TextAdded = true;
            // Add the dynamic resource binding
            this.SetResourceReference(ForegroundProperty, "TextColor");

        }
        else
        {
            TextAdded = false;
            this.Foreground = Brushes.DarkGray;
        }
           

    }
}
