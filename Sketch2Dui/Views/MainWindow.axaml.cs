using Avalonia;
using Avalonia.Controls;

namespace Sketch2Dui.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
#if DEBUG
        this.AttachDevTools();
#endif
    }
}