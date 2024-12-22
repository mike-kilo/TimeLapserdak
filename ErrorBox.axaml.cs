using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TimeLapserdak;

public partial class ErrorBox : UserControl
{
    public string ErrorInfo
    {
        get { return GetValue(ErrorInfoProperty); }
        set { SetValue(ErrorInfoProperty, value); }
    }

    public static readonly StyledProperty<string> ErrorInfoProperty =
        AvaloniaProperty.Register<ErrorBox, string>(nameof(ErrorInfo), defaultValue: string.Empty, defaultBindingMode: Avalonia.Data.BindingMode.OneWay);

    public ErrorBox()
    {
        InitializeComponent();
    }

    public void CloseButtonClick(object sender, RoutedEventArgs e)
    {
        this.ErrorInfo = string.Empty;
    }
}