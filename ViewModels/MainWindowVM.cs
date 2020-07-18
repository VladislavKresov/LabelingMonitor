using LabelingMonitor.Models;
using LabelingMonitor.Models.Input_data;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        public int CountOfImages;
        public int CroppingMode;

        // Binding the main image
        private BitmapImage _MainImageSource;
        public BitmapImage MainImageSource
        {
            get { return _MainImageSource; }
            set { SetProperty(ref _MainImageSource, value); }
        }

        // Binding the changed image 1
        private BitmapImage _ChangedImage1Source;
        public BitmapImage ChangedImage1Source
        {
            get { return _ChangedImage1Source; }
            set { SetProperty(ref _ChangedImage1Source, value); }
        }

        // Binding the changed image 2
        private BitmapImage _ChangedImage2Source;
        public BitmapImage ChangedImage2Source
        {
            get { return _ChangedImage2Source; }
            set { SetProperty(ref _ChangedImage2Source, value); }
        }

        // Binding the path to current image
        private string _PathToCurrentImage;
        public string PathToCurrentImage
        {
            get { return _PathToCurrentImage; }
            set { SetProperty(ref _PathToCurrentImage, value); }
        }

        // Binding the number of current image
        private int _NumberOfCurrentImage;
        public int NumberOfCurrentImage
        {
            get { return _NumberOfCurrentImage; }
            set { SetProperty(ref _NumberOfCurrentImage, value); }
        }

        // Binding the items of comboBox
        private List<string> _ComboBoxItems;
        public List<string> ComboBoxItems
        {
            get { return _ComboBoxItems; }
            set { SetProperty(ref _ComboBoxItems, value); }
        }

        public MainWindowVM()
        {
            InitializeUserData();
            InitializeVariables();            
            UpdateImageInfo();
        }

        private void InitializeUserData()
        {            
            UserData.PathesToImages = new List<string>();
            UserData.PathesToCsvFiles = new List<string>();
            UserData.ColorMarkers = new List<char>();
            UserData.PathesToImages.Add(@"D:\Projects\DotNet\PhotoMarker\Cows\100MEDIA\7.jpg");            
            UserData.PathesToCsvFiles.Add(@"D:\Projects\DotNet\PhotoMarker\Cows\100MEDIA\masks\7.csv");
            UserData.PathesToImages.Add(@"D:\Projects\DotNet\PhotoMarker\Cows\100MEDIA\8.jpg");            
            UserData.PathesToCsvFiles.Add(@"D:\Projects\DotNet\PhotoMarker\Cows\100MEDIA\masks\8.csv");
            UserData.PathesToImages.Add(@"D:\Projects\DotNet\PhotoMarker\Cows\100MEDIA\9.jpg");            
            UserData.PathesToCsvFiles.Add(@"D:\Projects\DotNet\PhotoMarker\Cows\100MEDIA\masks\9.csv");
            UserData.BackgroundColor = Color.FromArgb(128, 128, 128);
            UserData.ColorMarkers.Add('1');
            UserData.ColorMarkers.Add('2');
        }

        private void InitializeVariables()
        {
            CountOfImages = UserData.PathesToImages.Count();
            NumberOfCurrentImage = 1;
            CroppingMode = 0;
        }

        private void UpdateImageInfo()
        {          
            var images = ImageCollection.GetCroppedImagesCollection(NumberOfCurrentImage - 1, CroppingMode);
            MainImageSource = images[0];
            ChangedImage1Source = images[1];
            ChangedImage2Source = images[2];
            PathToCurrentImage = UserData.GetPathToImage(NumberOfCurrentImage - 1);
            GC.Collect();
        }

        public void OnButtonPreviousClick()
        {
            NumberOfCurrentImage = clamp(--NumberOfCurrentImage, 1, CountOfImages);
            UpdateImageInfo();
        }

        public void OnButtonNextClick()
        {
            NumberOfCurrentImage = clamp(++NumberOfCurrentImage, 1, CountOfImages);
            UpdateImageInfo();
        }

        public void GoTo(int number)
        {
            NumberOfCurrentImage = clamp(number, 1, CountOfImages);
            UpdateImageInfo();
        }

        public void ChangeMode(int mode)
        {
            CroppingMode = clamp(mode, 0, 1);
            UpdateImageInfo();
        }

        private int clamp(int value, int minVal, int maxVal)
        {
            if (value < minVal)
                return minVal;
            if (value > maxVal)
                return maxVal;
            return value;
        }
    }
}
