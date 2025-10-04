using System;
using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PanAndZoom;

namespace Sketch2Dui.Controls;

public class Canvas2D : Canvas
{
    private bool _isDragging;
    private Point _lastPoint;
    private Control? _dragTarget;
    private ZoomBorder? _zoomBorder;

    public Canvas2D()
    {
        this.Children.CollectionChanged += OnChildrenChanged;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        _zoomBorder = this.GetParent<ZoomBorder>();
    }

    private void OnChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (Control newItem in e.NewItems)
            {
                newItem.PointerPressed += OnPointerPressed;
                newItem.PointerReleased += NewItemOnPointerReleased;
            }
        }
    }

    private void NewItemOnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null);

            if (_dragTarget != null)
            {
                // Устанавливаем ZIndex для _dragTarget
                _dragTarget.SetValue(Canvas.ZIndexProperty, 0); // Поставим ZIndex на 0 после отпускания
                _dragTarget = null;
            }
        }

    }

    private Point GetContentPosition(PointerEventArgs e)
    {
        var p = e.GetPosition(_zoomBorder);

        var zx = Math.Abs(_zoomBorder?.ZoomX ?? 1.0) < double.Epsilon ? 1.0 : _zoomBorder.ZoomX;
        var zy = Math.Abs(_zoomBorder?.ZoomY ?? 1.0) < double.Epsilon ? 1.0 : _zoomBorder.ZoomY;

        var cx = (p.X - _zoomBorder.OffsetX) / zx;
        var cy = (p.Y - _zoomBorder.OffsetY) / zy;

        cx = Math.Round(cx / 5) * 5;
        cy = Math.Round(cy / 5) * 5;
        return new Point(cx, cy);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (_isDragging && _dragTarget != null && e.Pointer.Captured == _dragTarget)
        {
            var current = GetContentPosition(e);
            var delta = current - _lastPoint;

            Canvas.SetLeft(_dragTarget, Canvas.GetLeft(_dragTarget) + delta.X);
            Canvas.SetTop(_dragTarget, Canvas.GetTop(_dragTarget) + delta.Y);

            _lastPoint = current;
        }

        base.OnPointerMoved(e);
    }

 
    private void OnPressed(Control? control)
    {
        _dragTarget = control;
        _dragTarget?.SetValue(Canvas.ZIndexProperty, 1); // Поднимаем элемент выше всех
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control)
        {
            _isDragging = true;
            OnPressed(control);
            _lastPoint = GetContentPosition(e);

            e.Pointer.Capture(control);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        Debug.WriteLine($"Pointer pressed on: {e.Source}");
        base.OnPointerPressed(e);
    }
}

// Вспомогательный метод для получения родителя
public static class ControlExtensions
{
    public static T? GetParent<T>(this Control control) where T : Control
    {
        var parent = control.Parent;
        while (parent != null)
        {
            if (parent is T typedParent)
            {
                return typedParent;
            }
            parent = parent.Parent;
        }
        return null; // Если не найден родитель нужного типа
    }
    
}
