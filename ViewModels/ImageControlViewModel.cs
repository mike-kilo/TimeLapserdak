using Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLapserdak.ViewModels
{
    public partial class ImageControlViewModel : ViewModelBase, INotifyPropertyChanged
    {
		private Rect _imageCrop;

		public Rect ImageCrop
		{
			get => _imageCrop; 
			set 
			{ 
				_imageCrop =value; 
				OnPropertyChanged(nameof(ImageCrop));
			}
		}
	}
}
