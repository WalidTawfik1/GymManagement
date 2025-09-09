using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Gym.UI.Views
{
    /// <summary>
    /// Interaction logic for FinancialDetailsView.xaml
    /// </summary>
    public partial class FinancialDetailsView : UserControl
    {
        public FinancialDetailsView()
        {
            InitializeComponent();
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                var scrollViewer = GetScrollViewer(dataGrid);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3.0);
                    e.Handled = true;
                }
            }
        }

        private ScrollViewer GetScrollViewer(DependencyObject o)
        {
            if (o is ScrollViewer) { return (ScrollViewer)o; }

            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(o, i);
                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
