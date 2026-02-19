using System.Windows;

namespace ScienceBowlTimer
{
    public partial class PublicDisplayWindow : Window
    {
        public PublicDisplayWindow()
        {
            InitializeComponent();
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
