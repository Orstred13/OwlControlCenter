using System.Diagnostics;
using System.Windows.Shapes;
using NAudio.CoreAudioApi;

namespace OwlControlCenter;

public class AppController {
    private Process[] Processes => GetProcessByName(ProcessName);

    public FunctionType Type;

    private float signalLevel;

    public float SignalLevel {
        get => signalLevel;
        set {
            value /= 1000;
            if (Math.Abs(signalLevel - value) < 0.01) return;
            value = (float)Math.Round(value, 2);
            signalLevel = value;
            switch (Type) {
                case FunctionType.Execute:
                    if (signalLevel > 0.9) StartApp();
                    break;
                case FunctionType.Close:
                    if (signalLevel > 0.9) CloseApp();
                    break;
                case FunctionType.Volume:
                    SetAppVolume(signalLevel);
                    break;
            }
        }
    }

    public string AppPath { get; set; }

    private string ProcessName => System.IO.Path.GetFileNameWithoutExtension(AppPath);

    public AppController(string appPath, FunctionType type) {
        AppPath = appPath;
        Type = type;
    }

    public Process[] GetProcessByName(string processName) {
        Process[] processes = Process.GetProcessesByName(processName);
        if (processes.Length > 0) {
            return processes;
        } else {
            return null;
        }
    }

    public void StartApp() {
        try {
            Process.Start(AppPath);
        } catch (Exception ex) {
            MessageBox.Show($"Ошибка при запуске приложения {ProcessName}", "Ошибка запуска",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void CloseApp() {
        try {
            foreach (var process in Processes) {
                process.CloseMainWindow();
                process.Close();
            }
        } catch (Exception ex) {
            MessageBox.Show($"Ошибка при закрытии приложения {ProcessName}", "Ошибка запуска",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void SetAppVolume(float volume) {
        if (volume < 0.0f) {
            volume = 0.0f;
        }

        if (volume > 1.0f) {
            volume = 1.0f;
        }

        try {
            var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            for (int i = 0; i < device.AudioSessionManager.Sessions.Count; i++) {
                foreach (var process in Processes) {
                    var session = device.AudioSessionManager.Sessions[i];
                    if (session.GetProcessID == process.Id) {
                        session.SimpleAudioVolume.Volume = volume;
                        return;
                    }
                }
            }
        } catch (Exception ex) {
            Console.WriteLine($"Ошибка при установке громкости: {ex.Message}");
        }
    }
}
