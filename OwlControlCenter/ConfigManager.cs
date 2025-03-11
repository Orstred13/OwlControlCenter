using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace OwlControlCenter;

public class ConfigManager {
    private string configFilePath = "appsettings.json";

    public ConfigManager() {
        if (!File.Exists(configFilePath)) {
            SaveConfig(new Config()); // Сохраняем настройки по умолчанию
        }
    }

    public void SaveConfig(Config config) {
        try {
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, json);
        } catch (Exception ex) {
            MessageBox.Show($"Ошибка при сохранении конфигурации!", "Ошибка сохранения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public Config GetConfig() {
        try {
            string json = File.ReadAllText(configFilePath);
            return JsonSerializer.Deserialize<Config>(json);
        } catch (Exception ex) {
            MessageBox.Show($"Ошибка при загрузки конфигурации!", "Ошибка загрузки",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new Config();
        }
    }
}

public class Config {
    public ObservableCollection<AppController> AppControllers { get; set; }
    public string PortName { get; set; } = "COM7";
    public int BaudRate { get; set; } = 9600;
}
