using System;
using System.Windows;

namespace ScienceBowlTimer
{
    public partial class TimeAdjustDialog : Window
    {
        private TimeSpan _currentTime;

        public TimeSpan Result { get; private set; }

        public TimeAdjustDialog(TimeSpan initialTime)
        {
            InitializeComponent();
            _currentTime = initialTime;
            Result = initialTime;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            int minutes = (int)_currentTime.TotalMinutes;
            int seconds = _currentTime.Seconds;
            TimeDisplay.Text = $"{minutes}:{seconds:D2}";
        }

        private void MinutePlus1_Click(object sender, RoutedEventArgs e)
        {
            _currentTime = _currentTime.Add(TimeSpan.FromMinutes(1));
            UpdateDisplay();
        }

        private void MinutePlus10_Click(object sender, RoutedEventArgs e)
        {
            _currentTime = _currentTime.Add(TimeSpan.FromMinutes(10));
            UpdateDisplay();
        }

        private void MinuteMinus1_Click(object sender, RoutedEventArgs e)
        {
            var newTime = _currentTime.Subtract(TimeSpan.FromMinutes(1));
            if (newTime.TotalSeconds < 1)
            {
                _currentTime = TimeSpan.FromSeconds(1);
            }
            else
            {
                _currentTime = newTime;
            }
            UpdateDisplay();
        }

        private void MinuteMinus10_Click(object sender, RoutedEventArgs e)
        {
            var newTime = _currentTime.Subtract(TimeSpan.FromMinutes(10));
            if (newTime.TotalSeconds < 1)
            {
                _currentTime = TimeSpan.FromSeconds(1);
            }
            else
            {
                _currentTime = newTime;
            }
            UpdateDisplay();
        }

        private void SecondPlus1_Click(object sender, RoutedEventArgs e)
        {
            _currentTime = _currentTime.Add(TimeSpan.FromSeconds(1));
            UpdateDisplay();
        }

        private void SecondPlus10_Click(object sender, RoutedEventArgs e)
        {
            _currentTime = _currentTime.Add(TimeSpan.FromSeconds(10));
            UpdateDisplay();
        }

        private void SecondMinus1_Click(object sender, RoutedEventArgs e)
        {
            var newTime = _currentTime.Subtract(TimeSpan.FromSeconds(1));
            if (newTime.TotalSeconds < 1)
            {
                _currentTime = TimeSpan.FromSeconds(1);
            }
            else
            {
                _currentTime = newTime;
            }
            UpdateDisplay();
        }

        private void SecondMinus10_Click(object sender, RoutedEventArgs e)
        {
            var newTime = _currentTime.Subtract(TimeSpan.FromSeconds(10));
            if (newTime.TotalSeconds < 1)
            {
                _currentTime = TimeSpan.FromSeconds(1);
            }
            else
            {
                _currentTime = newTime;
            }
            UpdateDisplay();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTime.TotalSeconds < 1)
            {
                _currentTime = TimeSpan.FromSeconds(1);
            }
            Result = _currentTime;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
