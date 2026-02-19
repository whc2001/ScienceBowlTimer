using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace ScienceBowlTimer
{
    public class GlobalKeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private readonly Dictionary<HotkeyCombo, Action> _hotkeys = new();

        public GlobalKeyboardHook()
        {
            _proc = HookCallback;
        }

        public void Start()
        {
            _hookID = SetHook(_proc);
        }

        public void Stop()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        public void RegisterHotkey(HotkeyCombo combo, Action action)
        {
            _hotkeys[combo] = action;
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                if (curModule != null)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
            return IntPtr.Zero;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Key key = KeyInterop.KeyFromVirtualKey(vkCode);

                bool ctrl = (Keyboard.Modifiers & ModifierKeys.Control) != 0;
                bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
                bool alt = (Keyboard.Modifiers & ModifierKeys.Alt) != 0;

                var combo = new HotkeyCombo(key, ctrl, shift, alt);

                if (_hotkeys.TryGetValue(combo, out var action))
                {
                    action?.Invoke();
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }

    public class HotkeyCombo : IEquatable<HotkeyCombo>
    {
        public Key Key { get; }
        public bool Ctrl { get; }
        public bool Shift { get; }
        public bool Alt { get; }

        public HotkeyCombo(Key key, bool ctrl = false, bool shift = false, bool alt = false)
        {
            Key = key;
            Ctrl = ctrl;
            Shift = shift;
            Alt = alt;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as HotkeyCombo);
        }

        public bool Equals(HotkeyCombo? other)
        {
            return other != null &&
                   Key == other.Key &&
                   Ctrl == other.Ctrl &&
                   Shift == other.Shift &&
                   Alt == other.Alt;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Ctrl, Shift, Alt);
        }

        public static bool operator ==(HotkeyCombo? left, HotkeyCombo? right)
        {
            return EqualityComparer<HotkeyCombo>.Default.Equals(left, right);
        }

        public static bool operator !=(HotkeyCombo? left, HotkeyCombo? right)
        {
            return !(left == right);
        }
    }
}
