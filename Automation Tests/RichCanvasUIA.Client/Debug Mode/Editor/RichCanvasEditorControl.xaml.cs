using System.Windows;
using System.Windows.Controls;

using RichCanvas;

namespace RichCanvasUIA.Client.Debug_Mode.Editor
{
    /// <summary>
    /// Interaction logic for RichCanvasEditorControl.xaml
    /// </summary>
    public partial class RichCanvasEditorControl : UserControl
    {
        // make it get-only. Think of adding it directly to RichCanvas library?
        public static DependencyProperty SelectedContainerProperty = DependencyProperty.Register(nameof(SelectedContainer),
            typeof(RichCanvasContainer),
            typeof(RichCanvasEditorControl),
            new FrameworkPropertyMetadata(default(RichCanvasContainer)));

        public RichCanvasContainer SelectedContainer
        {
            get => (RichCanvasContainer)GetValue(SelectedContainerProperty);
            set => SetValue(SelectedContainerProperty, value);
        }

        public RichCanvasEditorControl()
        {
            InitializeComponent();
        }

        private void source_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SelectedContainer = (RichCanvasContainer)source.ItemContainerGenerator.ContainerFromItem(e.AddedItems[0]);
            }
        }
    }
}
