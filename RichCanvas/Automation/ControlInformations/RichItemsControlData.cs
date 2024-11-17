using System.Windows;

namespace RichCanvas.Automation.ControlInformations
{
    public class RichItemsControlData
    {
        public double TranslateTransformX { get; set; }
        public double TranslateTransformY { get; set; }
        public Rect ItemsExtent { get; set; }
        public double ScrollFactor { get; set; }
        public Point ViewportLocation { get; set; }
        public Size ViewportSize { get; set; }
        public Size ViewportExtent { get; set; }
    }
}
