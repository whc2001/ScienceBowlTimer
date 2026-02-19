using System;
using System.IO;
using System.Linq;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace ScienceBowlTimer
{
    public partial class MainWindow : Window
    {
        private readonly PublicDisplayWindow _publicDisplay;
        private readonly TimerManager _timerManager;
        private readonly AudioManager _audioManager;
        private readonly GlobalKeyboardHook _keyboardHook;
        private readonly HotkeyConfig _hotkeyConfig;
        private readonly bool _isDualDisplay;

        public MainWindow()
        {
            InitializeComponent();

            _publicDisplay = new PublicDisplayWindow();
            _timerManager = new TimerManager();
            _audioManager = new AudioManager(AppDomain.CurrentDomain.BaseDirectory);
            _keyboardHook = new GlobalKeyboardHook();

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hotkeys.json");
            _hotkeyConfig = HotkeyConfig.LoadFromFile(configPath);
            _hotkeyConfig.SaveToFile(configPath);

            InitializeTimerEvents();
            InitializeHotkeys();

            _isDualDisplay = WinForms.Screen.AllScreens.Length > 1;

            _publicDisplay.Show();

            if (_isDualDisplay)
            {
                PositionWindowsOnDisplays();
                Show();
            }
            else
            {
                Hide();
            }

            _keyboardHook.Start();
        }

        private void PositionWindowsOnDisplays()
        {
            var screens = WinForms.Screen.AllScreens;
            if (screens.Length < 2) return;

            var primaryScreen = screens.First(s => s.Primary);
            var secondaryScreen = screens.First(s => !s.Primary);

            PositionWindow(_publicDisplay, secondaryScreen);
            PositionWindow(this, primaryScreen);
        }

        private void PositionWindow(Window window, WinForms.Screen screen)
        {
            window.WindowState = WindowState.Normal;
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = screen.Bounds.Left;
            window.Top = screen.Bounds.Top;
            window.Width = screen.Bounds.Width;
            window.Height = screen.Bounds.Height;
            window.WindowState = WindowState.Maximized;
            window.Activate();
        }

        private void SwapDisplays_Click(object sender, RoutedEventArgs e)
        {
            if (!_isDualDisplay) return;

            var screens = WinForms.Screen.AllScreens;
            if (screens.Length < 2) return;

            var currentPublicScreen = screens.FirstOrDefault(s => 
                s.WorkingArea.Contains(new System.Drawing.Point((int)_publicDisplay.Left + 100, (int)_publicDisplay.Top + 100)));
            var currentControlScreen = screens.FirstOrDefault(s => 
                s.WorkingArea.Contains(new System.Drawing.Point((int)Left + 100, (int)Top + 100)));

            if (currentPublicScreen != null && currentControlScreen != null)
            {
                PositionWindow(_publicDisplay, currentControlScreen);
                PositionWindow(this, currentPublicScreen);
            }
        }

        private void InitializeTimerEvents()
        {
            _timerManager.HalfChanged += half => _publicDisplay.UpdateHalf(half);
            _timerManager.RemainingTimeChanged += time => _publicDisplay.UpdateRemainingTime(time);
            _timerManager.QuestionTypeChanged += type => _publicDisplay.UpdateQuestionType(type);
            _timerManager.CountdownChanged += countdown => _publicDisplay.UpdateCountdown(countdown);
            _timerManager.HalfFinished += () => _audioManager.PlayHalfFinished();
            _timerManager.QuestionTimeUp += () => _audioManager.PlayTime();
            _timerManager.BonusFiveSeconds += () => _audioManager.PlayFiveSeconds();
        }

        private void InitializeHotkeys()
        {
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartFirstHalf), () => Dispatcher.Invoke(StartFirstHalf_Click, this, new RoutedEventArgs()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartSecondHalf), () => Dispatcher.Invoke(StartSecondHalf_Click, this, new RoutedEventArgs()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StopTimer), () => Dispatcher.Invoke(StopTimer_Click, this, new RoutedEventArgs()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartTossUp), () => Dispatcher.Invoke(StartTossUp_Click, this, new RoutedEventArgs()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartBonus), () => Dispatcher.Invoke(StartBonus_Click, this, new RoutedEventArgs()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.RestartLast), () => Dispatcher.Invoke(RestartLast_Click, this, new RoutedEventArgs()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StopQuestionTimer), () => Dispatcher.Invoke(StopQuestionTimer_Click, this, new RoutedEventArgs()));
        }

        private void StartFirstHalf_Click(object sender, RoutedEventArgs e)
        {
            _timerManager.StartFirstHalf();
        }

        private void StartSecondHalf_Click(object sender, RoutedEventArgs e)
        {
            _timerManager.StartSecondHalf();
        }

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            _timerManager.StopHalfTimer();
        }

        private void StartTossUp_Click(object sender, RoutedEventArgs e)
        {
            _timerManager.StartTossUp();
        }

        private void StartBonus_Click(object sender, RoutedEventArgs e)
        {
            _timerManager.StartBonus();
        }

        private void RestartLast_Click(object sender, RoutedEventArgs e)
        {
            _timerManager.RestartLast();
        }

        private void StopQuestionTimer_Click(object sender, RoutedEventArgs e)
        {
            _timerManager.StopQuestionTimer();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _keyboardHook.Dispose();
            _publicDisplay.Close();
        }
    }
}