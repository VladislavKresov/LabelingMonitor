using LabelingMonitor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                ViewModel.AddEffect(Effects_Cmb.SelectedIndex);
            }
            if (sender.Equals(Undo_BTN))
            {
                ViewModel.UndoLastEffect();
            }
            if (sender.Equals(CreateImages_BTN))
            {
                ViewModel.CreateImagesAsync();
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }    
}
