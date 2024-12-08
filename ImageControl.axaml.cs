using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using System;

namespace TimeLapserdak;

public partial class ImageControl : UserControl
{
    public int OriginX
    {
        get => (int)GetValue(OriginXProperty);
        set => SetValue(OriginXProperty, value);
    }

    public static readonly StyledProperty<int> OriginXProperty =
        AvaloniaProperty.Register<ImageControl, int>(nameof(OriginX), defaultValue: 10, defaultBindingMode: BindingMode.TwoWay);

    public int OriginY
    {
        get => (int)GetValue(OriginYProperty);
        set => SetValue(OriginYProperty, value);
    }

    public static readonly StyledProperty<int> OriginYProperty =
        AvaloniaProperty.Register<ImageControl, int>(nameof(OriginY), defaultValue: 10, defaultBindingMode: BindingMode.TwoWay);

    public int CropWidth
    {
        get => (int)GetValue(CropWidthProperty);
        set => SetValue(CropWidthProperty, value);
    }

    public static readonly StyledProperty<int> CropWidthProperty =
        AvaloniaProperty.Register<ImageControl, int>(nameof(CropWidth), defaultValue: 160, defaultBindingMode: BindingMode.TwoWay);

    public int CropHeight
    {
        get => (int)GetValue(CropHeightProperty);
        set => SetValue(CropHeightProperty, value);
    }

    public static readonly StyledProperty<int> CropHeightProperty =
        AvaloniaProperty.Register<ImageControl, int>(nameof(CropHeight), defaultValue: 90, defaultBindingMode: BindingMode.TwoWay, coerce: CropDimesionCoerce);

    public Bitmap? ImageSource
    {
        get => (Bitmap?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly StyledProperty<Bitmap?> ImageSourceProperty = 
        AvaloniaProperty.Register<ImageControl, Bitmap?>(nameof(ImageSource), defaultValue: null, defaultBindingMode: BindingMode.OneWay);

    public ImageControl()
    {
        InitializeComponent();
        this.DataContext = this;
    }

    public static int CropDimesionCoerce(AvaloniaObject o, int value)
    {
        (o as ImageControl).CropWidth = (int)Math.Round(value * 16.0 / 9.0);
        return value;
    }

    public void MouseHoverOverImage(object sender, PointerEventArgs args)
    {
        var point = args.GetCurrentPoint(sender as Control);
        var intrPoints = args.GetIntermediatePoints(sender as Control);   
        var pos = point.Position;

        if (0 <= pos.X - this.OriginX && pos.X - this.OriginX <= this.CropWidth)
        {
            if (Math.Abs(pos.Y - this.OriginY - this.CropHeight) <= 10)
            {
                this.Cursor = new Cursor(StandardCursorType.SizeNorthSouth);
                if (point.Properties.IsLeftButtonPressed)
                {
                    var pdiff = intrPoints[^1].Position - intrPoints[0].Position;
                    this.CropHeight += (int)Math.Ceiling(pdiff.Y);
                }

                return;
            }

            if (0 <= pos.Y - this.OriginY && pos.Y - this.OriginY <= this.CropHeight)
            {
                this.Cursor = new Cursor(StandardCursorType.SizeAll);
                if (point.Properties.IsLeftButtonPressed) 
                {
                    var pdiff = intrPoints[^1].Position - intrPoints[0].Position;
                    this.OriginX += (int)Math.Round(pdiff.X);
                    this.OriginY += (int)Math.Round(pdiff.Y);
                }
                
                return;
            }
        }

        Cursor = Cursor.Default;
    }

    public void MouseExitedOverImage(object sender, PointerEventArgs args)
    { 
        Cursor = Cursor.Default;
    }
}