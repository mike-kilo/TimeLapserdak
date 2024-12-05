using Avalonia;
using System;
using System.ComponentModel;

namespace TimeLapserdak.ViewModels
{
    public partial class ImageControlViewModel : ViewModelBase, INotifyPropertyChanged
    {
		private int _originX = 10;

		public int OriginX
		{
			get => _originX;
			set 
			{ 
				_originX = value; 
				OnPropertyChanged(nameof(OriginX));
            }
        }

		private int _originY = 10;

		public int OriginY
		{
			get => _originY;
			set 
			{ 
				_originY = value; 
				OnPropertyChanged(nameof(OriginY));
            }
        }

		private int _cropWidth = 160;

		public int CropWidth
		{
			get => _cropWidth;
			private set 
			{ 
				_cropWidth = value; 
				OnPropertyChanged(nameof(CropWidth));
            }
        }

		private int _cropHeight = 90;

		public int CropHeight
		{
			get => _cropHeight;
			set 
			{ 
				_cropHeight = value; 
				OnPropertyChanged(nameof(CropHeight));
				this.CropWidth = (int)Math.Round(this.CropHeight * 16.0 / 9.0, MidpointRounding.AwayFromZero);
            }
        }


	}
}
