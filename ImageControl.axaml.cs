using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using System;

namespace TimeLapserdak;

public partial class ImageControl : UserControl
{
    #region Properties

    public double OriginX
    {
        get => (double)GetValue(OriginXProperty);
        set => SetValue(OriginXProperty, value);
    }

    public static readonly StyledProperty<double> OriginXProperty =
        AvaloniaProperty.Register<ImageControl, double>(nameof(OriginX), defaultValue: 10, defaultBindingMode: BindingMode.TwoWay, coerce: OriginXCoerce);

    public double OriginY
    {
        get => (double)GetValue(OriginYProperty);
        set => SetValue(OriginYProperty, value);
    }

    public static readonly StyledProperty<double> OriginYProperty =
        AvaloniaProperty.Register<ImageControl, double>(nameof(OriginY), defaultValue: 10, defaultBindingMode: BindingMode.TwoWay, coerce: OriginYCoerce);

    public double CropWidth
    {
        get => (double)GetValue(CropWidthProperty);
        set => SetValue(CropWidthProperty, value);
    }

    public static readonly StyledProperty<double> CropWidthProperty =
        AvaloniaProperty.Register<ImageControl, double>(nameof(CropWidth), defaultValue: 160, defaultBindingMode: BindingMode.TwoWay);

    public double CropHeight
    {
        get => (double)GetValue(CropHeightProperty);
        set => SetValue(CropHeightProperty, value);
    }

    public static readonly StyledProperty<double> CropHeightProperty =
        AvaloniaProperty.Register<ImageControl, double>(nameof(CropHeight), defaultValue: 90, defaultBindingMode: BindingMode.TwoWay, coerce: CropDimesionCoerce);

    public Bitmap? ImageSource
    {
        get => (Bitmap?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly StyledProperty<Bitmap?> ImageSourceProperty = 
        AvaloniaProperty.Register<ImageControl, Bitmap?>(nameof(ImageSource), defaultValue: null, defaultBindingMode: BindingMode.OneWay);

    public PixelRect? CroppingBox
    {
        get
        {
            if (this.ImageSource is not Bitmap b || this.FindControl<Image>("TheImage") is not Image i) return null;
            var scale = b.Size / i.Bounds.Size;

            return new PixelRect(
                new PixelPoint((int)Math.Round(scale.X * this.OriginX), (int)Math.Round(scale.Y * this.OriginY)), 
                new PixelSize((int)Math.Round(scale.Y * this.CropHeight / 9.0) * 16, (int)Math.Round(scale.Y * this.CropHeight / 9.0) * 9));
        }
    }

    public static readonly StyledProperty<PixelRect?> CroppingBoxProperty =
        AvaloniaProperty.Register<ImageControl, PixelRect?>(nameof(CroppingBox), defaultValue: null, defaultBindingMode: BindingMode.OneWay);

    #endregion Properties

    #region Constructors

    public ImageControl()
    {
        InitializeComponent();
        this.DataContext = this;
    }

    #endregion Constructors

    #region Coerce methods

    public static double CropDimesionCoerce(AvaloniaObject o, double value)
    {
        if (o is ImageControl ic) ic.CropWidth = value * 16.0 / 9.0;
        return value;
    }

    public static double OriginXCoerce(AvaloniaObject o, double value) =>
        (o is ImageControl ic && ic.GetControl<Image>(nameof(TheImage)) is Image im) ? 
        Math.Min(Math.Max(0, value), im.DesiredSize.Width - ic.CropWidth) : 
        value;

    public static double OriginYCoerce(AvaloniaObject o, double value) => 
        (o is ImageControl ic && ic.GetControl<Image>(nameof(TheImage)) is Image im) ? 
        Math.Min(Math.Max(0, value), im.DesiredSize.Height - ic.CropHeight) : 
        value;

    #endregion Coerce methods

    #region Event handlers

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
                    this.CropHeight += pdiff.Y;
                }

                return;
            }

            if (0 <= pos.Y - this.OriginY && pos.Y - this.OriginY <= this.CropHeight)
            {
                this.Cursor = new Cursor(StandardCursorType.SizeAll);
                if (point.Properties.IsLeftButtonPressed) 
                {
                    var pdiff = intrPoints[^1].Position - intrPoints[0].Position;
                    this.OriginX += pdiff.X;
                    this.OriginY += pdiff.Y;
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

    #endregion Event handlers
}