using System;
using System.Windows;

namespace ScienceBowlTimer
{
    public partial class ControlWindow : Window
    {
        public event Action? StartFirstHalfClicked;
        public event Action? StartSecondHalfClicked;
        public event Action? StartBreakClicked;
        public event Action? PauseResumeClicked;
        public event Action? StopTimerClicked;
        public event Action? StartTossUpClicked;
        public event Action? StartBonusClicked;
        public event Action? RestartLastClicked;
        public event Action? StopQuestionTimerClicked;
        public event Action? SwapDisplaysClicked;
        public event Func<TimeSpan, TimeSpan?>? AdjustHalfTimerClicked;

        private bool _isHalfTimerPaused;

        public ControlWindow()
        {
            InitializeComponent();
            _isHalfTimerPaused = false;
            UpdateAdjustButtonState();
        }

        public void SetHalfTimerPaused(bool isPaused)
        {
            _isHalfTimerPaused = isPaused;
            UpdateAdjustButtonState();
        }

        private void UpdateAdjustButtonState()
        {
            AdjustHalfTimerButton.IsEnabled = _isHalfTimerPaused;
        }

        private void StartFirstHalf_Click(object sender, RoutedEventArgs e)
        {
            StartFirstHalfClicked?.Invoke();
        }

        private void StartSecondHalf_Click(object sender, RoutedEventArgs e)
        {
            StartSecondHalfClicked?.Invoke();
        }

        private void StartBreak_Click(object sender, RoutedEventArgs e)
        {
            StartBreakClicked?.Invoke();
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

        private void AdjustHalfTimer_Click(object sender, RoutedEventArgs e)
        {
            if (!_isHalfTimerPaused) return;

            if (AdjustHalfTimerClicked != null)
            {
                TimeSpan currentTime = TimeSpan.Zero;
                TimeSpan? adjustedTime = AdjustHalfTimerClicked.Invoke(currentTime);
            }
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
