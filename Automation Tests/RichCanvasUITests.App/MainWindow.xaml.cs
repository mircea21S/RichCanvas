using System.Linq;
using System.Windows;

using RichCanvasUITests.App.Models;

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

        private void source_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var richCanvas = (RichCanvas.RichCanvas)sender;
            var data = (MainWindowViewModel)richCanvas.DataContext;
            System.Collections.ObjectModel.ObservableCollection<RichItemContainerModel> items = data.Items;
            RichItemContainerModel image = items.FirstOrDefault(x => x is ImageModel);
            if (image == null)
            {
                return;
            }
            if (richCanvas.IsPanning)
            {
                image.Top = richCanvas.ViewportLocation.Y;
                image.Left = richCanvas.ViewportLocation.X;
                image.Width = richCanvas.ViewportSize.Width;
                image.Height = richCanvas.ViewportSize.Height;
            }
        }
    }
}
