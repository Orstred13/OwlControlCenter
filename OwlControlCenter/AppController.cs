using System.Diagnostics;
using System.Windows.Shapes;
using NAudio.CoreAudioApi;

namespace OwlControlCenter;

public class AppController {
    private Process[] processes {
        get => GetProcessByName(processName);
    }

    private FunctionType type;

    private float signalLevel;

    public float SignalLevel {
        get => signalLevel;
        set {
            if (Math.Abs(signalLevel - value) < 0.01) return;
            value = (float)Math.Round(value, 2);
            signalLevel = value;
            switch (type) {
                case FunctionType.Execute:
                    if (signalLevel < 50) CloseApp();
                    else {
                        StartApp();
                    }

                    break;
                case FunctionType.Volume:
                    SetAppVolume(signalLevel);
                    break;
            }
        }
    }

    public string AppPath { get; }

    private string processName {
        get => System.IO.Path.GetFileNameWithoutExtension(AppPath);
    }

    public AppController(string appPath, FunctionType type) {
        AppPath = appPath;
        this.type = type;
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
            MessageBox.Show($"Ошибка при запуске приложения {processName}", "Ошибка запуска",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void CloseApp() {
        try {
            foreach (var process in processes) {
                process.CloseMainWindow();
                process.Close();
            }
        } catch (Exception ex) {
            MessageBox.Show($"Ошибка при закрытии приложения {processName}", "Ошибка запуска",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void SetAppVolume(float volume) {
        if (volume < 0.0f || volume > 1.0f) {
            return;
        }

        try {
            var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            for (int i = 0; i < device.AudioSessionManager.Sessions.Count; i++) {
                foreach (var process in processes) {
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
