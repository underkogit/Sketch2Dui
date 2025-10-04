using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PanAndZoom;
using Sketch2Dui.Controls;

namespace Sketch2Dui.Views;

public partial class MainView : UserControl
{
   
    private readonly ZoomBorder? _zoomBorder;
    private readonly Canvas2D? _canvas2D;
    public MainView()
    {
        this.InitializeComponent();


        _zoomBorder = this.Find<ZoomBorder>("ZoomBorder");
        if (_zoomBorder != null)
        {
            _zoomBorder.KeyDown += ZoomBorder_KeyDown;
        }
        _canvas2D = this.Find<Canvas2D>("CanvasArea");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ZoomBorder_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.F:
                _zoomBorder?.Fill();
                break;
            case Key.U:
                _zoomBorder?.Uniform();
                break;
            case Key.R:
                _zoomBorder?.ResetMatrix();
                break;
            case Key.T:
                _zoomBorder?.ToggleStretchMode();
                _zoomBorder?.AutoFit();
                break;
        }
    }

   
  

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var button = new Button();
        button.Content = "Button";
        _canvas2D?.Children.Add(button);
        
        Canvas.SetLeft(button, 10);
        Canvas.SetTop(button, 10);
    }
}