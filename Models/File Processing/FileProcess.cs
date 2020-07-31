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
            if (!effects.Contains(EditPageVM.EFFECT_CROP) && !effects.Contains(EditPageVM.EFFECT_ROTATE))
                return sourceImages;

            List<FramedImage> processedImages = sourceImages;
            int rotationCount = effects.FindAll(element => element.Equals(EditPageVM.EFFECT_ROTATE)).Count;
            for (int i = 0; i < processedImages.Count; i++)
            {
                FramedImage currentImage = processedImages[i];
                int height = new BitmapImage(new Uri(currentImage.source)).PixelHeight;
                currentImage.frames = RotateFrames90(processedImages[i].frames, height);
                processedImages[i] = currentImage;
            }
            return processedImages;
        }

        private static List<Frame> RotateFrames90(List<Frame> frames, int height)
        {
            for (int i = 0; i < frames.Count; i++)
            {
                // Rotating each frame
                Frame RotatedFrame = new Frame();
                RotatedFrame.TopLeftX = frames[i].TopLeftY;
                RotatedFrame.TopLeftY = height - frames[i].BottomRightX;
                RotatedFrame.BottomRightX = frames[i].BottomRightY;
                RotatedFrame.BottomRightY = height - frames[i].TopLeftX;
                frames[i] = RotatedFrame;
            }
            return frames;
        }

        public static List<string> ParceFramedImages(List<UserData.FramedImage> sourceImages)
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
            char[,] mask = ReadMaskFromCSV(maskedImage.csvMask, bm.PixelWidth, bm.PixelHeight);
            if (!effects.Contains(EditPageVM.EFFECT_CROP) && !effects.Contains(EditPageVM.EFFECT_ROTATE))
                return mask;
            
            int rotationCount = effects.FindAll(element => element.Equals(EditPageVM.EFFECT_ROTATE)).Count;
            for (int i = 0; i < rotationCount; i++)
                mask = RotateMask90(mask);

            return mask;
        }

        private static char[,] RotateMask90(char[,] m)
        {
            var result = new char[m.GetLength(1), m.GetLength(0)];

            for (int i = 0; i < m.GetLength(1); i++)
                for (int j = 0; j < m.GetLength(0); j++)
                    result[i, j] = m[m.GetLength(0) - j - 1, i];

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
    }
}
