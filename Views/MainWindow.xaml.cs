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

        public const int BUTTON_PREVIOUS = 0;
        public const int BUTTON_NEXT = 1;
        public MainWindow()
        {
            ViewModel = new MainWindowVM();
            InitializeComponent();            
            DataContext = ViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(Next_BTN))
                ViewModel.MoveForward();
            else
                if (sender.Equals(Previous_BTN))
                ViewModel.MoveBack();
            if (sender.Equals(GoTo_BTN))
            {
                int number;
                if (int.TryParse(GoTo_TxtBlock.Text, out number))
                    ViewModel.GoTo(number);
            }
        }

        private void CmbMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ChangeCroppingType(CmbMode.SelectedIndex);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (sender.Equals(OpenImage_Item))
            {
                ofd.Filter = " Images (*.jpg)|*.jpg|(*.png)|*.png";
                if(ofd.ShowDialog() == true)                
                    UserData.PathesToImages = ofd.FileNames.ToList<string>();
                
            }
            
            if (sender.Equals(OpenMarker_Item))
            {
                if (UserData.MarkerType == UserData.MARKER_TYPE_MASK)
                {
                    ofd.Filter = " Markers (*.csv)|*.csv";
                    if (ofd.ShowDialog() == true)
                        UserData.PathesToCsvFiles = ofd.FileNames.ToList<string>();
                }
                else
                {                    
                    ofd.Filter = " Marker file (*.txt)|*.txt";
                    if (ofd.ShowDialog() == true)
                        UserData.PathesToTxtFiles = ofd.FileNames.ToList<string>();
                }
            }

            if(sender.Equals(ChangeMarkerType_Item))
            {
                UserData.SwitchMarkerType();
            }

            ViewModel.UpdateImageInfo();
        }

    }
}
