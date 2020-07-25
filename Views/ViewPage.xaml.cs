using LabelingMonitor.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace LabelingMonitor.Views
{
    /// <summary>
    /// Interaction logic for ViewPage.xaml
    /// </summary>
    public partial class ViewPage : Page
    {
        private ViewPageVM ViewModel;

        public ViewPage()
        {
            ViewModel = ViewPageVM.GetInstance();
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

    }
}
