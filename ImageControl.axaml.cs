using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeLapserdak;

public partial class ImageControl : UserControl
{
    private enum MouseMoveAction
    {
        None,
        Resize,
        Move,
    }

    private static readonly List<ImageControl> _instances = [];

    #region Properties

    public static double ImageAspectRatio => 16.0 / 9.0;

    private MouseMoveAction _currentMouseAction = MouseMoveAction.None;
    private PointerPoint _initialClickPoint = new();
    private Point _onClickParam = new();

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
        AvaloniaProperty.Register<ImageControl, double>(nameof(CropWidth), defaultValue: 160, defaultBindingMode: BindingMode.TwoWay, coerce: CropWidthCoerce);

    public double CropHeight
    {
        get => (double)GetValue(CropHeightProperty);
        set => SetValue(CropHeightProperty, value);
    }

    public static readonly StyledProperty<double> CropHeightProperty =
        AvaloniaProperty.Register<ImageControl, double>(nameof(CropHeight), defaultValue: 90, defaultBindingMode: BindingMode.TwoWay, coerce: CropHeightCoerce);

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
            if (this.ImageSource is not Bitmap b || this.TheImageControl is not Image i) return null;
            var scale = b.Size / i.Bounds.Size;

            return new PixelRect(
                new PixelPoint((int)Math.Round(scale.X * this.OriginX), (int)Math.Round(scale.Y * this.OriginY)), 
                new PixelSize((int)Math.Floor(scale.Y * this.CropHeight * ImageAspectRatio), (int)Math.Floor(scale.Y * this.CropHeight)));
        }
    }

    public static readonly StyledProperty<PixelRect?> CroppingBoxProperty =
        AvaloniaProperty.Register<ImageControl, PixelRect?>(nameof(CroppingBox), defaultValue: null, defaultBindingMode: BindingMode.OneWay);

    public bool IsMain
    {
        get { return (bool)GetValue(IsMainProperty); }
        set { SetValue(IsMainProperty, value); }
    }

    public static readonly StyledProperty<bool> IsMainProperty =
        AvaloniaProperty.Register<ImageControl, bool>(nameof(IsMain), defaultValue: false);

    #endregion Properties

    public Image? TheImageControl { get; private set; }

    #region Constructors

    public ImageControl()
    {
        InitializeComponent();
        this.TheImageControl = this.GetControl<Image>(nameof(TheImage));
    }

    ~ImageControl() 
    { 
        _instances.Remove(this);
    }

    #endregion Constructors

    #region Coerce methods

    public static double OriginXCoerce(AvaloniaObject o, double value) =>
        (o is ImageControl ic) ? 
        Math.Min(Math.Max(0, value), (ic.TheImageControl?.DesiredSize.Width ?? 0) - ic.CropWidth) : 
        value;

    public static double OriginYCoerce(AvaloniaObject o, double value) => 
        (o is ImageControl ic) ? 
        Math.Min(Math.Max(0, value), (ic.TheImageControl?.DesiredSize.Height ?? 0) - ic.CropHeight) : 
        value;

    public static double CropWidthCoerce(AvaloniaObject o, double value) =>
        (o is ImageControl ic) ?
        Math.Min(Math.Max(0, value), (ic.TheImageControl?.DesiredSize.Width ?? 0) - ic.OriginX) :
        value;

    public static double CropHeightCoerce(AvaloniaObject o, double value)
    {
        if (o is not ImageControl ic) return value;
        var height = Math.Min(Math.Max(0, value), Math.Min((ic.TheImageControl?.DesiredSize.Height ?? 0) - ic.OriginY, ((ic.TheImageControl?.DesiredSize.Width ?? 0) - ic.OriginX) / ImageAspectRatio));
        ic.CropWidth = height * ImageAspectRatio;
        return height;
    }

    #endregion Coerce methods

    #region Event handlers

    public void ControlLoaded(object sender, RoutedEventArgs args)
    {
        this.IsMain = this.IsMain && !_instances.Any(i => i.IsMain);
        _instances.Add(this);
    }

    public void MouseHoverOverImage(object sender, PointerEventArgs args)
    {
        var point = args.GetCurrentPoint(sender as Control);
        var pos = point.Position;

        Cursor = Cursor.Default;
        if (0 <= pos.X - this.OriginX && pos.X - this.OriginX <= this.CropWidth)
        {
            if (Math.Abs(pos.Y - this.OriginY - this.CropHeight) <= 10) 
                this.Cursor = new Cursor(StandardCursorType.SizeNorthSouth);
            else if 
                (0 <= pos.Y - this.OriginY && pos.Y - this.OriginY <= this.CropHeight) this.Cursor = new Cursor(StandardCursorType.SizeAll);
        }

        if (this._currentMouseAction == MouseMoveAction.Move)
        {
            var pdiff = point.Position - this._initialClickPoint.Position;
            this.OriginX = this._onClickParam.X + pdiff.X;
            this.OriginY = this._onClickParam.Y + pdiff.Y;
        }

        if (this._currentMouseAction == MouseMoveAction.Resize)
        {
            var pdiff = point.Position - this._initialClickPoint.Position;
            this.CropHeight = this._onClickParam.Y + pdiff.Y;
        }
    }

    public void MouseExitedOverImage(object sender, PointerEventArgs args)
    { 
        Cursor = Cursor.Default;
        this._currentMouseAction = MouseMoveAction.None;
    }

    public void MouseButtonPressedOverImage(object sender, PointerPressedEventArgs args)
    {
        this._initialClickPoint = args.GetCurrentPoint(sender as Control);
        if (this._initialClickPoint.Properties.IsLeftButtonPressed)
            this._currentMouseAction = this.Cursor?.ToString() switch
            {
                "SizeAll" => MouseMoveAction.Move,
                "SizeNorthSouth" => MouseMoveAction.Resize,
                _ => MouseMoveAction.None,
            };
        if (this._currentMouseAction == MouseMoveAction.Move) this._onClickParam = new Point(this.OriginX, this.OriginY);
        if (this._currentMouseAction == MouseMoveAction.Resize) this._onClickParam = new Point(0, this.CropHeight);
    }

    public void MouseButtonReleasedOverImage(object sender, PointerReleasedEventArgs args)
    {
        this._currentMouseAction = MouseMoveAction.None;
    }

    public void ImageSizeChanged(object sender, SizeChangedEventArgs args)
    {
        this.OriginX = this.OriginX;
        this.OriginY = this.OriginY;
    }

    #endregion Event handlers
}