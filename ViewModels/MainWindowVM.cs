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
        // Binding the enable statement for "move back" button
        private bool _PrevBTN_Enabled;
        public bool PrevBTN_Enabled
        {
            get { return _PrevBTN_Enabled; }
            set { SetProperty(ref _PrevBTN_Enabled, value); }
        }
        // Binding the enable statement for "move forward" button
        private bool _NextBTN_Enabled;
        public bool NextBTN_Enabled
        {
            get { return _NextBTN_Enabled; }
            set { SetProperty(ref _NextBTN_Enabled, value); }
        }

        public MainWindowVM()
        {
            InitializeUserData();
            InitializeBindingVariables();            
            UpdateImageInfo();
        }

        private void InitializeUserData()
        {            
            UserData.PathesToImages = new List<string>();
            UserData.PathesToCsvFiles = new List<string>();
            UserData.PathesToTxtFiles = new List<string>();
            UserData.FramedImages = new List<UserData.FramedImage>();
            UserData.MaskedImages = new List<UserData.MaskedImage>();
            UserData.SymbolMarkers = new List<char>();
            UserData.BackgroundColor = Color.FromArgb(128, 128, 128);
            UserData.FrameColor = Color.Blue;
            UserData.SymbolMarkers.Add('1');
            UserData.SymbolMarkers.Add('2');
            UserData.MarkerType = UserData.MARKER_TYPE_MASK;
            UserData.CroppingType = UserData.CROP_ONLY_SYMBOL;
        }

        private void InitializeBindingVariables()
        {           
            NumberOfCurrentImage = 1;
            PrevBTN_Enabled = true;
            NextBTN_Enabled = true;
        }

        /// <summary>
        /// Processes the images and updates the views
        /// </summary>
        public void UpdateImageInfo()
        {
            // Trying to parce data if it isn't
            if (UserData.GetCountOfParcedImages() == 0)
                UserData.TryToParceImages();

            // updating data if have parced images
            if (UserData.GetCountOfParcedImages() > 0)
            {
                // Getting source image
                string pathToSource = UserData.GetPathToImage(NumberOfCurrentImage - 1);
                BitmapImage sourceImage = new BitmapImage(new Uri(pathToSource));

                // Setting images depending on marker type
                if (UserData.MarkerType.Equals(UserData.MARKER_TYPE_FRAME))
                {
                    // Setting the <Image> views
                    MainImageSource = sourceImage;
                    // Here we getting image with drawn frames
                    ChangedImage1Source = ImageCollection.GetFramed(NumberOfCurrentImage - 1);
                    // Empty image
                    ChangedImage2Source = null;
                }
                else
                {
                    // Getting collection of masked images
                    List<BitmapImage> maskedImages = new List<BitmapImage>();                    
                    maskedImages.AddRange(ImageCollection.GetMasks(NumberOfCurrentImage - 1, UserData.CroppingType));
                    // Setting the <Image> views
                    MainImageSource = sourceImage;
                    // Here is two source images croppes by two marker-symbols from .csv file 
                    ChangedImage1Source = maskedImages[0];
                    ChangedImage2Source = maskedImages[1];
                }
                // Setting the path to lable
                PathToCurrentImage = pathToSource;

                // Disposing the memory
                GC.Collect();
            }            
        }
        
        ///////////////// service methods //////////////////////        
        
        /// <summary>
        /// Switches current image to previous
        /// </summary>
        public void MoveBack()
        {
            if (NumberOfCurrentImage > 1)
            {
                NumberOfCurrentImage--;
                UpdateImageInfo();
                PrevBTN_Enabled = true;
                if (!NextBTN_Enabled)
                    NextBTN_Enabled = true;
            }
            else
            {
                if(PrevBTN_Enabled)
                    PrevBTN_Enabled = false;
            }
        }
        /// <summary>
        /// Switches current image to next
        /// </summary>
        public void MoveForward()
        {
            if (NumberOfCurrentImage < UserData.GetCountOfParcedImages())
            {
                NumberOfCurrentImage++;                
                UpdateImageInfo();
                NextBTN_Enabled = true;
                if (!PrevBTN_Enabled)
                    PrevBTN_Enabled = true;
            }
            else
            {
                if (NextBTN_Enabled)
                    NextBTN_Enabled = false;
            }
        }
        /// <summary>
        /// Changes the current image to another by number
        /// </summary>        
        public void GoTo(int number)
        {
            if (NumberOfCurrentImage != number)
            {
                NumberOfCurrentImage = clamp(number, 1, UserData.GetCountOfParcedImages());
                PrevBTN_Enabled = true;
                NextBTN_Enabled = true;
                UpdateImageInfo();
            }
        }
        /// <summary>
        /// Changes type of cropping
        /// </summary>        
        public void ChangeCroppingType(int type)
        {
            if (type != UserData.CroppingType)
            {
                UserData.SwitchCroppingType();
                UpdateImageInfo();
            }
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
