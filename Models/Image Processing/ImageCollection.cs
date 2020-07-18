using LabelingMonitor.Models.CSV_Processing;
using LabelingMonitor.Models.Input_data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Media.Imaging;

namespace LabelingMonitor.Models
{
    /// <summary>
    ///  Forms list of original and changes images
    /// </summary>
    class ImageCollection
    {
        /// <summary>
        /// Forms list of cropped images by color markers
        /// </summary>
        /// <param name="collectionIndex"> The index of path to image and .csv in UserData.cs </param>
        /// <returns>List of images</returns>
        public static List<BitmapImage> GetCroppedImagesCollection(int collectionIndex) 
        {
            List<BitmapImage> images = new List<BitmapImage>();
            List<char> symbols = UserData.ColorMarkers;
            string pathToSource = UserData.GetPathToImage(collectionIndex);
            string pathToCsvMask = UserData.GetPathToCsvFile(collectionIndex);
            BitmapImage sourceImage = new BitmapImage(new Uri(pathToSource));

            //Forms list of images
            images.Add(sourceImage);
            List<BitmapImage> croppedImages = ImageLabelCropper.Crop(pathToSource, pathToCsvMask, symbols, ImageLabelCropper.CROP_ONLY_SYMBOL);
            foreach (BitmapImage croppedImage in croppedImages)
                images.Add(croppedImage);

            return images;
        }

    }
}
