using LabelingMonitor.Models.Input_data;
using LabelingMonitor.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LabelingMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowVM ViewModel;
        public MainWindow()
        {
            ViewModel = MainWindowVM.GetInstance();
            InitializeComponent();            
            DataContext = ViewModel;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (sender.Equals(OpenImage_Item))
            {
                ofd.Filter = " Images (*.jpg)|*.jpg|(*.png)|*.png";
                if (ofd.ShowDialog() == true)
                    ViewModel.OpenNewImageCollection(ofd.FileNames.ToList<string>());                    
                
            }
            
            if (sender.Equals(OpenMarker_Item))
            {
                if (ViewModel.MarkerType == UserData.MARKER_TYPE_MASK)               
                    ofd.Filter = " Markers (*.csv)|*.csv";
                else                
                    ofd.Filter = " Marker file (*.txt)|*.txt";

                if (ofd.ShowDialog() == true)
                    ViewModel.OpenNewFileCollection(ofd.FileNames.ToList<string>());
            }

            if(sender.Equals(ChangeMarkerType_Item))
            {                
                ViewModel.SwitchMarkerType();
            }      
            
            if(sender.Equals(Clear_Item))
            {
                ViewModel.Clear();
            }
        }

    }
}
