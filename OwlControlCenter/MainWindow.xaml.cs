using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Helium.Controls.Window;

namespace OwlControlCenter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : HeWindow {
    public MainWindow() {
        InitializeComponent();
        MainWindowVm vm = new MainWindowVm();
        DataContext = vm;
    }
    
    private void MainWindow_OnClosing(object? sender, CancelEventArgs e) {
        e.Cancel = true;
        Hide();
    }

    private void DataGrid_SelectionChanged(object sender, SelectedCellsChangedEventArgs selectedCellsChangedEventArgs) {
        var dataGrid = sender as DataGrid;
        dataGrid.UnselectAllCells();
    }
}
