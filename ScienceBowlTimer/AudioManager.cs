using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace ScienceBowlTimer
{
    /// <summary>
    /// Manages audio playback for Science Bowl timer events using NAudio for low-latency playback.
    /// Uses two separate audio channels with pre-initialized "always hot" audio devices:
    /// - Question channel: For Time and FiveSeconds sounds (interrupt each other)
    /// - Half channel: For HalfFinished sound (mixes with question channel)
    /// </summary>
    public class AudioManager : IDisposable
    {
        private readonly CachedSound? _timeSound;
        private readonly CachedSound? _fiveSecondsSound;
        private readonly CachedSound? _halfFinishedSound;

        // Pre-initialized audio channels that are always "hot"
        private readonly AudioChannel _questionChannel;
        private readonly AudioChannel _halfChannel;

        public AudioManager(string basePath)
        {
            // Preload audio files into memory
            _timeSound = LoadSound(basePath, "Time.wav");
            _fiveSecondsSound = LoadSound(basePath, "FiveSeconds.wav");
            _halfFinishedSound = LoadSound(basePath, "HalfFinished.wav");

            // Determine wave format from first available sound, or use default
            var format = _timeSound?.WaveFormat 
                      ?? _fiveSecondsSound?.WaveFormat 
                      ?? _halfFinishedSound?.WaveFormat 
                      ?? new WaveFormat(44100, 16, 2);

            // Pre-initialize audio channels - they start playing immediately (silence until fed data)
            _questionChannel = new AudioChannel(format);
            _halfChannel = new AudioChannel(format);
        }

        private CachedSound? LoadSound(string basePath, string filename)
        {
            try
            {
                string filePath = Path.Combine(basePath, filename);
                if (File.Exists(filePath))
                {
                    return new CachedSound(filePath);
                }
            }
            catch
            {
            }
            return null;
        }

        public void PlayTime()
        {
            PlayOnChannel(_questionChannel, _timeSound);
        }

        public void PlayFiveSeconds()
        {
            PlayOnChannel(_questionChannel, _fiveSecondsSound);
        }

        public void PlayHalfFinished()
        {
            PlayOnChannel(_halfChannel, _halfFinishedSound);
        }

        private void PlayOnChannel(AudioChannel channel, CachedSound? sound)
        {
            if (sound == null) return;

            Task.Run(() =>
            {
                try
                {
                    WindowsVolumeControl.SetVolumeToMaximum();
                    channel.Play(sound);
                }
                catch
                {
                }
            });
        }

        public void Dispose()
        {
            _questionChannel.Dispose();
            _halfChannel.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// A pre-initialized audio channel that's always ready for instant playback.
    /// Uses BufferedWaveProvider to avoid Init() latency on each play.
    /// Uses WASAPI for automatic latency detection based on device capabilities.
    /// </summary>
    internal class AudioChannel : IDisposable
    {
        private readonly IWavePlayer _waveOut;
        private readonly BufferedWaveProvider _buffer;
        private readonly object _lock = new();

        public AudioChannel(WaveFormat format)
        {
            _buffer = new BufferedWaveProvider(format)
            {
                BufferDuration = TimeSpan.FromSeconds(10),
                DiscardOnBufferOverflow = true
            };

            // Try WASAPI first (better latency auto-detection), fall back to WaveOut
            try
            {
                // WASAPI shared mode automatically uses optimal latency for the device
                var wasapi = new WasapiOut(
                    NAudio.CoreAudioApi.AudioClientShareMode.Shared,
                    useEventSync: true,
                    latency: 0); // 0 = use device default latency
                _waveOut = wasapi;
            }
            catch
            {
                // Fall back to WaveOutEvent with conservative settings
                _waveOut = new WaveOutEvent
                {
                    DesiredLatency = 150,
                    NumberOfBuffers = 3
                };
            }

            // Initialize once - the device is now "hot" and ready
            _waveOut.Init(_buffer);
            _waveOut.Play();
        }

        public void Play(CachedSound sound)
        {
            lock (_lock)
            {
                // Clear any existing audio (interrupts previous sound)
                _buffer.ClearBuffer();

                // Add the new audio data
                _buffer.AddSamples(sound.AudioData, 0, sound.AudioData.Length);
            }
        }

        public void Dispose()
        {
            _waveOut.Stop();
            _waveOut.Dispose();
        }
    }

    /// <summary>
    /// Represents an audio file loaded into memory for instant playback.
    /// </summary>
    internal class CachedSound
    {
        public byte[] AudioData { get; }
        public WaveFormat WaveFormat { get; }

        public CachedSound(string audioFilePath)
        {
            using var reader = new WaveFileReader(audioFilePath);
            WaveFormat = reader.WaveFormat;

            AudioData = new byte[reader.Length];
            reader.ReadExactly(AudioData);
        }
    }
}
