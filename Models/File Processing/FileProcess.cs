using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LabelingMonitor.Models.Input_data;
using LabelingMonitor.ViewModels;
using static LabelingMonitor.Models.Input_data.UserData;

namespace LabelingMonitor.Models.File_Processing
{
    class FileProcess
    {
        public static List<FramedImage> GetProcessedFramedImages(List<FramedImage> sourceImages, List<int> effects)
        {
            bool needCrop = effects.Contains(EditPageVM.EFFECT_CROP);
            bool needRotate = effects.Contains(EditPageVM.EFFECT_ROTATE);
            if (!needCrop && !needRotate)
                return sourceImages;

            List<FramedImage> processedImages = sourceImages;
            int rotationCount = effects.FindAll(element => element.Equals(EditPageVM.EFFECT_ROTATE)).Count;
            int indent = EditPageVM.IndentToCrop;
            // Processing frames
            for (int i = 0; i < processedImages.Count; i++)
            {
                FramedImage currentImage = processedImages[i];
                var source = new BitmapImage(new Uri(currentImage.source));
                int width = source.PixelWidth;
                int height = source.PixelHeight;
                // Cropping frames
                if (needCrop)
                {
                    currentImage.frames = CropFrames(width, height, indent, currentImage.frames);
                    width -= 2 * indent; 
                    height -= 2 * indent; 
                }
                // Rotating frames
                for (int rotation = 0; rotation < rotationCount; rotation++)
                {
                    currentImage.frames = RotateFrames90(currentImage.frames, width);
                    int temp = width;
                    width = height;
                    height = temp;
                }

                processedImages[i] = currentImage;
            }
            return processedImages;
        }

        private static List<Frame> CropFrames(int width, int height, int indent, List<Frame> frames)
        {
            List<Frame> CroppedFrames = new List<Frame>();            
            foreach (var frame in frames)
            {
                Frame croppedFrame = new Frame();
                // cropping frames
                croppedFrame.TopLeftX = clamp(frame.TopLeftX - indent, 0, width - 2*indent);
                croppedFrame.TopLeftY = clamp(frame.TopLeftY - indent, 0, height - 2*indent);
                croppedFrame.BottomRightX = clamp(frame.BottomRightX - indent, 0, width - 2*indent);
                croppedFrame.BottomRightY = clamp(frame.BottomRightY - indent, 0, height - 2*indent);
                // Validating frame
                if (croppedFrame.TopLeftX < (width - 2 * indent) && croppedFrame.BottomRightX > 0 
                    && croppedFrame.TopLeftY < (height - 2 * indent) && croppedFrame.BottomRightY > 0)
                    CroppedFrames.Add(croppedFrame);
            }
            return CroppedFrames;
        }

        private static List<Frame> RotateFrames90(List<Frame> frames, int width)
        {
            List<Frame> rotatedFrames = new List<Frame>();
            foreach (var frame in frames)
            {
                // Rotating each frame
                Frame RotatedFrame = new Frame();
                RotatedFrame.TopLeftX = frame.TopLeftY;
                RotatedFrame.TopLeftY = width - frame.BottomRightX;
                RotatedFrame.BottomRightX = frame.BottomRightY;
                RotatedFrame.BottomRightY = width - frame.TopLeftX;
                rotatedFrames.Add(RotatedFrame);
            }
            return rotatedFrames;
        }

        public static List<string> ParceFramedImages(List<FramedImage> sourceImages)
        {
            List<string> parcedList = new List<string>();
            foreach (var image in sourceImages)
            {
                string line = image.source;
                foreach (Frame frame in image.frames)
                    line += "," + frame.TopLeftX + "," + frame.TopLeftY + "," + frame.BottomRightX + "," + frame.BottomRightY;
                parcedList.Add(line);
            }
            return parcedList;
        }

        public static char[,] GetProcessedMask(MaskedImage maskedImage, List<int> effects)
        {
            BitmapImage bm = new BitmapImage(new Uri(maskedImage.source));

            int indent = EditPageVM.IndentToCrop;
            char[,] mask = ReadMaskFromCSV(maskedImage.csvMask, bm.PixelWidth, bm.PixelHeight);
            if (!effects.Contains(EditPageVM.EFFECT_CROP) && !effects.Contains(EditPageVM.EFFECT_ROTATE))
                return mask;

            // Cropping mask
            if (effects.Contains(EditPageVM.EFFECT_CROP))
            {
                int width = bm.PixelWidth - 2 * indent;
                int height = bm.PixelHeight - 2 * indent;
                mask = CropMask(width, height, indent, mask);
            }
            // Rotating mask
            int rotationCount = effects.FindAll(element => element.Equals(EditPageVM.EFFECT_ROTATE)).Count;
            for (int i = 0; i < rotationCount; i++)
                mask = RotateMask90(mask);

            return mask;
        }

        private static char[,] CropMask(int width, int height, int indent, char[,] mask)
        {      
            char[,] CropedMask = new char[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    CropedMask[x, y] = mask[x + indent, y + indent];
                }
            }
            return CropedMask;
        }

        private static char[,] RotateMask90(char[,] m)
        {
            var result = new char[m.GetLength(1), m.GetLength(0)];

            for (int y = 0; y < m.GetLength(1); y++)
                for (int x = 0; x < m.GetLength(0); x++)
                    result[y, x] = m[m.GetLength(0) - x - 1, y];

            return result;
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
