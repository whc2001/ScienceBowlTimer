using ScienceBowlTimer.WinAPI;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
        }

        public void Start()
        {
            if (DisplayInfo.IsDualDisplay())
            {
                // Position BEFORE showing to ensure correct placement
                PositionWindow(_publicDisplay, DisplayInfo.GetSecondaryScreen()!);
                PositionWindow(_controlPanel, DisplayInfo.GetPrimaryScreen());

                _publicDisplay.Show();
                _publicDisplay.WindowState = WindowState.Maximized;

                _controlPanel.Show();
                _controlPanel.WindowState = WindowState.Maximized;
            }
            else
            {
                _publicDisplay.Show();
            }

            _keyboardHook.Start();
        }

        private void PositionWindow(Window window, WinForms.Screen screen)
        {
            window.WindowState = WindowState.Normal;
            window.WindowStartupLocation = WindowStartupLocation.Manual;

            // Get the DPI scale factor for the target screen
            var targetDpiScale = DisplayInfo.GetDPIScaleForScreen(screen);

            // Convert physical pixels to WPF device-independent units using the target screen's DPI
            window.Left = screen.Bounds.Left / targetDpiScale;
            window.Top = screen.Bounds.Top / targetDpiScale;
            window.Width = screen.Bounds.Width / targetDpiScale;
            window.Height = screen.Bounds.Height / targetDpiScale;
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
            _timerManager.HalfTimerPausedChanged += paused =>
            {
                _publicDisplay.SetHalfTimerPaused(paused);
                _controlPanel.SetHalfTimerPaused(paused);
            };
        }

        private void InitializeHotkeys()
        {
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartFirstHalf), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StartFirstHalf()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartSecondHalf), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StartSecondHalf()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartBreak), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StartBreak()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.PauseResumeTimer), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.PauseResumeHalfTimer()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StopTimer), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StopHalfTimer()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartTossUp), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StartTossUp()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StartBonus), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StartBonus()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.RestartLast), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.RestartLast()));
            _keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey(_hotkeyConfig.StopQuestionTimer), () => _controlPanel.Dispatcher.Invoke(() => _timerManager.StopQuestionTimer()));
            
            ////FIXME: DEBUG
            //_keyboardHook.RegisterHotkey(HotkeyConfig.ParseHotkey("F1"), () =>
            //{
            //    StringBuilder sb = new StringBuilder();
            //    sb.AppendLine("Screens:");
            //    var primary = DisplayInfo.GetPrimaryScreen();
            //    sb.AppendLine($"Primary: {primary.DeviceName} {primary.Bounds} DPI={DisplayInfo.GetDPIScaleForScreen(primary)}");
            //    var secondary = DisplayInfo.GetSecondaryScreen();
            //    if (secondary != null)
            //    {
            //        sb.AppendLine($"Secondary: {secondary.DeviceName} {secondary.Bounds} DPI={DisplayInfo.GetDPIScaleForScreen(secondary)}");
            //    }
            //    sb.AppendLine();

            //    var publicScreen = DisplayInfo.GetScreenFromWindow(_publicDisplay);
            //    var controlScreen = DisplayInfo.GetScreenFromWindow(_controlPanel);

            //    sb.AppendLine($"PublicDisplay: {_publicDisplay.Left}, {_publicDisplay.Top}, {_publicDisplay.Width}x{_publicDisplay.Height}");
            //    sb.AppendLine($"  -> Screen: {publicScreen?.DeviceName ?? "NULL"} {publicScreen?.Bounds.ToString() ?? "NULL"}");
            //    sb.AppendLine($"ControlPanel: {_controlPanel.Left}, {_controlPanel.Top}, {_controlPanel.Width}x{_controlPanel.Height}");
            //    sb.AppendLine($"  -> Screen: {controlScreen?.DeviceName ?? "NULL"} {controlScreen?.Bounds.ToString() ?? "NULL"}");

            //    System.Windows.MessageBox.Show(sb.ToString(), "Debug Info", MessageBoxButton.OK, MessageBoxImage.Information);
            //});
        }

        private void InitializeButtonHandlers()
        {
            _controlPanel.StartFirstHalfClicked += () => _timerManager.StartFirstHalf();
            _controlPanel.StartSecondHalfClicked += () => _timerManager.StartSecondHalf();
            _controlPanel.StartBreakClicked += () => _timerManager.StartBreak();
            _controlPanel.PauseResumeClicked += () => _timerManager.PauseResumeHalfTimer();
            _controlPanel.StopTimerClicked += () => _timerManager.StopHalfTimer();
            _controlPanel.StartTossUpClicked += () => _timerManager.StartTossUp();
            _controlPanel.StartBonusClicked += () => _timerManager.StartBonus();
            _controlPanel.RestartLastClicked += () => _timerManager.RestartLast();
            _controlPanel.StopQuestionTimerClicked += () => _timerManager.StopQuestionTimer();
            _controlPanel.SwapDisplaysClicked += SwapDisplays;
            _controlPanel.AdjustHalfTimerClicked += AdjustHalfTimer;
        }

        private TimeSpan? AdjustHalfTimer(TimeSpan currentTime)
        {
            TimeSpan actualTime = _timerManager.GetHalfTimeRemaining();
            var adjustWindow = new TimeAdjustDialog(actualTime);
            adjustWindow.Owner = _controlPanel;

            if (adjustWindow.ShowDialog() == true)
            {
                _timerManager.SetHalfTimeRemaining(adjustWindow.Result);
                return adjustWindow.Result;
            }

            return null;
        }

        private void SwapDisplays()
        {
            if (!DisplayInfo.IsDualDisplay()) return;

            var currentPublicScreen = DisplayInfo.GetScreenFromWindow(_publicDisplay);
            var currentControlScreen = DisplayInfo.GetScreenFromWindow(_controlPanel);

            if (currentPublicScreen != null && currentControlScreen != null)
            {
                PositionWindow(_publicDisplay, currentControlScreen);
                PositionWindow(_controlPanel, currentPublicScreen);

                _publicDisplay.WindowState = WindowState.Maximized;
                _controlPanel.WindowState = WindowState.Maximized;
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
