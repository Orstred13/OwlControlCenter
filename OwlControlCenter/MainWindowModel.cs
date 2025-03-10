using System.Globalization;

namespace OwlControlCenter;

public class MainWindowModel {
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
}
