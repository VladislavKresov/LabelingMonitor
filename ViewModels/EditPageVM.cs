using LabelingMonitor.Models.Input_data;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LabelingMonitor.ViewModels
{
    class EditPageVM : BindableBase
    {             
        // Binding the main image
        private BitmapImage _MainImageSource;
        public BitmapImage MainImageSource
        {
            get { return _MainImageSource; }
            set { SetProperty(ref _MainImageSource, value); }
        }
        // Binding the edited image
        private BitmapImage _EditedImageSource;
        public BitmapImage EditedImageSource
        {
            get { return _EditedImageSource; }
            set { SetProperty(ref _EditedImageSource, value); }
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
        // Binding the enable statement for ComboBox
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

        private static EditPageVM instance;

        private EditPageVM()
        {
            InitializeVariables();
            PropertyChanged += EditPage_PropertyChanged;
        }

        private void InitializeVariables()
        {           
            PrevBTN_Enabled = true;
            NextBTN_Enabled = true;
            CmbEnabled = false;
            CurrentFrameImage = 1;
            CurrentMaskImage = 1;
            Updated = true;
        }
        public static EditPageVM GetInstance()
        {
            if(instance == null)
            {
                instance = new EditPageVM();
                return instance;
            }
            return instance;
        }

        private void EditPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MarkerType))
            {
                // on marker type changing                
                UpdateImages();
            }
            if (e.PropertyName == nameof(Updated))
            {
                // on data update
                if (!Updated)
                {
                    UpdateImages();
                    Updated = true;
                }
            }
        }

        private void UpdateImages()
        {
            try
            {
                ValidateViewsEnablity();
                // updating data if have parced images
                if (UserData.GetCountOfParcedImages(MarkerType) > 0)
                {
                    // Getting source image
                    string pathToSource = UserData.GetPathToImage(CurrentImageNumber - 1, MarkerType);
                    BitmapImage sourceImage = new BitmapImage(new Uri(pathToSource));                    

                    PathToCurrentImage = pathToSource;
                    MainImageSource = sourceImage;

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

        private void ResetViews()
        {
            PathToCurrentImage = "";
            MainImageSource = null;
            EditedImageSource = null;
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
    }

}
