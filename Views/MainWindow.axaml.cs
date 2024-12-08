using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using System.IO;
using System.Linq;
using TimeLapserdak.ViewModels;

namespace TimeLapserdak.Views
{
    public partial class MainWindow : Window
    {
        private readonly ImageControl _startingImageControl;
        private readonly ImageControl _endingImageControl;

        public MainWindow()
        {
            InitializeComponent();

            this._startingImageControl = this.FindControl<ImageControl>("StartingImage") ?? new ImageControl();
            this._endingImageControl = this.FindControl<ImageControl>("EndingImage") ?? new ImageControl();
        }

        public async void BrowseFolderClick(object sender, RoutedEventArgs e)
        {
            if (TopLevel.GetTopLevel(this) is not TopLevel topLevel) return;

            // Start async operation to open the dialog.
            var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select source folder",
                AllowMultiple = false,
            });

            if (this.DataContext is not MainWindowViewModel dc) return;

            dc.StartingImageBinding = null;
            dc.EndingImageBinding = null;

            if (folder.Count > 0)
            {
                dc.ImagesFolder = folder[0].Path.LocalPath.ToString();
                dc.InputFilesList.Clear();
                Directory.GetFiles(dc.ImagesFolder, "*.jpg", SearchOption.TopDirectoryOnly)
                    .Select(f => new FileInfo(f))
                    .ToList()
                    .ForEach(f => dc.InputFilesList.Add(f));

                if (dc.InputFilesList.Count == 0) return;

                dc.StartingImageBinding = new Bitmap(dc.InputFilesList.First().FullName);
                dc.EndingImageBinding = new Bitmap(dc.InputFilesList.Last().FullName);
            }

            this._startingImageControl.ImageSource = dc.StartingImageBinding;
            this._endingImageControl.ImageSource = dc.EndingImageBinding;
        }
    }
}