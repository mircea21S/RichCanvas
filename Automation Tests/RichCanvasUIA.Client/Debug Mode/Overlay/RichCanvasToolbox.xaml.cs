using System.Windows;
using System.Windows.Controls;

using RichCanvasUIA.Client.Debug_Mode.Editor;

namespace RichCanvasUIA.Client.Debug_Mode.Overlay
{
    /// <summary>
    /// Interaction logic for RichCanvasToolbox.xaml
    /// </summary>
    public partial class RichCanvasToolbox : UserControl
    {
        public static DependencyProperty RichCanvasEditorProperty = DependencyProperty.Register(nameof(RichCanvasEditor), typeof(RichCanvasEditor), typeof(RichCanvasToolbox), new FrameworkPropertyMetadata(default(RichCanvasEditor)));

        public RichCanvasEditor RichCanvasEditor
        {
            get => (RichCanvasEditor)GetValue(RichCanvasEditorProperty);
            set => SetValue(RichCanvasEditorProperty, value);
        }

        public RichCanvasToolbox()
        {
            InitializeComponent();
        }
    }
}
