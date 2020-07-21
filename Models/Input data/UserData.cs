using System;
using System.Collections.Generic;
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

        public const int MARKER_TYPE_FRAME = 0;
        public const int MARKER_TYPE_MASK = 1;
        public const int CROP_ONLY_SYMBOL = 0;
        public const int CROP_ALL_EXCEPT_SYMBOL = 1;

        public static int MarkerType { get; set; }
        /////////// Masked images
        public static int CroppingType { get; set; }
        public static List<string> PathesToImages { get; set; }
        public static List<string> PathesToCsvFiles { get; set; }
        public static List<MaskedImage> MaskedImages { get; set; }
        public static List<char> SymbolMarkers { get; set; }            
        public static Color BackgroundColor { get; set; }
        public static Color FrameColor { get; set; }

        /////////// Framed images
        public static List<string> PathesToTxtFiles { get; set; }        
        public static List<FramedImage> FramedImages { get; set; }

        ///////////////////////// Service methods
        /// <summary>
        /// Returns the path to image depending on the marker type from MaskedImages collection
        /// </summary>       
        public static string GetPathToImage(int index)
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
        //Returns the count of parced images depending on the marker type        
        /// </summary>        
        public static int GetCountOfParcedImages()
        {
            if (MarkerType == MARKER_TYPE_FRAME)
                return FramedImages.Count;
            else
                return MaskedImages.Count;
        }

        /// <summary>
        /// Changes type between two states
        /// </summary>
        public static void SwitchMarkerType()
        {
            if (MarkerType == MARKER_TYPE_FRAME)
                MarkerType = MARKER_TYPE_MASK;
            else
                MarkerType = MARKER_TYPE_FRAME;
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
        // Parcing the Images collection depending on the marker type        
        /// </summary>        
        public static bool TryToParceImages()
        {
            if (MarkerType == MARKER_TYPE_FRAME)
                return TryToParceFramedImages();
            else
                return TryToParceMaskedImages();
        }

        /// <summary>
        // Parcing images data from files to FramesImages list        
        /// </summary>        
        private static bool TryToParceFramedImages()
        {
            if (PathesToTxtFiles.Count == 0)
                return false;

            try
            {
                List<FramedImage> parcedImages = new List<FramedImage>();
                foreach (string fileName in PathesToTxtFiles)
                {
                    using (StreamReader sr = new StreamReader(fileName))
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
        // Matches images and .csv masks by compairing the names        
        /// </summary>        
        private static bool TryToParceMaskedImages()
        {
            if(PathesToImages.Count() == PathesToCsvFiles.Count())
            {
                List<MaskedImage> parcedImages = new List<MaskedImage>();
                int imageCount = PathesToImages.Count();
                for (int imgIndx = 0; imgIndx < imageCount; imgIndx++)
                {
                    string imageName = Path.GetFileNameWithoutExtension(PathesToImages[imgIndx]);
                    for (int maskIndx = 0; maskIndx < imageCount-1; maskIndx++)
                    {
                        string maskName = Path.GetFileNameWithoutExtension(PathesToCsvFiles[imgIndx]);
                        if(imageName == maskName)
                        {
                            MaskedImage image = new MaskedImage();
                            image.source = PathesToImages[imgIndx];
                            image.csvMask = PathesToCsvFiles[imgIndx];
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
            }
            return false;
        }
    }
}
