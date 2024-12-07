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

        private ObservableCollection<FileInfo> _inputFilesList = new();

        public ObservableCollection<FileInfo> InputFilesList
        {
            get => this._inputFilesList;
            set 
            { 
                this._inputFilesList = value; 
                OnPropertyChanged(nameof(InputFilesList));
            }
        }

    }
}
