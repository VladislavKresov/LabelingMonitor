using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using LabelingMonitor.Models.Input_data;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace LabelingMonitor.Models.Image_Processing
{
    class ImageProcess
    {
        public static byte[] DrawFrames(byte[] image, Rectangle[] rectangles, Color color)
        {
            byte[] newImg;
            using (Bitmap img = ByteToBitmap(image))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawRectangles(new Pen(color, 10f), rectangles);
                    newImg = BitmapToByte(img);
                }
            }
            return newImg;
        }

        public static byte[] Brightness(byte[] image, int percents)
        {
            byte[] photoBytes = image;
            byte[] newImg;
            ISupportedImageFormat format = new JpegFormat { Quality = 70 };
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        imageFactory.Load(inStream)
                                    .Brightness(percents)
                                    .Format(format)
                                    .Save(outStream);
                    }
                    newImg = outStream.ToArray();
                }
            }
            return newImg;
        }

        public static byte[] Shadowing(byte[] image, int percents)
        {
            byte[] photoBytes = image;
            byte[] newImg;

            ISupportedImageFormat format = new JpegFormat { Quality = 70 };
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        imageFactory.Load(inStream)
                                    .Brightness(-percents)
                                    .Format(format)
                                    .Save(outStream);
                    }
                    newImg = outStream.ToArray();
                }
            }
            return newImg;
        }

        public static byte[] Crop(byte[] image, float left, float top, float width, float height)
        {

            byte[] photoBytes = image;
            byte[] newImg;
            ISupportedImageFormat format = new JpegFormat { Quality = 70 };
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        imageFactory.Load(inStream)
                                    .Crop(new CropLayer(left, top, width, height, CropMode.Pixels))
                                    .Format(format)
                                    .Save(outStream);
                    }
                    newImg = outStream.ToArray();
                }
            }
            return newImg;
        }

        public static byte[] Rotate(byte[] image, float angle)
        {
            byte[] photoBytes = image;
            byte[] newImg;
            ISupportedImageFormat format = new JpegFormat { Quality = 70 };
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        imageFactory.Load(inStream)
                                    .Rotate(angle)
                                    .Format(format)
                                    .Save(outStream);
                    }
                    newImg = outStream.ToArray();
                }
            }
            return newImg;
        }

        public static byte[] Quality(byte[] image, int pixelSize)
        {
            byte[] photoBytes = image;
            byte[] newImg;
            ISupportedImageFormat format = new JpegFormat { Quality = 70 };
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        imageFactory.Load(inStream)
                                    .Pixelate(pixelSize)
                                    .Format(format)
                                    .Save(outStream);
                    }
                    newImg = outStream.ToArray();
                }
            }
            return newImg;
        }

        /// <summary>
        /// Croppes two source images by two marker-symbols from .csv file
        /// </summary>
        public static List<BitmapImage> CropMasks(string pathToImage, string pathToCsvMask, List<char> symbols, int CROPPING_MODE)
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
            newImages.Add(ImageProcess.BitmapToBitmapImage(newImage1));
            newImages.Add(ImageProcess.BitmapToBitmapImage(newImage2));

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

        public static byte[] BitmapToByte(Bitmap img)
        {
            ImageConverter converter = new ImageConverter();
            byte[] newImg = (byte[])converter.ConvertTo(img, typeof(byte[]));
            img.Dispose();
            return newImg;
        }

        public static byte[] BitmapImageToByte(BitmapImage image)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        public static Bitmap ByteToBitmap(byte[] img)
        {
            Bitmap image;
            using (MemoryStream ms = new MemoryStream(img))
                image = new Bitmap(ms);
            return image;
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
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

        public static BitmapImage ByteToBitmapImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

    }
}
