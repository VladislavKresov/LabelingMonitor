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
            string pathToSource = UserData.GetPathToImage(IndexOfImageInUserData);
            string pathToCsvMask = UserData.GetPathToCsvFile(IndexOfImageInUserData);
            
            List<BitmapImage> croppedImages = Crop(pathToSource, pathToCsvMask, symbols, CroppingMode);
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
            using (Bitmap image = new Bitmap(framedImage.source))
            {
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
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.DrawRectangles(new Pen(UserData.FrameColor, 10f), frames);                    
                }
                return BitmapToBitmapImage(image);
            }
        }

        /// <summary>
        /// Croppes two source images by two marker-symbols from .csv file
        /// </summary>
        private static List<BitmapImage> Crop(string pathToImage, string pathToCsvMask, List<char> symbols, int CROPPING_MODE)
        {
            List<BitmapImage> newImages = new List<BitmapImage>();
            Bitmap sourceImage = new Bitmap(pathToImage);
            int width = sourceImage.Width;
            int height = sourceImage.Height;
            Color backgroundColor = UserData.BackgroundColor;

            Bitmap newImage1;
            Bitmap newImage2;
            if (CROPPING_MODE == UserData.CROP_ALL_EXCEPT_SYMBOL)
            {
                newImage1 = new Bitmap(width, height);
                newImage2 = new Bitmap(width, height);
                using (var g = Graphics.FromImage(newImage1))
                    g.Clear(backgroundColor);
                using (var g = Graphics.FromImage(newImage2))
                    g.Clear(backgroundColor);
            }
            else
            {
                newImage1 = new Bitmap(pathToImage);
                newImage2 = new Bitmap(pathToImage);
            }

            char[,] mask = ReadMaskFromCSV(pathToCsvMask, width, height);

            // Lockes Bitmap pixels to change them faster
            LockBitmap SourceLockedImage = new LockBitmap(sourceImage);
            SourceLockedImage.LockBits();
            LockBitmap NewlockedImage1 = new LockBitmap(newImage1);
            NewlockedImage1.LockBits();
            LockBitmap NewLockedImage2 = new LockBitmap(newImage2);
            NewLockedImage2.LockBits();

            // Compares pixels to mask and changes them by cropping mode
            if (CROPPING_MODE == UserData.CROP_ALL_EXCEPT_SYMBOL)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (mask[x, y] == symbols[0])
                            NewlockedImage1.SetPixel(x, y, SourceLockedImage.GetPixel(x, y));

                        if (mask[x, y] == symbols[1])
                            NewLockedImage2.SetPixel(x, y, SourceLockedImage.GetPixel(x, y));
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (mask[x, y] == symbols[0])
                            NewlockedImage1.SetPixel(x, y, backgroundColor);

                        if (mask[x, y] == symbols[1])
                            NewLockedImage2.SetPixel(x, y, backgroundColor);
                    }
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
            MemoryStream memory = new MemoryStream();

            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();
            bitmap.Dispose();
            memory.Dispose();

            return bitmapimage;
        }

        private byte[] bitmapToByte(Bitmap img)
        {
            ImageConverter converter = new ImageConverter();
            byte[] newImg = (byte[])converter.ConvertTo(img, typeof(byte[]));
            img.Dispose();
            return newImg;
        }

        private Bitmap byteToBitmap(byte[] img)
        {
            Bitmap image;
            using (MemoryStream ms = new MemoryStream(img))
                image = new Bitmap(ms);
            return image;
        }
    }
}
