namespace OwlControlCenter;

public class MainWindowModel {
    private ConfigManager configManager;
    private AppController[] appControllers;
    
    public MainWindowModel() {
        configManager = new ConfigManager();
        appControllers = configManager.GetConfig();
    }
}
