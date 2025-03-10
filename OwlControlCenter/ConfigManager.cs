using System.IO;
using System.Text.Json;

namespace OwlControlCenter;

public class ConfigManager {
    private string configFilePath = "appsettings.json";

    public ConfigManager() {
        if (!File.Exists(configFilePath)) {
            SaveConfig([]); // Сохраняем настройки по умолчанию
        }
    }

    public void SaveConfig(AppController[] config) {
        try {
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, json);
        } catch (Exception ex) {
            MessageBox.Show($"Ошибка при сохранении конфигурации!", "Ошибка сохранения",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public AppController[]? GetConfig() {
        try {
            string json = File.ReadAllText(configFilePath);
            return JsonSerializer.Deserialize<AppController[]>(json);
        } catch (Exception ex) {
            MessageBox.Show($"Ошибка при загрузки конфигурации!", "Ошибка загрузки",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return [];
        }
    }
}
