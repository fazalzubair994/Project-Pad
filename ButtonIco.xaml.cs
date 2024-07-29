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

namespace projectPad;

/// <summary>
/// Interaction logic for ButtonIco.xaml
/// </summary>
public partial class ButtonIco : Button
{
    public ButtonIco()
    {
        InitializeComponent();
        this.DataContext = this;
    }



    public string Icon
    {
        get { return (string)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(string), typeof(ButtonIco), new PropertyMetadata("&#xe74d;"));



    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(ButtonIco), new PropertyMetadata("Your Text"));




    public double TextFontSize
    {
        get { return (double)GetValue(TextFontSizeProperty); }
        set { SetValue(TextFontSizeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TextFontSize.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextFontSizeProperty =
        DependencyProperty.Register("TextFontSize", typeof(double), typeof(ButtonIco), new PropertyMetadata(Convert.ToDouble(18)));

    
}
