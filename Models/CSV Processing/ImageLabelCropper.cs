using LabelingMonitor.Models.Input_data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LabelingMonitor.Models.CSV_Processing
{
    class ImageLabelCropper
    {
        public static int CROP_ALL_EXCEPT_VALUE = 0;
        public static int CROP_VALUE = 1;

        /// <summary>
        /// Crops the images by color mask
        /// </summary>
        /// <param name="image">Original image</param>
        /// <param name="mask">Color mask</param>
        /// <param name="maskColor">Color in mask</param>
        /// <param name="backgroundColor">Color to change cropped pixels</param>
        /// <param name="CropperMode">Mode to crop</param>
        /// <returns>Cropped original image</returns>
        public static byte[] Crop(byte[] image, byte[] mask, byte maskColor, byte backgroundColor, int CropperMode)
        {
            if (image.Length == mask.Length)
            {
                byte[] croppedImage = new byte[image.Length];
                if (CropperMode == CROP_ALL_EXCEPT_VALUE)
                {
                    for (int i = 0; i < mask.Length; i++)
                    {
                        if (mask[i] == maskColor)
                            croppedImage[i] = image[i];
                        else
                            croppedImage[i] = backgroundColor;
                    }
                }
                if (CropperMode == CROP_VALUE)
                {
                    for (int i = 0; i < mask.Length; i++)
                    {
                        if (mask[i] != maskColor)
                            croppedImage[i] = image[i];
                        else
                            croppedImage[i] = backgroundColor;
                    }
                }
                return croppedImage;
            }
            else
            {
                throw new Exception("Size of image is not equals to mask size");
            }
        }

        public static List<BitmapImage> Crop(string pathToImage, string pathToCsvMask, List<char> symbols)
        {
            List<BitmapImage> newImages = new List<BitmapImage>();
            Bitmap sourceImage = new Bitmap(pathToImage);
            Bitmap newImage1 = new Bitmap(pathToImage);
            Bitmap newImage2 = new Bitmap(pathToImage);
            int width = newImage1.Width;
            int height = newImage1.Height;
            char[,] mask = ReadMaskFromCSV(pathToCsvMask, width, height);

            // Lockes Bitmap pixels to change them faster
            LockBitmap SourceLockedImage = new LockBitmap(sourceImage);
            SourceLockedImage.LockBits();
            LockBitmap NewlockedImage1 = new LockBitmap(newImage1);
            NewlockedImage1.LockBits();
            LockBitmap NewLockedImage2 = new LockBitmap(newImage2);
            NewLockedImage2.LockBits();

            // Compares pixels to mask and sets them from source if equals
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {                    
                    if (mask[x, y] == symbols[0])
                        NewlockedImage1.SetPixel(x, y, UserData.BackgroundColor);
                    if (mask[x, y] == symbols[1])
                        NewLockedImage2.SetPixel(x, y, UserData.BackgroundColor);
                }
            }

            //Unlocks pixels
            SourceLockedImage.UnlockBits();
            NewlockedImage1.UnlockBits();                
            NewLockedImage2.UnlockBits();            
            
            //Add cropped images to list
            newImages.Add(BitmapToBitmapImage(newImage1));
            newImages.Add(BitmapToBitmapImage(newImage2));

            //Disposes bitmaps
            sourceImage.Dispose();
            newImage1.Dispose();
            newImage2.Dispose();

            return newImages;            
        }

        private static char[,] ReadMaskFromCSV(string csvMask, int width, int height)
        {
            char[,] mask = new char[width, height];
            using (StreamReader reader = new StreamReader(csvMask))
            {
                for (int y = 0; y < height; y++)
                {
                    string[] line = reader.ReadLine().Split(';');
                    for (int x = 0; x < width; x++)
                    {
                        mask[x, y] = char.Parse(line[x]);
                    }
                }
            }
            return mask;
        }

        private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                bitmap.Dispose();
                return bitmapimage;
            }
        }
    }
}
