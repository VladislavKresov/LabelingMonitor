using LabelingMonitor.Models;
using LabelingMonitor.Models.Input_data;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LabelingMonitor.ViewModels
{
    class ViewPageVM : BindableBase
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
        private string _MarkerTypeText;
        public string MarkerTypeText
        {
            get { return _MarkerTypeText; }
            set { SetProperty(ref _MarkerTypeText, value); }
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
        // Binding the enable state for "move back" button
        private bool _PrevBTN_Enabled;
        public bool PrevBTN_Enabled
        {
            get { return _PrevBTN_Enabled; }
            set { SetProperty(ref _PrevBTN_Enabled, value); }
        }
        // Binding the enable state for "move forward" button
        private bool _NextBTN_Enabled;
        public bool NextBTN_Enabled
        {
            get { return _NextBTN_Enabled; }
            set { SetProperty(ref _NextBTN_Enabled, value); }
        }
        // Binding the enable state for ComboBox
        private bool _CmbEnabled;
        public bool CmbEnabled
        {
            get { return _CmbEnabled; }
            set { SetProperty(ref _CmbEnabled, value); }
        }
        // Binding the marker Type state
        private int _MarkerType;
        public int MarkerType
        {
            get { return _MarkerType; }
            set { SetProperty(ref _MarkerType, value); }
        }
        // Binding the update state
        private bool _Updated;
        public bool Updated
        {
            get { return _Updated; }
            set { SetProperty(ref _Updated, value); }
        }


        private int CurrentFrameImage;
        private int CurrentMaskImage;
        private int CurrentImageNumber
        {
            get
            {
                if (MarkerType == UserData.MARKER_TYPE_FRAME)
                    return CurrentFrameImage;
                else
                    return CurrentMaskImage;
            }
            set
            {
                if (MarkerType == UserData.MARKER_TYPE_FRAME)
                    CurrentFrameImage = value;
                else
                    CurrentMaskImage = value;
            }
        }

        private static ViewPageVM instance;
        private ViewPageVM()
        {
            PropertyChanged += ViewPage_PropertyChanged;
            InitializeVariables();
        }

        public static ViewPageVM GetInstance()
        {
            if(instance == null)
            {
                instance = new ViewPageVM();
                return instance;
            }
            return instance;
        }

        private void InitializeVariables()
        {           
            NumberOfCurrentImage = 1;
            PrevBTN_Enabled = true;
            NextBTN_Enabled = true;
            CmbEnabled = false;
            CurrentFrameImage = 1;
            CurrentMaskImage = 1;
            Updated = false;
        }

        private void ViewPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MarkerType))
            {
                // on marker type changing                
                ResetViews();
                UpdateImages();
            }
            if (e.PropertyName == nameof(Updated))
            {
                // on data update
                if(!Updated)
                {
                    UpdateImages();
                }
            }
        }

        /// <summary>
        /// Processes the images and updates the views
        /// </summary>
        private void UpdateImages()
        {
            try
            {
                Updated = true;
                // Validating views
                ValidateViewsEnablity();
                // Validating the number of image in case changing of marker type
                NumberOfCurrentImage = clamp(CurrentImageNumber, 1, UserData.GetCountOfParcedImages(MarkerType));
                // updating data if have parced images
                if (UserData.GetCountOfParcedImages(MarkerType) > 0)
                {
                    // Getting source image
                    string pathToSource = UserData.GetPathToImage(NumberOfCurrentImage - 1, MarkerType);
                    BitmapImage sourceImage = new BitmapImage(new Uri(pathToSource));

                    // Setting images depending on marker type
                    if (MarkerType == UserData.MARKER_TYPE_FRAME)
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
            catch (Exception e)
            {
                string messageBoxText = e.Message;
                string caption = "Couldn't open image";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);
                ResetViews();
            }
        }

        ///////////////// service methods //////////////////////      

        /// <summary>
        /// Switches current image to previous
        /// </summary>
        public void MoveBack()
        {
            if (CurrentImageNumber > 1)
            {
                CurrentImageNumber--;
                Updated = false;
            }
        }

        /// <summary>
        /// Switches current image to next
        /// </summary>
        public void MoveForward()
        {
            if (CurrentImageNumber < UserData.GetCountOfParcedImages(MarkerType))
            {
                CurrentImageNumber++;
                Updated = false;
            }
        }

        /// <summary>
        /// Changes the current image to another by number
        /// </summary>        
        public void GoTo(int number)
        {
            if (CurrentImageNumber != number)
            {
                CurrentImageNumber = number;
                Updated = false;
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
                Updated = false;
            }
        }        

        private void ValidateViewsEnablity()
        {
            if (MarkerType == UserData.MARKER_TYPE_FRAME)
            {
                CmbEnabled = false;
                MarkerTypeText = "Marker type: framed";
            }
            else
            {
                CmbEnabled = true;
                MarkerTypeText = "Marker type: masked";
            }

            if (CurrentImageNumber == 1)
                PrevBTN_Enabled = false;
            else
                PrevBTN_Enabled = true;

            if (CurrentImageNumber == UserData.GetCountOfParcedImages(MarkerType))
                NextBTN_Enabled = false;
            else
                NextBTN_Enabled = true;
        }

        private void ResetViews()
        {
            PathToCurrentImage = "";
            MainImageSource = null;
            ChangedImage1Source = null;
            ChangedImage2Source = null;
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
