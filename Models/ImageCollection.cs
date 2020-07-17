using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            List<byte> colors = Input_data.UserData.ColorMarkers;
            byte backgroundColor = Input_data.UserData.BackgroundColor;

            var image = ImageFileToByteArray(Input_data.UserData.PathesToImages[collectionIndex]);
            var mask = CsvFileToByteArray(Input_data.UserData.PathesToCsvFiles[collectionIndex]);

            foreach (byte color in colors)
                images.Add(ConvertToBitMapImage(image));
                //images.Add(ConvertToBitMapImage(CSV_Processing.ImageLabelCropper.Crop(image,
                //    mask,
                //    color,
                //    backgroundColor, 
                //    CSV_Processing.ImageLabelCropper.CROP_ALL_EXCEPT_VALUE)));
            return images;
        }
        /// <summary>
        /// Creates array from .csv file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>byte[] array with all values in file</returns>
        private static byte[] CsvFileToByteArray(string path)
        {
            List<byte> valuesList = new List<byte>();
            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    char[] separator = { ';', '\n' };
                    string[] values = reader.ReadLine().Split(separator);
                    for (int i = 0; i < values.Length-2; i++)
                    {
                        valuesList.Add(byte.Parse(values[i]));
                    }
                }
            }
            return valuesList.ToArray();
        }
        /// <summary>
        /// Reads the image and creates byte[] array with the colors of pixels
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] ImageFileToByteArray(string fileName)
        {
            byte[] fileData = null;
            using (FileStream fs = File.OpenRead(fileName))
            {
                var binaryReader = new BinaryReader(fs);
                fileData = binaryReader.ReadBytes((int)fs.Length);
                binaryReader.Dispose();
            }
            return fileData;
        }
        /// <summary>
        /// Converts byte[] array to image
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static BitmapImage ConvertToBitMapImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(bytes))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            //image.Freeze();
            return image;
        }
    }
}
