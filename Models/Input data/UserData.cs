using LabelingMonitor.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingMonitor.Models.Input_data
{
    /// <summary>
    /// Contains the user files and statements
    /// </summary>
    class UserData
    {
        public struct MaskedImage
        {
            public string source;
            public string csvMask;
        }
        public struct Frame
        {
            public int TopLeftX;
            public int TopLeftY;
            public int BottomRightX;
            public int BottomRightY;
        }
        public struct FramedImage
        {
            public string source;
            public List<Frame> frames;
        }

        public static readonly int MARKER_TYPE_FRAME = 0;
        public static readonly int MARKER_TYPE_MASK = 1;
        public static readonly int CROP_ONLY_SYMBOL = 0;
        public static readonly int CROP_ALL_EXCEPT_SYMBOL = 1;

        /////////// Masked images
        public static int CroppingType { get; set; }
        public static List<string> PathesToImages { get; set; }
        public static List<string> PathesToCsvFiles { get; set; }
        public static List<MaskedImage> MaskedImages { get; set; }
        public static List<char> SymbolMarkers { get; set; }            
        public static Color BackgroundColor { get; set; }
        public static Color FrameColor { get; set; }

        /////////// Framed images
        public static string PathToTxtFile { get; set; }        
        public static List<FramedImage> FramedImages { get; set; }

        ///////////////////////// Service methods
        /// <summary>
        /// Returns the path to image depending on the marker type from MaskedImages collection
        /// </summary>       
        public static string GetPathToImage(int index, int MarkerType)
        {
            if(MarkerType == MARKER_TYPE_FRAME)
                return FramedImages[index].source;
            else
                return MaskedImages[index].source;
        }

        /// <summary>
        /// Return path to (.csv) mask from MaskedImages collection
        /// </summary>        
        public static string GetPathToCsvFile(int index)
        {
            return MaskedImages[index].csvMask;
        }                        

        /// <summary>
        /// Returns the count of parced images depending on the marker type        
        /// </summary>        
        public static int GetCountOfParcedImages(int MarkerType)
        {
            if (MarkerType == MARKER_TYPE_FRAME)
                return FramedImages.Count;
            else
                return MaskedImages.Count;
        }        

        /// <summary>
        /// Changes type between two states
        /// </summary>
        public static void SwitchCroppingType()
        {
            if (CroppingType == CROP_ONLY_SYMBOL)
                CroppingType = CROP_ALL_EXCEPT_SYMBOL;
            else
                CroppingType = CROP_ONLY_SYMBOL;
        }   

        /// <summary>
        /// Parcing the Images collection depending on the marker type        
        /// </summary>        
        public static bool TryToParceImages(int MarkerType)
        {
            if (MarkerType == MARKER_TYPE_FRAME)
                return TryToParceFramedImages();
            else
                return TryToParceMaskedImages();
        }

        /// <summary>
        /// Parcing images data from files to FramesImages list        
        /// </summary>        
        
        private static bool TryToParceFramedImages()
        {
            if (PathToTxtFile == string.Empty)
                return false;

            try
            {
                List<FramedImage> parcedImages = new List<FramedImage>();
                
                using (StreamReader sr = new StreamReader(PathToTxtFile))
                {
                    string currentLine;
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        string[] splitedLine = currentLine.Split(',');
                        List<Frame> currentFrames = new List<Frame>();
                        for (int indexOfStartFramePos = 1; indexOfStartFramePos <= splitedLine.Length-4; indexOfStartFramePos+=4)
                        {
                            Frame frame = new Frame();
                            frame.TopLeftX = int.Parse(splitedLine[indexOfStartFramePos]);
                            frame.TopLeftY = int.Parse(splitedLine[indexOfStartFramePos+1]);
                            frame.BottomRightX = int.Parse(splitedLine[indexOfStartFramePos+2]);
                            frame.BottomRightY = int.Parse(splitedLine[indexOfStartFramePos+3]);
                            currentFrames.Add(frame);
                        }
                        FramedImage currentImage = new FramedImage();
                        currentImage.source = splitedLine[0];
                        currentImage.frames = currentFrames;
                        parcedImages.Add(currentImage);
                    }
                }
                
                FramedImages = parcedImages;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Matches images and .csv masks by compairing the names        
        /// </summary>        
        private static bool TryToParceMaskedImages()
        {
            List<MaskedImage> parcedImages = new List<MaskedImage>();
            int imageCount = PathesToImages.Count();
            int maskCount = PathesToCsvFiles.Count();
            for (int imgIndx = 0; imgIndx < imageCount; imgIndx++)
            {
                string imageName = Path.GetFileNameWithoutExtension(PathesToImages[imgIndx]);
                for (int maskIndx = 0; maskIndx < maskCount; maskIndx++)
                {
                    string maskName = Path.GetFileNameWithoutExtension(PathesToCsvFiles[maskIndx]);
                    if(imageName == maskName)
                    {
                        MaskedImage image = new MaskedImage();
                        image.source = PathesToImages[imgIndx];
                        image.csvMask = PathesToCsvFiles[maskIndx];
                        parcedImages.Add(image);
                        break;
                    }
                }
            }
            if (parcedImages.Count > 0)
            {
                MaskedImages = parcedImages;
                return true;
            }                                                             
            return false;
        }
    }
}
