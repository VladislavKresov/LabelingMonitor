using LabelingMonitor.ViewModels;
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
    /// Interaction logic for EditPage.xaml
    /// </summary>
    public partial class EditPage : Page
    {
        private EditPageVM ViewModel;
        public EditPage()
        {
            ViewModel = EditPageVM.GetInstance();
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            if(sender.Equals(MoveBack_BTN))
            {
                ViewModel.MoveBack();
            }
            if (sender.Equals(MoveForward_BTN))
            {
                ViewModel.MoveForward();
            }
            if (sender.Equals(Apply_BTN))
            {

            }
            if (sender.Equals(Cancel_BTN))
            {

            }
            if (sender.Equals(CreateAll_BTN))
            {

            }
        }
    }    
}
