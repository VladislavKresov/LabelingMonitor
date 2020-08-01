using LabelingMonitor.Models;
using LabelingMonitor.Models.File_Processing;
using LabelingMonitor.Models.Input_data;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using static LabelingMonitor.Models.Input_data.UserData;

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
        // Binding the enable state for "Undo" button
        private bool _UndoBTN_Enabled;
        public bool UndoBTN_Enabled
        {
            get { return _UndoBTN_Enabled; }
            set { SetProperty(ref _UndoBTN_Enabled, value); }
        }
        // Binding the enable state for "Create images" button
        private bool _CreateImagesBTN_Enabled;
        public bool CreateImagesBTN_Enabled
        {
            get { return _CreateImagesBTN_Enabled; }
            set { SetProperty(ref _CreateImagesBTN_Enabled, value); }
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
        // Binding the cropping indent
        private int _CroppingIndent;
        public int CroppingIndent
        {
            get { return _CroppingIndent; }
            set { SetProperty(ref _CroppingIndent, value); }
        }

        public const int EFFECT_CLEAR = 0;
        public const int EFFECT_LIGHTENING = 1;
        public const int EFFECT_SHADOWING = 2;
        public const int EFFECT_CROP = 3;
        public const int EFFECT_ROTATE = 4;
        public const int EFFECT_PIXELIZE = 5;

        private int CurrentFrameImage;
        private int CurrentMaskImage;
        //returns image number depending on marker type
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
        private List<int> Effects;

        private static EditPageVM instance;
        public static int IndentToCrop;

        private EditPageVM()
        {
            InitializeVariables();
            ValidateViewsEnablity();
            PropertyChanged += EditPage_PropertyChanged;
        }

        private void EditPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
                if (!Updated)
                {
                    UpdateImages();
                    Updated = true;
                }
            }
            if (e.PropertyName == nameof(CroppingIndent))
            {
                IndentToCrop = _CroppingIndent;
            }
        }

        private void InitializeVariables()
        {
            CurrentFrameImage = 1;
            CurrentMaskImage = 1;
            Updated = true;
            CroppingIndent = 0;
            IndentToCrop = 0;
            Effects = new List<int>();
        }

        public static EditPageVM GetInstance()
        {
            if (instance == null)
            {
                instance = new EditPageVM();
                return instance;
            }
            return instance;
        }

        public void AddEffect(int effect)
        {
            if (effect == EFFECT_CLEAR)
                ClearEffects();
            else
                if(effect != EFFECT_CROP || !Effects.Contains(EFFECT_CROP))
                    Effects.Add(effect);

            Updated = false;
        }

        public void UndoLastEffect()
        {
            Effects.RemoveAt(Effects.Count - 1);
            Updated = false;
        }

        public void ClearEffects()
        {
            Effects.Clear();
            Updated = false;
        }

        private void UpdateImages()
        {
            try
            {
                ValidateViewsEnablity();
                // updating data if have parced images
                if (GetCountOfParcedImages(MarkerType) > 0)
                {
                    // Getting source image
                    string pathToSource = GetPathToImage(CurrentImageNumber - 1, MarkerType);
                    BitmapImage sourceImage = new BitmapImage(new Uri(pathToSource));

                    // Drawing markers
                    BitmapImage markedImage;
                    if(MarkerType == MARKER_TYPE_FRAME)
                        markedImage = ImageCollection.GetFramed(CurrentImageNumber - 1);
                    else
                    {
                        markedImage = ImageCollection.GetMasks(CurrentImageNumber - 1, UserData.CroppingType)[0];
                    }
                    // Processing effcts
                    BitmapImage processedImage = ImageCollection.GetProcessed(markedImage, Effects);

                    PathToCurrentImage = pathToSource;
                    MainImageSource = sourceImage;
                    EditedImageSource = processedImage;

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
                System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);
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

            if (Effects.Count == 0)
                UndoBTN_Enabled = false;
            else
                UndoBTN_Enabled = true;

            if (UserData.GetCountOfParcedImages(MarkerType) > 0)
                CreateImagesBTN_Enabled = true;
            else
                CreateImagesBTN_Enabled = false;
        }

        /// <summary>
        /// Creates new processed marker files and images
        /// </summary>
        public async void CreateImagesAsync()
        {
            // Show the FolderBrowserDialog.
            var folderBrowserDialog = new FolderBrowserDialog();           
            DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                await Task.Run(() =>
                {                
                string folder = folderBrowserDialog.SelectedPath;
                // If framed type
                if (MarkerType == UserData.MARKER_TYPE_FRAME)
                {
                    // Asking for a new file
                    string caption = "Create new marker file?";
                    string messageBoxText = "Press 'Yes' if you want to create new marker file. Or 'No' if you want to add markers to current file.";
                    MessageBoxButton button = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Question;
                    MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);

                    // Setting file to write
                    string pathToMarkerFile;                    
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        pathToMarkerFile = CreateNewPath(folder, UserData.PathToTxtFile);
                    }
                    else
                    {
                        pathToMarkerFile = UserData.PathToTxtFile;
                    }

                    var processedImages = UserData.FramedImages;                    
                    // Processing frames
                    processedImages = FileProcess.GetProcessedFramedImages(processedImages, Effects);

                    // Creating new processed images
                    for (int i = 0; i < processedImages.Count; i++)
                    {
                        FramedImage newImage = new FramedImage();
                        newImage.source = CreateProcessedImage(folder, processedImages[i].source);
                        newImage.frames = processedImages[i].frames;
                        processedImages[i] = newImage;
                    }

                    // Parcing to writeble format
                    var parcedLines = FileProcess.ParceFramedImages(processedImages);

                    // Writing processed images
                    using (StreamWriter sw = new StreamWriter(pathToMarkerFile, true))
                    {
                        foreach (var line in parcedLines)
                            sw.WriteLine(line);
                    }
                }
                // If masked type
                else
                {
                    // Creating new processed images               
                    for (int i = 0; i < UserData.MaskedImages.Count; i++)
                    {
                        CreateProcessedImage(folder, UserData.MaskedImages[i].source);
                        CreateProcessedMask(folder, UserData.MaskedImages[i]);                        
                    }
                }
                GC.Collect();
                });
            }
        }

        /// <summary>
        /// Creates the image and returned new path
        /// </summary>        
        private string CreateProcessedImage(string folder, string source)
        {
            // Creating new image path
            string newImagePath = CreateNewPath(folder, source);
            // Processing image
            var processedImage = ImageCollection.GetProcessed(new BitmapImage(new Uri(source)), Effects);
            // Saving the processed picture
            ImageCollection.Save(processedImage, newImagePath);
            return newImagePath;
        }

        /// <summary>
        /// Creates processed marker file
        /// </summary>
        private void CreateProcessedMask(string folder, MaskedImage maskedImage)
        {
            try
            {
                StringBuilder csvContent = new StringBuilder();
                string newFilePath = CreateNewPath(folder,maskedImage.csvMask);
                // Getting processed mask
                char[,] processedMask = FileProcess.GetProcessedMask(maskedImage, Effects);
                // Writing to new File                
                int width = processedMask.GetLength(0);
                int height = processedMask.GetLength(1);
                for (int y = 0; y < height; y++)
                {
                    string line = "";
                    for (int x = 0; x < width; x++)
                    {
                        line+=processedMask[x,y]+";";
                    }
                    csvContent.AppendLine(line);
                }
                
                File.AppendAllText(newFilePath, csvContent.ToString());
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message,
                                           "Error",
                                           MessageBoxButtons.OK,
                                           MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creates new unique path from source
        /// </summary>
        private string CreateNewPath(string folder, string source)
        {
            int count = 1;
            string sourceExtension = Path.GetExtension(source);
            string newImageName = count + sourceExtension;
            string newImagePath = folder + @"\" + newImageName;
            for (count = 1; File.Exists(newImagePath); count++)
            {
                newImageName = count + sourceExtension;
                newImagePath = folder + @"\" + newImageName;
            }
            return newImagePath;
        }
        
    }
}
