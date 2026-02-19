using System;
using System.Runtime.InteropServices;

namespace ScienceBowlTimer
{
    /// <summary>
    /// Provides volume control functionality using the Windows Core Audio API.
    /// This allows setting the system volume without triggering the Windows volume UI popup.
    /// </summary>
    public static class WindowsVolumeControl
    {
        /// <summary>
        /// COM class for enumerating multimedia devices (speakers, microphones, etc.)
        /// CLSID: BCDE0395-E52F-467C-8E3D-C4579291692E
        /// </summary>
        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        private class MMDeviceEnumerator
        {
        }

        /// <summary>
        /// Specifies the direction of audio data flow (render = output, capture = input)
        /// </summary>
        private enum EDataFlow
        {
            eRender,    // Audio output (speakers, headphones)
            eCapture,   // Audio input (microphones)
            eAll,       // All devices
        }

        /// <summary>
        /// Specifies the role of an audio device
        /// </summary>
        private enum ERole
        {
            eConsole,        // System sounds
            eMultimedia,     // Music, videos, games
            eCommunications, // Voice communications
        }

        /// <summary>
        /// COM interface for enumerating multimedia devices
        /// IID: A95664D2-9614-4F35-A746-DE8DB63617E6
        /// </summary>
        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceEnumerator
        {
            int NotImpl1();
            
            /// <summary>
            /// Gets the default audio endpoint for the specified data flow direction and role
            /// </summary>
            [PreserveSig]
            int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);
        }

        /// <summary>
        /// COM interface representing a multimedia device
        /// IID: D666063F-1587-4E43-81F1-B948E807363F
        /// </summary>
        [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDevice
        {
            /// <summary>
            /// Activates a COM interface on the device
            /// </summary>
            [PreserveSig]
            int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
        }

        /// <summary>
        /// COM interface for controlling audio endpoint volume
        /// IID: 5CDF2C82-841E-4546-9722-0CF74078229A
        /// </summary>
        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioEndpointVolume
        {
            int NotImpl1();
            int NotImpl2();
            
            [PreserveSig]
            int GetChannelCount(out int pnChannelCount);
            
            /// <summary>
            /// Sets the master volume level in decibels
            /// </summary>
            [PreserveSig]
            int SetMasterVolumeLevel(float fLevelDB, ref Guid pguidEventContext);
            
            /// <summary>
            /// Sets the master volume level as a scalar value (0.0 to 1.0)
            /// </summary>
            [PreserveSig]
            int SetMasterVolumeLevelScalar(float fLevel, ref Guid pguidEventContext);
            
            [PreserveSig]
            int GetMasterVolumeLevel(out float pfLevelDB);
            
            [PreserveSig]
            int GetMasterVolumeLevelScalar(out float pfLevel);
        }

        /// <summary>
        /// Sets the system master volume to maximum (100%) without showing the Windows volume UI.
        /// Uses the Windows Core Audio API to directly control the audio endpoint.
        /// </summary>
        public static void SetVolumeToMaximum()
        {
            try
            {
                // Create the device enumerator
                var deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                
                // Get the default audio output device (speakers/headphones) for multimedia
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out IMMDevice device);
                
                // Activate the volume control interface on the device
                var iidIAudioEndpointVolume = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");
                device.Activate(ref iidIAudioEndpointVolume, 0, IntPtr.Zero, out object obj);
                
                // Cast to the volume control interface
                var endpointVolume = (IAudioEndpointVolume)obj;
                
                // Set volume to maximum (1.0 = 100%)
                var guid = Guid.Empty;
                endpointVolume.SetMasterVolumeLevelScalar(1.0f, ref guid);
            }
            catch
            {
                // Silently fail if volume control is unavailable
                // This can happen if no audio devices are present or COM interop fails
            }
        }
    }
}
