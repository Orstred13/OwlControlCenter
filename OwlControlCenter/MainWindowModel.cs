using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace OwlControlCenter;

public class MainWindowModel : INotifyPropertyChanged {
    public ConfigManager ConfigManager;
    public Config Config { get; set; }

    private ComPortListener comPortListener;

    public string PortName {
        get => Config.PortName;
        set => Config.PortName = value;
    }

    public int BaudRate {
        get => Config.BaudRate;
        set => Config.BaudRate = value;
    }

    public List<AppController> AppControllers {
        get => Config.AppControllers;
        set => Config.AppControllers = value;
    }

    public MainWindowModel() {
        ConfigManager = new ConfigManager();
        Config = ConfigManager.GetConfig();
        if (AppControllers == null) AppControllers = new List<AppController>();
        if (!string.IsNullOrEmpty(PortName)) {
            comPortListener = new ComPortListener(PortName, BaudRate);
            comPortListener.DataReceived += GetData;

            Task.Run(() => comPortListener.StartListeningAsync());
        }
    }

    private void GetData(string data) {
        if (string.IsNullOrEmpty(data)) {
            throw new ArgumentException("Данные не могут быть пустыми или null.", nameof(data));
        }
        
        string[] appData = data.Split('|');
        
        int count = appData.Length - AppControllers.Count;
        if (count > 0) {
            AppControllers.AddRange(Enumerable.Repeat(new AppController("", FunctionType.None), count));
            ConfigManager.SaveConfig(Config);
        }
        
        for (int i = 0; i < AppControllers.Count; i++) {
            if (!float.TryParse(appData[i], NumberStyles.Float, CultureInfo.InvariantCulture, out float signalLevel)) {
                throw new FormatException($"Неверный формат данных: {appData[i]}");
            }

            if (!AppControllers[i].IsReady) AppControllers[i].IsReady = true;
            AppControllers[i].SignalLevel = signalLevel;
        }
    }

    public void ApplyConfig() {
        ConfigManager.SaveConfig(Config);
    }

    public void Rerun() {
        comPortListener.StopListening();
        comPortListener.Dispose();
        
        if (AppControllers == null) AppControllers = new List<AppController>();
        if (!string.IsNullOrEmpty(PortName)) {
            comPortListener = new ComPortListener(PortName, BaudRate);
            comPortListener.DataReceived += GetData;

            Task.Run(() => comPortListener.StartListeningAsync());
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
