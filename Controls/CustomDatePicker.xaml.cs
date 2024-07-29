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

namespace Project_Pad.Controls
{
    /// <summary>
    /// Interaction logic for CustomDatePicker.xaml
    /// </summary>
    public partial class CustomDatePicker : Border
    {
        public CustomDatePicker()
        {
            InitializeComponent();

            DataContext = this;
        }

        public event EventHandler<DateTime> DateSelected;

       

        public string TodayDayOfWeek => DateTime.Today.DayOfWeek.ToString();
        public string TomorrowDayOfWeek => DateTime.Today.AddDays(1).DayOfWeek.ToString();

        private void Today_Click(object sender, RoutedEventArgs e)
        {
            DateSelected?.Invoke(this, DateTime.Today);
        }

        private void Tomorrow_Click(object sender, RoutedEventArgs e)
        {
            DateSelected?.Invoke(this, DateTime.Today.AddDays(1));
        }

        private void PickDate_Click(object sender, RoutedEventArgs e)
        {
            PickADateControl.Visibility = Visibility.Visible;
            ShowDateBtn.Visibility = Visibility.Collapsed;
        }

        private void PickADateControl_DateSelected(object sender, DateTime e)
        {
            PickADateControl.Visibility = Visibility.Collapsed;
            ShowDateBtn.Visibility = Visibility.Visible;
            CustomDate.Text = e.ToLongDateString();
            DateSelected?.Invoke(this, e);
        }
    }
}
