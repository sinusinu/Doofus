// Copyright 2024 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Doofus;

public class VolumeControlPlugin : IDoofusPlugin {
    public string Id => id;
    public string Name => name;
    public string Description => description;
    public string? Argument => argument;

    private string id = "volume-control";
    private string name = "Volume Control";
    private string description = "Sets the system audio volume.";
    private string? argument = "Absolute volume in percent (0-100)";

    public void Initialize(IDoofusEngineBridge engine) {
        if (engine.Language == "ko-KR") {
            name = "음량 조절";
            description = "시스템 오디오 음량을 조절합니다.";
            argument = "설정할 음량 절대값 (0-100)";
        }
    }

    public async Task DoActionAsync(IDoofusEngineBridge engine, string? argument) {
        await Task.Delay(1);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            if (int.TryParse(argument!, out var newVolume)) {
                new WindowsAudio().Volume = newVolume;
                var outputMessage = "Volume is set to {0} percent.";
                if (engine.Language == "ko-KR") outputMessage = "음량을 {0}%로 설정했습니다.";
                engine.Write(string.Format(outputMessage, newVolume));
            } else {
                var outputMessage = "An error has occurred while setting the volume.";
                if (engine.Language == "ko-KR") outputMessage = "음량 설정 중 오류가 발생했습니다.";
                engine.Write(outputMessage);
            }
        } else {
            var outputMessage = "Volume control is not supported on this platform.";
            if (engine.Language == "ko-KR") outputMessage = "이 운영체제에서는 음량 조절을 사용할 수 없습니다.";
            engine.Write(outputMessage);
        }
    }

    // https://stackoverflow.com/questions/13139181/how-to-programmatically-set-the-system-volume
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IAudioEndpointVolume {
        int _0(); int _1(); int _2(); int _3();
        int SetMasterVolumeLevelScalar(float fLevel, Guid pguidEventContext);
        int _5();
        int GetMasterVolumeLevelScalar(out float pfLevel);
        int _7(); int _8(); int _9(); int _10(); int _11(); int _12();
    }

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IMMDevice {
        int Activate(ref Guid id, int clsCtx, int activationParams, out IAudioEndpointVolume aev);
    }

    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IMMDeviceEnumerator {
        int _0();
        int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice endpoint);
    }

    [ComImport, Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")] class MMDeviceEnumeratorComObject { }

    internal class WindowsAudio {
        private readonly IAudioEndpointVolume _MMVolume;

        internal WindowsAudio() {
            var enumerator = new MMDeviceEnumeratorComObject() as IMMDeviceEnumerator;
            enumerator!.GetDefaultAudioEndpoint(0, 1, out IMMDevice dev);
            var aevGuid = typeof(IAudioEndpointVolume).GUID;
            dev.Activate(ref aevGuid, 1, 0, out _MMVolume);
        }

        internal int Volume {
            get {
                _MMVolume.GetMasterVolumeLevelScalar(out float level);
                return (int)(level * 100);
            }
            set {
                _MMVolume.SetMasterVolumeLevelScalar((float)value / 100, default);
            }
        }
    }
}