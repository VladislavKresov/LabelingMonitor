using LabelingMonitor.Models.Image_Processing;
using LabelingMonitor.Models.Input_data;
using LabelingMonitor.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using System.Windows.Media.Imaging;

namespace LabelingMonitor.Models
{
    /// <summary>
    ///  Forms list of changes images
    /// </summary>
    class ImageCollection
    {
        /// <summary>
        /// Forms list of changed images by color masks
        /// </summary>
        public static List<BitmapImage> GetMasks(int IndexOfImageInUserData, int CroppingMode) 
        {
            List<BitmapImage> images = new List<BitmapImage>();            
            List<char> symbols = UserData.SymbolMarkers;
            string pathToSource = UserData.GetPathToImage(IndexOfImageInUserData, UserData.MARKER_TYPE_MASK);
            string pathToCsvMask = UserData.GetPathToCsvFile(IndexOfImageInUserData);
            
            List<BitmapImage> croppedImages = ImageProcess.CropMasks(pathToSource, pathToCsvMask, symbols, CroppingMode);
            foreach (BitmapImage croppedImage in croppedImages)
                images.Add(croppedImage);

            return images;
        }

        /// <summary>
        /// Creates the image with the drawn frame
        /// </summary>
        public static BitmapImage GetFramed(int IndexOfImageInUserData)
        {
            // Getting the source image            
            UserData.FramedImage framedImage = UserData.FramedImages[IndexOfImageInUserData];
            BitmapImage image = new BitmapImage(new Uri(framedImage.source));
            // Creating the frames
            Rectangle[] frames = new Rectangle[framedImage.frames.Count];
            for(int i = 0; i < framedImage.frames.Count; i++)
            {
                int x = framedImage.frames[i].TopLeftX;
                int y = framedImage.frames[i].TopLeftY;
                int width = framedImage.frames[i].BottomRightX - x;
                int height = framedImage.frames[i].BottomRightY - y;
                frames[i] = new Rectangle(x, y, width, height);
            }
            // Drawing the frames
            var byteImage = ImageProcess.BitmapImageToByte(image);
            var framedByteImage = ImageProcess.DrawFrames(byteImage, frames, UserData.FrameColor);
            return ImageProcess.ByteToBitmapImage(framedByteImage);            
        }   

        /// <summary>
        /// Applyes effects to image
        /// </summary>
        public static BitmapImage GetProcessed(BitmapImage sourceImage, List<int> effects)
        {
            var processedImage = ImageProcess.BitmapImageToByte(sourceImage);

            foreach(int effect in effects)
                switch (effect)
                {
                    case EditPageVM.EFFECT_LIGHTENING:
                        processedImage = ImageProcess.Brightness(processedImage, 25);
                        break;
                    case EditPageVM.EFFECT_SHADOWING:
                        processedImage = ImageProcess.Shadowing(processedImage, 5);
                        break;
                    case EditPageVM.EFFECT_CROP:
                        int indent = EditPageVM.IndentToCrop;                        
                        int width = sourceImage.PixelWidth - 2 * indent;
                        int height = sourceImage.PixelHeight - 2 * indent;
                        processedImage = ImageProcess.Crop(processedImage, indent, indent, width, height);
                        break;
                    case EditPageVM.EFFECT_ROTATE:
                        processedImage = ImageProcess.Rotate(processedImage, -90f);
                        break;
                    case EditPageVM.EFFECT_PIXELIZE:
                        processedImage = ImageProcess.Quality(processedImage, 25);
                        break;
                    default:
                        break;
                }

            return ImageProcess.ByteToBitmapImage(processedImage);
        }

        /// <summary>
        /// Creates the image to directory
        /// </summary>
        public static void Save(BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        private static int clamp(int value, int minVal, int maxVal)
        {
            if (value < minVal)
                return minVal;
            if (value > maxVal)
                return maxVal;
            return value;
        }

    }
}
