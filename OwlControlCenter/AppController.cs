using System.Diagnostics;
using System.Windows.Shapes;

namespace OwlControlCenter;

public class AppController {
    private Process _process;
    public string AppPath { get; }

    private string processName {
        get => System.IO.Path.GetFileName(AppPath);
    }

    public AppController(string appPath) {
        AppPath = appPath;
    }


    // Запуск приложения
    public void StartApp() {
        if (_process != null && !_process.HasExited) {
            return;
        }

        try {
            _process = Process.Start(AppPath);
        } catch (Exception ex) {
            MessageBox.Show($"Ошибка при запуске приложения {processName}", "Ошибка запуска",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void CloseApp() {
        if (_process == null || _process.HasExited) {
            return;
        }

        try {
            _process.CloseMainWindow();
            _process.Close();
        } catch (Exception ex) {
            MessageBox.Show($"Ошибка при закрытии приложения {processName}", "Ошибка запуска",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
