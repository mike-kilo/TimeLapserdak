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

            if (folder.Count > 0)
            {
                ((MainWindowViewModel)this.DataContext).ImagesFolder = folder[0].Path.LocalPath.ToString();
                ((MainWindowViewModel)this.DataContext).InputFilesList.Clear();
                Directory.GetFiles(((MainWindowViewModel)this.DataContext).ImagesFolder, "*.jpg", SearchOption.TopDirectoryOnly)
                    .Select(f => new FileInfo(f))
                    .ToList()
                    .ForEach(f => ((MainWindowViewModel)this.DataContext).InputFilesList.Add(f));
            }
        }
    }
}