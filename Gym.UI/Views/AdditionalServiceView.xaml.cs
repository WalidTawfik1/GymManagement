using Gym.UI.ViewModels;
using System.Windows.Controls;

namespace Gym.UI.Views
{
    /// <summary>
    /// Interaction logic for AdditionalServiceView.xaml
    /// </summary>
    public partial class AdditionalServiceView : UserControl
    {
        public AdditionalServiceView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is AdditionalServiceViewModel viewModel)
            {
                await viewModel.InitializeAsync();
            }
        }
    }
}