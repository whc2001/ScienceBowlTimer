using System;
using System.Windows;

namespace ScienceBowlTimer
{
    public partial class ControlWindow : Window
    {
        public event Action? StartFirstHalfClicked;
        public event Action? StartSecondHalfClicked;
        public event Action? PauseResumeClicked;
        public event Action? StopTimerClicked;
        public event Action? StartTossUpClicked;
        public event Action? StartBonusClicked;
        public event Action? RestartLastClicked;
        public event Action? StopQuestionTimerClicked;
        public event Action? SwapDisplaysClicked;

        public ControlWindow()
        {
            InitializeComponent();
        }

        private void StartFirstHalf_Click(object sender, RoutedEventArgs e)
        {
            StartFirstHalfClicked?.Invoke();
        }

        private void StartSecondHalf_Click(object sender, RoutedEventArgs e)
        {
            StartSecondHalfClicked?.Invoke();
        }

        private void PauseResume_Click(object sender, RoutedEventArgs e)
        {
            PauseResumeClicked?.Invoke();
        }

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            StopTimerClicked?.Invoke();
        }

        private void StartTossUp_Click(object sender, RoutedEventArgs e)
        {
            StartTossUpClicked?.Invoke();
        }

        private void StartBonus_Click(object sender, RoutedEventArgs e)
        {
            StartBonusClicked?.Invoke();
        }

        private void RestartLast_Click(object sender, RoutedEventArgs e)
        {
            RestartLastClicked?.Invoke();
        }

        private void StopQuestionTimer_Click(object sender, RoutedEventArgs e)
        {
            StopQuestionTimerClicked?.Invoke();
        }

        private void SwapDisplays_Click(object sender, RoutedEventArgs e)
        {
            SwapDisplaysClicked?.Invoke();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Shutdown the entire application when the control panel is closed
            System.Windows.Application.Current.Shutdown();
        }
    }
}
