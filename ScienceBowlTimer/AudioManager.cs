using System;
using System.IO;
using System.Media;

namespace ScienceBowlTimer
{
    public class AudioManager : IDisposable
    {
        private readonly SoundPlayer? _timePlayer;
        private readonly SoundPlayer? _fiveSecondsPlayer;
        private readonly SoundPlayer? _halfFinishedPlayer;

        public AudioManager(string basePath)
        {
            // Preload audio files to eliminate first-play delay
            _timePlayer = LoadAudioFile(basePath, "Time.wav");
            _fiveSecondsPlayer = LoadAudioFile(basePath, "FiveSeconds.wav");
            _halfFinishedPlayer = LoadAudioFile(basePath, "HalfFinished.wav");
        }

        private SoundPlayer? LoadAudioFile(string basePath, string filename)
        {
            try
            {
                string filePath = Path.Combine(basePath, filename);
                if (File.Exists(filePath))
                {
                    var player = new SoundPlayer(filePath);
                    player.Load(); // Synchronously load the audio file into memory
                    return player;
                }
            }
            catch
            {
                // Silently fail if audio file cannot be loaded
            }
            return null;
        }

        public void PlayTime()
        {
            PlayAudio(_timePlayer);
        }

        public void PlayFiveSeconds()
        {
            PlayAudio(_fiveSecondsPlayer);
        }

        public void PlayHalfFinished()
        {
            PlayAudio(_halfFinishedPlayer);
        }

        private void PlayAudio(SoundPlayer? player)
        {
            if (player != null)
            {
                try
                {
                    // Maximize volume without showing Windows UI popup
                    WindowsVolumeControl.SetVolumeToMaximum();

                    // Play the preloaded audio file
                    player.Play();
                }
                catch
                {
                    // Silently fail if audio playback fails
                }
            }
        }

        public void Dispose()
        {
            _timePlayer?.Dispose();
            _fiveSecondsPlayer?.Dispose();
            _halfFinishedPlayer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
