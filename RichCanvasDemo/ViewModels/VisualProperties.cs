using RichCanvasDemo.Common;
using System.Windows.Media;

namespace RichCanvasDemo.ViewModels
{
    public class VisualProperties : ObservableObject
    {
        // #MD ColorPicker bug when Color binded to non-colors??
        private Color _fillColor = Colors.Red;
        private Color _borderColor = Colors.Red;

        public Color FillColor
        {
            get => _fillColor;
            set => SetProperty(ref _fillColor, value);
        }
        public Color BorderColor
        {
            get => _borderColor;
            set => SetProperty(ref _borderColor, value);
        }
    }
}
