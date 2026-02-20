using System;
using System.Windows.Threading;

namespace ScienceBowlTimer
{
    public enum GameHalf
    {
        None,
        First,
        Second
    }

    public enum QuestionType
    {
        None,
        TossUp,
        Bonus
    }

    public class TimerManager
    {
        private readonly DispatcherTimer _halfTimer;
        private readonly DispatcherTimer _questionTimer;
        private TimeSpan _halfTimeRemaining;
        private int _questionSecondsRemaining;
        private QuestionType _currentQuestionType;
        private GameHalf _currentHalf;
        private int _lastQuestionSeconds;
        private bool _questionTimerStopped;
        private bool _halfTimerPaused;

        public event Action<string>? HalfChanged;
        public event Action<string>? RemainingTimeChanged;
        public event Action<string>? QuestionTypeChanged;
        public event Action<string>? CountdownChanged;
        public event Action? HalfFinished;
        public event Action? QuestionTimeUp;
        public event Action? BonusFiveSeconds;
        public event Action<bool>? HalfTimerPausedChanged;

        public TimerManager()
        {
            _halfTimer = new DispatcherTimer();
            _halfTimer.Interval = TimeSpan.FromSeconds(1);
            _halfTimer.Tick += HalfTimer_Tick;

            _questionTimer = new DispatcherTimer();
            _questionTimer.Interval = TimeSpan.FromSeconds(1);
            _questionTimer.Tick += QuestionTimer_Tick;

            _currentHalf = GameHalf.None;
            _currentQuestionType = QuestionType.None;
        }

        public void StartFirstHalf()
        {
            _currentHalf = GameHalf.First;
            _halfTimeRemaining = TimeSpan.FromMinutes(8);
            _halfTimer.Stop(); // Stop first to reset the tick cycle
            _halfTimer.Start();
            HalfChanged?.Invoke("FIRST HALF");
            HalfTimerPausedChanged?.Invoke(false);
            RemainingTimeChanged?.Invoke(FormatTime(_halfTimeRemaining));
        }

        public void StartSecondHalf()
        {
            _currentHalf = GameHalf.Second;
            _halfTimeRemaining = TimeSpan.FromMinutes(8);
            _halfTimer.Stop(); // Stop first to reset the tick cycle
            _halfTimer.Start();
            HalfChanged?.Invoke("SECOND HALF");
            HalfTimerPausedChanged?.Invoke(false);
            RemainingTimeChanged?.Invoke(FormatTime(_halfTimeRemaining));
        }

        public void StopHalfTimer()
        {
            _halfTimer.Stop();
            _halfTimerPaused = false;
            _currentHalf = GameHalf.None;
            HalfTimerPausedChanged?.Invoke(false);
            HalfChanged?.Invoke("--");
            RemainingTimeChanged?.Invoke("-:--");
        }

        public void PauseResumeHalfTimer()
        {
            if (_currentHalf == GameHalf.None || _halfTimeRemaining.TotalSeconds == 0)
            {
                // No half timer running, do nothing
                return;
            }

            if (_halfTimerPaused)
            {
                // Resume
                _halfTimer.Start();
                _halfTimerPaused = false;
                HalfTimerPausedChanged?.Invoke(_halfTimerPaused);
            }
            else
            {
                // Pause
                _halfTimer.Stop();
                _halfTimerPaused = true;
                HalfTimerPausedChanged?.Invoke(_halfTimerPaused);
            }
        }

        public void StartTossUp()
        {
            _currentQuestionType = QuestionType.TossUp;
            _questionSecondsRemaining = 5;
            _lastQuestionSeconds = 5;
            _questionTimerStopped = false;
            _questionTimer.Stop(); // Stop first to reset the tick cycle
            _questionTimer.Start();
            QuestionTypeChanged?.Invoke("TOSS-UP");
            CountdownChanged?.Invoke(_questionSecondsRemaining.ToString());
        }

        public void StartBonus()
        {
            _currentQuestionType = QuestionType.Bonus;
            _questionSecondsRemaining = 20;
            _lastQuestionSeconds = 20;
            _questionTimerStopped = false;
            _questionTimer.Stop(); // Stop first to reset the tick cycle
            _questionTimer.Start();
            QuestionTypeChanged?.Invoke("BONUS");
            CountdownChanged?.Invoke(_questionSecondsRemaining.ToString());
        }

        public void RestartLast()
        {
            if (_lastQuestionSeconds > 0)
            {
                _questionSecondsRemaining = _lastQuestionSeconds;
                _questionTimerStopped = false;
                _questionTimer.Stop(); // Stop first to reset the tick cycle
                _questionTimer.Start();

                if (_currentQuestionType == QuestionType.TossUp)
                    QuestionTypeChanged?.Invoke("TOSS-UP");
                else if (_currentQuestionType == QuestionType.Bonus)
                    QuestionTypeChanged?.Invoke("BONUS");

                CountdownChanged?.Invoke(_questionSecondsRemaining.ToString());
            }
        }

        public void StopQuestionTimer()
        {
            _questionTimer.Stop();
            _questionTimerStopped = true;
            _currentQuestionType = QuestionType.None;
            QuestionTypeChanged?.Invoke("-");
            CountdownChanged?.Invoke("-");
        }

        private void HalfTimer_Tick(object? sender, EventArgs e)
        {
            _halfTimeRemaining = _halfTimeRemaining.Subtract(TimeSpan.FromSeconds(1));
            RemainingTimeChanged?.Invoke(FormatTime(_halfTimeRemaining));

            if (_halfTimeRemaining.TotalSeconds <= 0)
            {
                _halfTimer.Stop();
                HalfFinished?.Invoke();
            }
        }

        private void QuestionTimer_Tick(object? sender, EventArgs e)
        {
            _questionSecondsRemaining--;
            CountdownChanged?.Invoke(_questionSecondsRemaining.ToString());

            if (_questionSecondsRemaining == 5 && _currentQuestionType == QuestionType.Bonus && !_questionTimerStopped)
            {
                BonusFiveSeconds?.Invoke();
            }

            if (_questionSecondsRemaining <= 0)
            {
                _questionTimer.Stop();
                if (!_questionTimerStopped)
                {
                    QuestionTimeUp?.Invoke();
                }
            }
        }

        private string FormatTime(TimeSpan time)
        {
            int minutes = (int)time.TotalMinutes;
            int seconds = time.Seconds;
            return $"{minutes}:{seconds:D2}";
        }
    }
}
