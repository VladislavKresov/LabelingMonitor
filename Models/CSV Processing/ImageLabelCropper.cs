using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
