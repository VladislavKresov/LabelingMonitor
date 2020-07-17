using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LabelingMonitor.ViewModels
{
    class MainWindowVM : BindableBase
    {
        // Binding the main image
        private BitmapImage mainImgSource;
        public BitmapImage MainImageSource
        {
            get { return mainImgSource; }
            set { SetProperty(ref mainImgSource, value); }
        }

        // Binding the changed image 1
        private BitmapImage changedImg1Source;
        public BitmapImage ChangedImage1Source
        {
            get { return changedImg1Source; }
            set { SetProperty(ref changedImg1Source, value); }
        }

        // Binding the changed image 2
        private BitmapImage changedImg2Source;
        public BitmapImage ChangedImage2Source
        {
            get { return changedImg2Source; }
            set { SetProperty(ref changedImg2Source, value); }
        }

        // Binding the path to current image
        private string currentPath;
        public string PathToCurrentImage
        {
            get { return currentPath; }
            set { SetProperty(ref currentPath, value); }
        }

        // Binding the number of current image
        private string _NumberOfCurrentImage;
        public string NumberOfCurrentImage
        {
            get { return _NumberOfCurrentImage; }
            set { SetProperty(ref _NumberOfCurrentImage, value); }
        }

        public MainWindowVM()
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(@"D:\Projects\DotNet\PhotoMarker\Cows\100MEDIA\1.jpg"));
            MainImageSource = bitmapImage;
            ChangedImage1Source = bitmapImage;
            ChangedImage2Source = bitmapImage;
            PathToCurrentImage = "Some path to image";
            NumberOfCurrentImage = "Some number";
        }

    }
}
