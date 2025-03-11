using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;

namespace OwlControlCenter;

public class MainWindowVm : INotifyPropertyChanged {
    private MainWindowModel model;
    
    public RelayCommand ApplyCommand { get; private set; } 
    
    public RelayCommand RerunCommand { get; private set; }
    
    public MainWindowVm() {
        model = new MainWindowModel();
        
        ApplyCommand = new RelayCommand(() => model.ApplyConfig());
        RerunCommand = new RelayCommand(() => model.Rerun());
    }

    public ObservableCollection<AppController> AppControllers {
        get => model.AppControllers;
        set {
            model.AppControllers = value;
            OnPropertyChanged();
        }
    }
    
    public string ComputerName {
        get => Environment.MachineName;
    }

    public string PortName {
        get => model.PortName;
        set {
            model.PortName = value;
            OnPropertyChanged();
        }
    }

    public int BaudRate {
        get => model.BaudRate;
        set {
            model.BaudRate = value;
            OnPropertyChanged();
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
