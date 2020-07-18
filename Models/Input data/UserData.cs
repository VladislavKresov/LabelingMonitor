using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingMonitor.Models.Input_data
{
    class UserData
    {        
        public static List<string> PathesToImages {get;set;}
        public static List<string> PathesToCsvFiles {get;set;}
        public static List<char> ColorMarkers {get;set;}
        public static Color BackgroundColor {get;set;}
        public static string PathToTxtFile {get;set;}

        public static string GetPathToImage(int index)
        {
            return PathesToImages[index];
        }

        public static string GetPathToCsvFile(int index)
        {
            return PathesToCsvFiles[index];
        }


    }
}
