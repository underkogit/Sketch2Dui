using CommunityToolkit.Mvvm.ComponentModel;

namespace Sketch2Dui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";
}