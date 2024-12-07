﻿using Avalonia.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace TimeLapserdak.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler? PropertyChanged;
        
        //protected virtual void OnPropertyChanged([CallerMemberName] string?  propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

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
    }
}
