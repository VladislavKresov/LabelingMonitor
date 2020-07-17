using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingMonitor.Models.Input_data
{
    class UserData
    {
        public static List<string> PathesToMasks { get; set; }
        public static List<string> PathesToImages {get;set;}
        public static List<string> PathesToCsvFiles {get;set;}
        public static List<byte> ColorMarkers {get;set;}
        public static byte BackgroundColor {get;set;}
        public static string PathToTxtFile {get;set;}

        public static string GetPathToImage(int index)
        {
            return PathesToImages[index];
        }
    }
}
