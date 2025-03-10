using System.Diagnostics;
using System.Windows.Shapes;
using NAudio.CoreAudioApi;

namespace OwlControlCenter;

public class AppController {
    private Process[] _processes {
        get => GetProcessByName(processName);
    }

    public string AppPath { get; }

    private string processName {
        get => System.IO.Path.GetFileNameWithoutExtension(AppPath);
    }

    public AppController(string appPath) {
        AppPath = appPath;
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
            foreach (var _process in _processes) {
                _process.CloseMainWindow();
                _process.Close();
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
                foreach (var _process in _processes) {
                    var session = device.AudioSessionManager.Sessions[i];
                    if (session.GetProcessID == _process.Id) {
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
