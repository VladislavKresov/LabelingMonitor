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
        public int MarkerType;
        private ViewPageVM viewPageVM;
        private EditPageVM editPageVM;

        // Binding the enable statement for "open images" menuItem
        private bool _OpenImagesEnabled;
        public bool OpenImagesEnabled
        {
            get { return _OpenImagesEnabled; }
            set { SetProperty(ref _OpenImagesEnabled, value); }
        }                

        private static MainWindowVM instance;
        private MainWindowVM()
        {
            InitializeUserData();
            InitializeVariables();            
        }

        public static MainWindowVM GetInstance()
        {
            if (instance == null)
            {
                instance = new MainWindowVM();
                return instance;
            }
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

        private void InitializeVariables()
        {           
            OpenImagesEnabled = true;
            viewPageVM = ViewPageVM.GetInstance();
            editPageVM = EditPageVM.GetInstance();
            MarkerType = UserData.MARKER_TYPE_MASK;
            viewPageVM.MarkerType = MarkerType;
            editPageVM.MarkerType = MarkerType;            
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
            // Updating pages
            UpdatePages();
        }        

        /// <summary>
        /// Sets collection to UserData and validating views
        /// </summary>
        public void OpenNewFileCollection(List<string> list)
        {
            UserData.SetFileCollection(list, MarkerType);
            // Trying to parce data if it isn't
            UserData.TryToParceImages(MarkerType);
            // Updating pages
            UpdatePages();
        }

        /// <summary>
        /// Switches marker type and validates views
        /// </summary>
        public void SwitchMarkerType()
        {
            if (MarkerType == UserData.MARKER_TYPE_FRAME)
                MarkerType = UserData.MARKER_TYPE_MASK;
            else
                MarkerType = UserData.MARKER_TYPE_FRAME;

            ValidateViewsEnablity();
            // Notifying for updating marker type
            editPageVM.MarkerType = MarkerType;
            viewPageVM.MarkerType = MarkerType;
        }
        /// <summary>
        /// Enable or Disable views depending on marker type
        /// </summary>
        private void ValidateViewsEnablity()
        {
            if (MarkerType == UserData.MARKER_TYPE_FRAME)
            {
                OpenImagesEnabled = false;
            }
            else
            {
                OpenImagesEnabled = true;
            }
        }
        /// <summary>
        /// Notifying the pages to update images
        /// </summary>
        private void UpdatePages()
        {
            viewPageVM.Updated = false;
            editPageVM.Updated = false;
        }

    }
}
