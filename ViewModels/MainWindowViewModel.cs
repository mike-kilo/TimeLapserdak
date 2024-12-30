using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace TimeLapserdak.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler? PropertyChanged;

        //protected virtual void OnPropertyChanged([CallerMemberName] string?  propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public static List<double> Framerates
        {
            get => [1.0, 2.0, 5.0, 25.0, 30.0, 50.0, 60.0, 100.0, 120.0];
        }

        public enum Orientation
        {
            Landscape,
            Square,
            Portrait,
        }

        public string Greeting { get; } = "Welcome to Avalonia!";

        private string _imagesFolder = string.Empty;
        public string ImagesFolder
        {
            get => _imagesFolder;
            set
            {
                _imagesFolder = value;
                OnPropertyChanged(nameof(ImagesFolder));
            }
        }

        private ObservableCollection<FileInfo> _inputFilesList = [];

        public ObservableCollection<FileInfo> InputFilesList
        {
            get => this._inputFilesList;
            set
            {
                this._inputFilesList = value;
                OnPropertyChanged(nameof(InputFilesList));
            }
        }

        private Bitmap? _startingImageBinding;

        public Bitmap? StartingImageBinding
        {
            get => this._startingImageBinding;
            set
            {
                this._startingImageBinding = value;
                OnPropertyChanged(nameof(StartingImageBinding));
            }
        }

        private Bitmap? _endingImageBinding;

        public Bitmap? EndingImageBinding
        {
            get => this._endingImageBinding;
            set
            {
                this._endingImageBinding = value;
                OnPropertyChanged(nameof(EndingImageBinding));
            }
        }

        private int _picturesProgress = 0;

        public int PicturesProgress
        {
            get => this._picturesProgress;
            set
            {
                this._picturesProgress = value;
                OnPropertyChanged(nameof(PicturesProgress));
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => this._isBusy;
            set
            {
                this._isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        private bool _isVideoConverting;

        public bool IsVideoConverting
        {
            get => _isVideoConverting;
            set
            {
                this._isVideoConverting = value;
                OnPropertyChanged(nameof(IsVideoConverting));
            }
        }

        private double _videoProgress = 0;

        public double VideoProgress
        {
            get => this._videoProgress;
            set
            {
                this._videoProgress = value;
                OnPropertyChanged(nameof(VideoProgress));
            }
        }

        public static bool IsFFMpegAvailable
        {
            get => ImageProcessing.IsFFMpegAvailable();
        }

        private string _errorMessage = string.Empty;

        public string ErrorMessage
        {
            get => this._errorMessage;
            set
            {
                this._errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public static string VersionNumber
        {
            get => Assembly.GetExecutingAssembly()?.GetName().Version?.ToString() ?? "Unknown";
        }

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
