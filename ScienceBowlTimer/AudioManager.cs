using System;
using System.IO;
using System.Media;

namespace ScienceBowlTimer
{
    public class AudioManager
    {
        private readonly string _basePath;

        public AudioManager(string basePath)
        {
            _basePath = basePath;
        }

        public void PlayTime()
        {
            PlayAudioFile("Time.wav");
        }

        public void PlayFiveSeconds()
        {
            PlayAudioFile("FiveSeconds.wav");
        }

        public void PlayHalfFinished()
        {
            PlayAudioFile("HalfFinished.wav");
        }

        private void PlayAudioFile(string filename)
        {
            try
            {
                string filePath = Path.Combine(_basePath, filename);
                if (File.Exists(filePath))
                {
                    WindowsVolumeControl.SetVolumeToMaximum();
                    using (var player = new SoundPlayer(filePath))
                    {
                        player.Play();
                    }
                }
            }
            catch
            {
                // Silently fail if audio playback fails
            }
        }
    }
}
