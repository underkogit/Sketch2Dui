using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using PanAndZoom;

namespace Sketch2Dui.Views;

public partial class MainView : UserControl
{
    private bool _isDragging;
    private Point _lastPoint;
    private Control? _dragTarget;
    private readonly ZoomBorder? _zoomBorder;

    public MainView()
    {
        this.InitializeComponent();


        _zoomBorder = this.Find<ZoomBorder>("ZoomBorder");
        if (_zoomBorder != null)
        {
            _zoomBorder.KeyDown += ZoomBorder_KeyDown;
        }
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

    private Point GetContentPosition(PointerEventArgs e)
    {
        var p = e.GetPosition(_zoomBorder);

        var zx = Math.Abs(_zoomBorder.ZoomX) < double.Epsilon ? 1.0 : _zoomBorder.ZoomX;
        var zy = Math.Abs(_zoomBorder.ZoomY) < double.Epsilon ? 1.0 : _zoomBorder.ZoomY;

        var cx = (p.X - _zoomBorder.OffsetX) / zx;
        var cy = (p.Y - _zoomBorder.OffsetY) / zy;


        cx = (cx / 5) * 5;
        cy = (cy / 5) * 5;
        return new Point(cx, cy);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control)
        {
            _isDragging = true;
            _dragTarget = control;
            _lastPoint = GetContentPosition(e);  

            
            e.Pointer.Capture(control);
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging && _dragTarget != null && e.Pointer.Captured == _dragTarget)
        {
            var current = GetContentPosition(e);  
            var delta = current - _lastPoint;

            double left = Canvas.GetLeft(_dragTarget);
            double top = Canvas.GetTop(_dragTarget);

            Canvas.SetLeft(_dragTarget, left + delta.X);
            Canvas.SetTop(_dragTarget, top + delta.Y);

            _lastPoint = current;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;

            // Снимаем захват указателя
            e.Pointer.Capture(null);
            _dragTarget = null;
        }
    }
}