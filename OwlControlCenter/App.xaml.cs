using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using Application = System.Windows.Application;

namespace OwlControlCenter;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    private NotifyIcon notifyIcon;

    public void App_Startup(object sender, StartupEventArgs e) {
        var assembly = Assembly.GetExecutingAssembly();
        Icon icon;
        using (var stream = assembly.GetManifestResourceStream("OwlControlCenter.Resources.icon.ico")) {
            icon = new Icon(stream);
        }

        notifyIcon = new NotifyIcon {
            Icon = icon,
            Visible = true,
            Text = "OwlControlCenter"
        };
        
        notifyIcon.ContextMenuStrip = CreateContextMenu();
        
        Current.MainWindow = new MainWindow();
        Current.MainWindow.Hide();

        notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
    }

    private ContextMenuStrip CreateContextMenu() {
        var contextMenu = new ContextMenuStrip();
        
        var exitMenuItem = new ToolStripMenuItem("Закрыть", null, ExitMenuItem_Click);
        contextMenu.Items.Add(exitMenuItem);

        return contextMenu;
    }

    private void ExitMenuItem_Click(object sender, EventArgs e) {
        notifyIcon.Dispose();
        Current.Shutdown();
    }

    private void NotifyIcon_MouseDoubleClick(object? sender, System.Windows.Forms.MouseEventArgs e) {
        if (Current.MainWindow.Visibility != Visibility.Visible) {
            Current.MainWindow.Show();
        } else {
            Current.MainWindow.Hide();
        }
    }
}
