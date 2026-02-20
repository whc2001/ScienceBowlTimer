using System;
using System.IO;
using System.Linq;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace ScienceBowlTimer
{
    public class ApplicationManager
    {
        private readonly PublicDisplayWindow _publicDisplay;
        private readonly ControlWindow _controlPanel;
        private readonly TimerManager _timerManager;
        private readonly AudioManager _audioManager;
        private readonly GlobalKeyboardHook _keyboardHook;
        private readonly HotkeyConfig _hotkeyConfig;
        private readonly bool _isDualDisplay;

        public ApplicationManager()
        {
            _publicDisplay = new PublicDisplayWindow();
            _controlPanel = new ControlWindow();
            _timerManager = new TimerManager();
            _audioManager = new AudioManager(AppDomain.CurrentDomain.BaseDirectory);
            _keyboardHook = new GlobalKeyboardHook();

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hotkeys.json");
            _hotkeyConfig = HotkeyConfig.LoadFromFile(configPath);
            _hotkeyConfig.SaveToFile(configPath);

            InitializeTimerEvents();
            InitializeHotkeys();
            InitializeButtonHandlers();

            // Subscribe to application exit to ensure cleanup
            System.Windows.Application.Current.Exit += OnApplicationExit;

            _isDualDisplay = WinForms.Screen.AllScreens.Length > 1;
        }

        public void Start()
        {
            _publicDisplay.Show();

            if (_isDualDisplay)
            {
                PositionWindowsOnDisplays();
                _controlPanel.Show();
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
            PositionWindow(_controlPanel, primaryScreen);
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

        private void InitializeTimerEvents()
        {
            _timerManager.HalfChanged += half => _publicDisplay.UpdateHalf(half);
            _timerManager.RemainingTimeChanged += time => _publicDisplay.UpdateRemainingTime(time);
            _timerManager.QuestionTypeChanged += type => _publicDisplay.UpdateQuestionType(type);
            _timerManager.CountdownChanged += countdown => _publicDisplay.UpdateCountdown(countdown);
            _timerManager.HalfFinished += () => _audioManager.PlayHalfFinished();
            _timerManager.QuestionTimeUp += () => _audioManager.PlayTime();
            _timerManager.BonusFiveSeconds += () => _audioManager.PlayFiveSeconds();
            _timerManager.HalfTimerPausedChanged += paused => _publicDisplay.SetHalfTimerPaused(paused);
        }

        private void InitializeHotkeys()
        {
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartFirstHalf), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StartFirstHalf()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartSecondHalf), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StartSecondHalf()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.PauseResumeTimer), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.PauseResumeHalfTimer()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StopTimer), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StopHalfTimer()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartTossUp), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StartTossUp()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartBonus), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StartBonus()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.RestartLast), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.RestartLast()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StopQuestionTimer), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StopQuestionTimer()));
        }

        private void InitializeButtonHandlers()
        {
            _controlPanel.StartFirstHalfClicked += () => _timerManager.StartFirstHalf();
            _controlPanel.StartSecondHalfClicked += () => _timerManager.StartSecondHalf();
            _controlPanel.PauseResumeClicked += () => _timerManager.PauseResumeHalfTimer();
            _controlPanel.StopTimerClicked += () => _timerManager.StopHalfTimer();
            _controlPanel.StartTossUpClicked += () => _timerManager.StartTossUp();
            _controlPanel.StartBonusClicked += () => _timerManager.StartBonus();
            _controlPanel.RestartLastClicked += () => _timerManager.RestartLast();
            _controlPanel.StopQuestionTimerClicked += () => _timerManager.StopQuestionTimer();
            _controlPanel.SwapDisplaysClicked += SwapDisplays;
        }

        private void SwapDisplays()
        {
            if (!_isDualDisplay) return;

            var screens = WinForms.Screen.AllScreens;
            if (screens.Length < 2) return;

            var currentPublicScreen = screens.FirstOrDefault(s =>
                s.WorkingArea.Contains(new System.Drawing.Point((int)_publicDisplay.Left + 100, (int)_publicDisplay.Top + 100)));
            var currentControlScreen = screens.FirstOrDefault(s =>
                s.WorkingArea.Contains(new System.Drawing.Point((int)_controlPanel.Left + 100, (int)_controlPanel.Top + 100)));

            if (currentPublicScreen != null && currentControlScreen != null)
            {
                PositionWindow(_publicDisplay, currentControlScreen);
                PositionWindow(_controlPanel, currentPublicScreen);
            }
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            // Cleanup: Dispose of keyboard hook to unhook from system
            _keyboardHook?.Dispose();

            // Cleanup: Dispose of audio manager to release audio resources
            _audioManager?.Dispose();
        }
    }
}
