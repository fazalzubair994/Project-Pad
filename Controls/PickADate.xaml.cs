using System;
using System.Windows;
using System.Windows.Controls;

namespace Project_Pad.Controls
{
    public partial class PickADate : Border
    {
        public PickADate()
        {
            InitializeComponent();
            DataContext = this;
            SelectedDate = DateTime.Today;
        }

        public event EventHandler<DateTime> DateSelected;

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(PickADate), new PropertyMetadata(DateTime.Today));

        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public int SelectedDay
        {
            get { return SelectedDate.Day; }
            set { SelectedDate = new DateTime(SelectedDate.Year, SelectedDate.Month, value); }
        }

        public int SelectedMonth
        {
            get { return SelectedDate.Month; }
            set { SelectedDate = new DateTime(SelectedDate.Year, value, SelectedDate.Day); }
        }

        public int SelectedYear
        {
            get { return SelectedDate.Year; }
            set { SelectedDate = new DateTime(value, SelectedDate.Month, SelectedDate.Day); }
        }

        private void IncreaseDay_Click(object sender, RoutedEventArgs e)
        {
            SelectedDay = SelectedDay < DateTime.DaysInMonth(SelectedYear, SelectedMonth) ? SelectedDay + 1 : 1;
            DateSelector.Text = SelectedDay.ToString();
        }

        private void DecreaseDay_Click(object sender, RoutedEventArgs e)
        {
            SelectedDay = SelectedDay > 1 ? SelectedDay - 1 : DateTime.DaysInMonth(SelectedYear, SelectedMonth);
            DateSelector.Text = SelectedDay.ToString();
        }

        private void IncreaseMonth_Click(object sender, RoutedEventArgs e)
        {
            SelectedMonth = SelectedMonth < 12 ? SelectedMonth + 1 : 1;
            MonthSelector.Text = SelectedMonth.ToString();
        }

        private void DecreaseMonth_Click(object sender, RoutedEventArgs e)
        {
            SelectedMonth = SelectedMonth > 1 ? SelectedMonth - 1 : 12;
            MonthSelector.Text = SelectedMonth.ToString();
        }

        private void IncreaseYear_Click(object sender, RoutedEventArgs e)
        {
            SelectedYear += 1;
            YearSelector.Text = SelectedYear.ToString();
        }

        private void DecreaseYear_Click(object sender, RoutedEventArgs e)
        {
            SelectedYear -= 1;
            YearSelector.Text = SelectedYear.ToString();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DateSelected?.Invoke(this, SelectedDate);
            
        }
    }
}
