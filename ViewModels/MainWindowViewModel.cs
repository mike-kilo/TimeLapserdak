using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using TimeLapserdak.Helpers;
using static TimeLapserdak.Helpers.Enums;

namespace TimeLapserdak.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public static List<double> Framerates { get; } = [1.0, 2.0, 5.0, 25.0, 30.0, 50.0, 60.0, 100.0, 120.0];

        public static bool IsFFMpegAvailable { get; } = ImageProcessing.IsFFMpegAvailable();

        public static string VersionNumber { get; } = Assembly.GetExecutingAssembly()?.GetName().Version?.ToString() ?? "Unknown";

        [ObservableProperty]
        private string imagesFolder = string.Empty;

        [ObservableProperty]
        private ObservableCollection<FileInfo> inputFilesList = [];

        [ObservableProperty]
        private Bitmap? startingImageBinding;

        [ObservableProperty]
        private Bitmap? endingImageBinding;

        [ObservableProperty]
        private int picturesProgress = 0;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isVideoConverting;

        [ObservableProperty]
        private double videoProgress = 0;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private double selectedFramerate = Framerates[3];

        [ObservableProperty]
        private bool isCropPositionLocked = false;

        [ObservableProperty]
        private bool isCropSizeLocked = false;

        [ObservableProperty]
        private List<Orientation> orientations = new(Enum.GetValues<Orientation>());

        [ObservableProperty]
        private Orientation cropOrientation = Orientation.Landscape;
    }
}
