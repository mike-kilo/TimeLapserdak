using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TimeLapserdak.Views;

public partial class ErrorBox : UserControl
{
    private string _errorInfo = string.Empty;

    public string ErrorInfo
    {
        get => _errorInfo;
        set => SetAndRaise(ErrorInfoProperty, ref _errorInfo, value);
    }

    public static readonly DirectProperty<ErrorBox, string> ErrorInfoProperty =
        AvaloniaProperty.RegisterDirect<ErrorBox, string>(nameof(ErrorInfo), o => o.ErrorInfo, (o, v) => o.ErrorInfo = v);

    public ErrorBox()
    {
        InitializeComponent();
    }

    public void CloseButtonClick(object sender, RoutedEventArgs e)
    {
        this.ErrorInfo = string.Empty;
    }
}