using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RichCanvasDemo.CustomControls
{
    /// <summary>
    /// Interaction logic for PropertiesInfo.xaml
    /// </summary>
    public partial class PropertiesInfo : UserControl
    {
        public static DependencyProperty BorderColorProperty = DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(PropertiesInfo), new FrameworkPropertyMetadata(Colors.Transparent));
        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public static DependencyProperty FillColorProperty = DependencyProperty.Register(nameof(FillColor), typeof(Color), typeof(PropertiesInfo), new FrameworkPropertyMetadata(Colors.Transparent));
        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        public PropertiesInfo()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
