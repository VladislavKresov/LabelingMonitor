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
        public static List<UserData.FramedImage> GetProcessedFramedImages(List<UserData.FramedImage> sourceImages, List<int> effects)
        {
            return sourceImages;
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
            return ReadMaskFromCSV(maskedImage.csvMask, bm.PixelWidth, bm.PixelHeight);
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
