using RichCanvasUITests.App.TestMocks;
using System.Windows;

namespace RichCanvasUITests.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RichItemContainer_Selected(object sender, RoutedEventArgs e)
        {
        }

        private void RichItemContainer_Unselected(object sender, RoutedEventArgs e)
        {
        }
    }
}
