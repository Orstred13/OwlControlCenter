using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace OwlControlCenter;

public class MainWindowModel : INotifyPropertyChanged {
    private ConfigManager configManager;
    private Config config;

    private List<AppController> appControllers {
        get => config.AppControllers;
        set => config.AppControllers = value;
    }

    public MainWindowModel() {
        configManager = new ConfigManager();
        config = configManager.GetConfig();
        if (!string.IsNullOrEmpty(config.PortName)) {
            var comPortListener = new ComPortListener(config.PortName, config.BaudRate);
            comPortListener.DataReceived += GetData;

            Task.Run(() => comPortListener.StartListeningAsync());
        }
    }

    private void GetData(string data) {
        if (string.IsNullOrEmpty(data)) {
            throw new ArgumentException("Данные не могут быть пустыми или null.", nameof(data));
        }
        
        string[] appData = data.Split('|');
        
        int count = appData.Length - appControllers.Count;
        if (count > 0) {
            appControllers.AddRange(Enumerable.Repeat(new AppController("", FunctionType.None), count));
            configManager.SaveConfig(config);
        }
        
        for (int i = 0; i < appControllers.Count; i++) {
            if (!float.TryParse(appData[i], NumberStyles.Float, CultureInfo.InvariantCulture, out float signalLevel)) {
                throw new FormatException($"Неверный формат данных: {appData[i]}");
            }

            appControllers[i].SignalLevel = signalLevel;
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
