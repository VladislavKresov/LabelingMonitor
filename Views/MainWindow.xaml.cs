using LabelingMonitor.ViewModels;
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
            ViewModel = new MainWindowVM();
            InitializeComponent();            
            DataContext = ViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(Next_BTN))
                ViewModel.OnButtonNextClick();
            else
                if (sender.Equals(Previous_BTN))
                ViewModel.OnButtonPreviousClick();
            if (sender.Equals(GoTo_BTN))
            {
                int number;
                if (int.TryParse(GoTo_TxtBlock.Text, out number))
                    ViewModel.GoTo(number);
            }
        }

        private void CmbMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ChangeMode(CmbMode.SelectedIndex);
        }
    }
}
