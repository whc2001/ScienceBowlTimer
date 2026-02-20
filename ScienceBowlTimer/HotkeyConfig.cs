using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace ScienceBowlTimer
{
    public class HotkeyConfig
    {
        public string StartFirstHalf { get; set; } = "Q";
        public string StartBreak { get; set; } = "W";
        public string StartSecondHalf { get; set; } = "E";
        public string PauseResumeTimer { get; set; } = "R";
        public string StopTimer { get; set; } = "T";
        public string StartTossUp { get; set; } = "Z";
        public string StartBonus { get; set; } = "X";
        public string RestartLast { get; set; } = "C";
        public string StopQuestionTimer { get; set; } = "V";

        public static HotkeyConfig LoadFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<HotkeyConfig>(json) ?? new HotkeyConfig();
                }
            }
            catch
            {
            }
            return new HotkeyConfig();
        }

        public void SaveToFile(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(filePath, json);
            }
            catch
            {
            }
        }

        public static HotkeyCombo ParseHotkey(string hotkeyString)
        {
            bool ctrl = false;
            bool shift = false;
            bool alt = false;
            string keyString = hotkeyString;

            if (hotkeyString.Contains("Ctrl+", StringComparison.OrdinalIgnoreCase))
            {
                ctrl = true;
                keyString = keyString.Replace("Ctrl+", "", StringComparison.OrdinalIgnoreCase);
            }
            if (hotkeyString.Contains("Shift+", StringComparison.OrdinalIgnoreCase))
            {
                shift = true;
                keyString = keyString.Replace("Shift+", "", StringComparison.OrdinalIgnoreCase);
            }
            if (hotkeyString.Contains("Alt+", StringComparison.OrdinalIgnoreCase))
            {
                alt = true;
                keyString = keyString.Replace("Alt+", "", StringComparison.OrdinalIgnoreCase);
            }

            Key key = Key.None;
            try
            {
                key = (Key)Enum.Parse(typeof(Key), keyString, true);
            }
            catch
            {
            }

            return new HotkeyCombo(key, ctrl, shift, alt);
        }
    }
}
