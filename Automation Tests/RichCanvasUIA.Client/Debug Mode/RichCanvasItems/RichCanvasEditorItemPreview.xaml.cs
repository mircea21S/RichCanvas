using System.Windows;
using System.Windows.Controls;

using RichCanvasUIA.Client.Debug_Mode.Editor;

namespace RichCanvasUIA.Client.Debug_Mode.RichCanvasItems
{
    /// <summary>
    /// Interaction logic for RichCanvasEditorItemPreview.xaml
    /// </summary>
    public partial class RichCanvasEditorItemPreview : UserControl
    {
        public static DependencyProperty RichCanvasEditorControlProperty = DependencyProperty.Register(nameof(RichCanvasEditorControl),
            typeof(RichCanvasEditorControl),
            typeof(RichCanvasEditorItemPreview),
            new FrameworkPropertyMetadata(default(RichCanvasEditorControl)));

        public RichCanvasEditorControl RichCanvasEditorControl
        {
            get => (RichCanvasEditorControl)GetValue(RichCanvasEditorControlProperty);
            set => SetValue(RichCanvasEditorControlProperty, value);
        }

        public RichCanvasEditorItemPreview()
        {
            InitializeComponent();
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
