using RichCanvasDemo.Helpers;
using RichCanvasDemo.ViewModels.Base;
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

namespace RichCanvasDemo
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
        private void RichItemsControl_OnDrawEnded(object sender, RoutedEventArgs e)
        {
            //e.OriginalSource is DataContext
            //Testing connecting lines
            //TODO: Information about xOy square position (-x,-y) (-x,y) (x,y) (x, -y)

            //var drawable = context as Drawable;
            //(this.DataContext as MainWindowViewModel).DrawConnectedLine(new Point(drawable.Left - drawable.Width, drawable.Top + drawable.Height));

        }

        private void OnScrolling(object sender, RoutedEventArgs e)
        {
            AttachedAdorner.OnScrolling();
        }
    }
}
