using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.IO;
using System.Linq;
using TimeLapserdak.ViewModels;

namespace TimeLapserdak.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public async void BrowseFolderClick(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);

            // Start async operation to open the dialog.
            var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select source folder",
                AllowMultiple = false,
            });

            var dc = (MainWindowViewModel)this.DataContext;
            if (dc is null) return;

            if (folder.Count > 0)
            {
                dc.ImagesFolder = folder[0].Path.LocalPath.ToString();
                dc.InputFilesList.Clear();
                Directory.GetFiles(dc.ImagesFolder, "*.jpg", SearchOption.TopDirectoryOnly)
                    .Select(f => new FileInfo(f))
                    .ToList()
                    .ForEach(f => dc.InputFilesList.Add(f));
            }
        }
    }
}