using System;
using System.Windows;
using System.Windows.Threading;

namespace ScienceBowlTimer
{
    /// <summary>
    /// Public display window for the Science Bowl timer.
    /// Handles blinking of the time display when paused.
    /// </summary>
    public partial class PublicDisplayWindow : Window
    {
        private bool paused = false;
        private readonly DispatcherTimer _blinkTimer;
        private bool _blinkState;

        public PublicDisplayWindow()
        {
            InitializeComponent();

            // Initialize blink timer for pause indication
            _blinkTimer = new DispatcherTimer();
            _blinkTimer.Interval = TimeSpan.FromMilliseconds(500);
            _blinkTimer.Tick += BlinkTimer_Tick;
        }

        public void UpdateHalf(string half)
        {
            Dispatcher.Invoke(() => CurrentHalfText.Text = half);
        }

        public void UpdateRemainingTime(string time)
        {
            Dispatcher.Invoke(() => RemainingTimeText.Text = time);
        }

        public void UpdateQuestionType(string questionType)
        {
            Dispatcher.Invoke(() => QuestionTypeText.Text = questionType);
        }

        public void UpdateCountdown(string countdown)
        {
            Dispatcher.Invoke(() => CountdownText.Text = countdown);
        }

        public void SetHalfTimerPaused(bool paused)
        {
            Dispatcher.Invoke(() =>
            {
                if (paused)
                {
                    RemainingTimeText.Visibility = Visibility.Hidden;
                    _blinkState = true;
                    _blinkTimer.Start();
                }
                else
                {
                    _blinkTimer.Stop();
                    RemainingTimeText.Visibility = Visibility.Visible;
                }
            });
        }

        private void BlinkTimer_Tick(object? sender, EventArgs e)
        {
            RemainingTimeText.Visibility = _blinkState ? Visibility.Visible : Visibility.Hidden;
            _blinkState = !_blinkState;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
