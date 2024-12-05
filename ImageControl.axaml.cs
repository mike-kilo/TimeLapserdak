using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TimeLapserdak.ViewModels;

namespace TimeLapserdak;

public partial class ImageControl : UserControl
{
    public ImageControl()
    {
        InitializeComponent();
        this.DataContext = new ImageControlViewModel();
    }
}