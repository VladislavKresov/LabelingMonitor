using LabelingMonitor.Models;
using LabelingMonitor.Models.Input_data;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LabelingMonitor.ViewModels
{
    class MainWindowVM : BindableBase
    {
        public readonly int MARKER_TYPE_FRAME = 0;
        public readonly int MARKER_TYPE_MASK = 1;

        // Binding the enable statement for "open images" menuItem
        private bool _OpenImagesEnabled;
        public bool OpenImagesEnabled
        {
            get { return _OpenImagesEnabled; }
            set { SetProperty(ref _OpenImagesEnabled, value); }
        }        
        // Binding the marker type
        private int _MarkerType;
        public int MarkerType
        {
            get { return _MarkerType; }
            set { SetProperty(ref _MarkerType, value); }            
        }
        // Binding update state
        private bool _NeedUpdate;
        public bool NeedUpdate
        {
            get { return _NeedUpdate; }
            set { SetProperty(ref _NeedUpdate, value); }
        }

        private static MainWindowVM instance;
        private MainWindowVM()
        {
            InitializeBindingVariables();            
            InitializeUserData();
        }

        public static MainWindowVM GetInstance()
        {
            if (instance == null)
            {
                instance = new MainWindowVM();
                return instance;
            }
            else
                return instance;
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
            UserData.CroppingType = UserData.CROP_ONLY_SYMBOL;          
        }

        private void InitializeBindingVariables()
        {           
            OpenImagesEnabled = true;
        }   

        ///////////////// service methods //////////////////////      
        /// <summary>
        /// Sets collection to UserData and validating views
        /// </summary>
        public void OpenNewImageCollection(List<string> list)
        {
            UserData.PathesToImages = list;
            // Trying to parce data if it isn't
            UserData.TryToParceImages(MarkerType);
            // Notifying for updating
            NeedUpdate = true;
        }

        /// <summary>
        /// Sets collection to UserData and validating views
        /// </summary>
        public void OpenNewFileCollection(List<string> list)
        {
            UserData.SetFileCollection(list, MarkerType);
            // Trying to parce data if it isn't
            UserData.TryToParceImages(MarkerType);
            // Notifying for updating
            NeedUpdate = true;
        }

        /// <summary>
        /// Switches marker type and validates views
        /// </summary>
        public void SwitchMarkerType()
        {
            if (MarkerType == MARKER_TYPE_FRAME)
                MarkerType = MARKER_TYPE_MASK;
            else
                MarkerType = MARKER_TYPE_FRAME;

            ValidateViewsEnablity();
            // Notifying for updating
            NeedUpdate = true;
        }

        private void ValidateViewsEnablity()
        {
            if (MarkerType == MARKER_TYPE_FRAME)
            {
                OpenImagesEnabled = false;
            }
            else
            {
                OpenImagesEnabled = true;
            }
        }

    }
}
