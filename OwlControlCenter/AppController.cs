using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Input;
using NAudio.CoreAudioApi;

namespace OwlControlCenter;

public class AppController : INotifyPropertyChanged {
    private const float EXECUTE_LEVEL = 0.9f;
    private Process[] Processes => GetProcessByName(ProcessName);

    private FunctionType type = FunctionType.None;
    public FunctionType Type {
        get => type;
        set => SetField(ref type, value);
    }

    [JsonIgnore]
    public bool IsReady = false;

    private float signalLevel;

    public float SignalLevel {
        get => signalLevel;
        set {
            //if (string.IsNullOrEmpty(AppPath)) return;
            
            value /= 1000;
            if (Math.Abs(signalLevel - value) < 0.01) return;
            value = (float)Math.Round(value, 2);
            if (IsReady) {
                switch (Type) {
                    case FunctionType.Execute:
                        if (value >= EXECUTE_LEVEL && signalLevel < EXECUTE_LEVEL) StartApp();
                        break;
                    case FunctionType.Close:
                        if (value >= EXECUTE_LEVEL && signalLevel < EXECUTE_LEVEL) CloseApp();
                        break;
                    case FunctionType.Volume:
                        SetAppVolume(value);
                        break;
                }
            }

            SetField(ref signalLevel, value);
        }
    }

    private string appPath;
    public string AppPath {
        get => appPath;
        set => SetField(ref appPath, value);
    }

    private string ProcessName => System.IO.Path.GetFileNameWithoutExtension(AppPath);
    
    public RelayCommand OpenPathDialogCommand { get; private set; }

    public AppController(string appPath, FunctionType type) {
        OpenPathDialogCommand = new RelayCommand(OpenPathDialog);
        
        AppPath = appPath;
        Type = type;
    }
    
    private void OpenPathDialog() {
        Microsoft.Win32.OpenFileDialog saveFileDialog = new Microsoft.Win32.OpenFileDialog();
        saveFileDialog.Filter = "Exe files (*.exe)|*.exe|All files (*.*)|*.*";

        if (saveFileDialog.ShowDialog() == true) {
            AppPath = saveFileDialog.FileName;
        }
    }

    public Process[] GetProcessByName(string processName) {
        if (string.IsNullOrEmpty(processName)) return [];

        Process[] processes = Process.GetProcessesByName(processName);
        if (processes.Length > 0) {
            return processes;
        }

        return [];
    }

    public void StartApp() {
        if (string.IsNullOrEmpty(AppPath)) return;
        
        if (Processes.Length > 0) return;

        try {
            Process.Start(AppPath);
        } catch (Exception ex) {
            MessageBox.Show($"Ошибка при запуске приложения {ProcessName}", "Ошибка запуска",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void CloseApp() {
        if (string.IsNullOrEmpty(AppPath)) return;

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
        if (string.IsNullOrEmpty(AppPath)) return;

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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
