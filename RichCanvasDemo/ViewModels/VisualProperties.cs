using RichCanvasDemo.Common;
using System.Windows.Media;

namespace RichCanvasDemo.ViewModels
{
    public class VisualProperties : ObservableObject
    {
        private Color _fillColor = Colors.Gray;
        private Color _borderColor = Colors.Gray;

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
