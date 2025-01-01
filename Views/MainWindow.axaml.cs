using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using FFMpegCore.Extensions.SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            dc.PicturesProgress = 0;
            dc.VideoProgress = 0;

            if (folder.Count > 0)
            {
                dc.ImagesFolder = folder[0].Path.LocalPath.ToString();
                dc.InputFilesList.Clear();
                Directory.GetFiles(dc.ImagesFolder, "*.jpg", SearchOption.TopDirectoryOnly)?
                    .OrderBy(f => f)
                    .Select(f => new FileInfo(f))?
                    .ToList()
                    .ForEach(f => dc.InputFilesList.Add(f));

                if (dc.InputFilesList.Count == 0) return;

                dc.StartingImageBinding = new Bitmap(dc.InputFilesList.First().FullName);
                dc.EndingImageBinding = new Bitmap(dc.InputFilesList.Last().FullName);
            }
        }

        public async void GenerateClick(object sender, RoutedEventArgs e)
        {
            var startingCrop = this._startingImageControl.CroppingBox ?? new PixelRect();
            var endingCrop = this._endingImageControl.CroppingBox ?? new PixelRect();

            if (this.DataContext is not MainWindowViewModel dc) return;
            dc.PicturesProgress = 0;
            dc.VideoProgress = 0;

            dc.IsBusy = true;
            dc.IsVideoConverting = true;

            var picsCount = dc.InputFilesList.Count;
            var positionStep = (endingCrop.Position - startingCrop.Position).ToPoint(picsCount);
            var sizeStep = new PixelSize(endingCrop.Width - startingCrop.Width, endingCrop.Height - startingCrop.Height).ToSize(picsCount);
            var cropOrientation = ImageControl.CroppingBoxOrientation;
            List<PixelRect> crops = Enumerable.Range(0, picsCount)
                .Select(n => new PixelRect(
                    startingCrop.Position + PixelPoint.FromPoint(positionStep, n),
                    PixelSize.FromSize(new Size(
                        (startingCrop.Height + sizeStep.Height * n) * cropOrientation.ToAspectRatio(),
                        startingCrop.Height + sizeStep.Height * n), 1.0)))
                .ToList();

            var tempFolder = string.Empty;
            try
            {
                tempFolder = Path.Combine(dc.ImagesFolder ?? Path.GetTempPath(), DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                Directory.CreateDirectory(tempFolder);
            }
            catch (Exception ex)
            { 
                dc.ErrorMessage = $"Encountered an error when creating temporary output folder \"{tempFolder}\"" + Environment.NewLine + ex.Message;
            }

            await Task.Run(() =>
            {
                Parallel.ForEach(dc.InputFilesList.Zip(crops, (f, c) => new { File = f, Crop = c }).ToList(), (i) =>
                {
                    if (ImageProcessing.CropAndResizePicture(i.File, i.Crop, tempFolder, cropOrientation))
                        Dispatcher.UIThread.Invoke(() => dc.PicturesProgress++);
                });
            });

            if (dc.PicturesProgress < picsCount)
                dc.ErrorMessage = @"Some pictures were not converted.
Apparently there are some issues 
with storage (hard drive) access,
insufficient space or missing folder.";

            if (ImageProcessing.IsFFMpegAvailable() && 
                (Directory.GetFiles(tempFolder, "*.jpg", SearchOption.TopDirectoryOnly).Select(f => new FileInfo(f)).ToList() is List<FileInfo> tmpFiles))
            {
                ImageProcessing.ProgressChangedEvent += this.VideoGenerateProgressChanged;
                var pipeSource = await ImageProcessing.GenerateVideoPipeSource(ImageProcessing.CreateFrames(tmpFiles), dc.SelectedFramerate);
                if (pipeSource is null) dc.ErrorMessage = "Encountered a problem in generating source from frames";
                if (pipeSource is not null)
                {
                    var convertedMessage = await ImageProcessing.GenerateVideo(pipeSource, Path.GetDirectoryName(dc.InputFilesList[0].FullName) ?? Path.GetTempPath(), tmpFiles.Count);
                    ImageProcessing.ProgressChangedEvent -= this.VideoGenerateProgressChanged;
                    if (string.IsNullOrEmpty(convertedMessage)) Directory.Delete(tempFolder, true);
                    if (convertedMessage.Length > 0) dc.ErrorMessage = convertedMessage;
                    dc.VideoProgress = 100;
                }
            }
            
            dc.IsVideoConverting = false;
            dc.IsBusy = false;
        }

        public void VideoGenerateProgressChanged(object? sender, double d)
        {
            Dispatcher.UIThread.Invoke(() => 
            { 
                if (this.DataContext is not MainWindowViewModel dc) return;
                dc.IsVideoConverting = false;
                dc.VideoProgress = d;
            });
        }

        public void DownloadFFMpegClick(object sender, RoutedEventArgs e)
        {
            var launcher = TopLevel.GetTopLevel(sender as Control)?.Launcher;
            launcher?.LaunchUriAsync(new Uri("https://www.ffmpeg.org/"));
        }

        public void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is not MainWindowViewModel dc) return;
            dc.ErrorMessage = ImageProcessing.IsFFMpegAvailable() ? string.Empty : "FFMpeg not found on your system";
        }
    }
}