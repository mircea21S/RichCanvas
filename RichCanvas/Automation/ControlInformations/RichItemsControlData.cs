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
        public double ViewportZoom { get; set; }
        public double ScaleFactor { get; set; }
        public Point MousePosition { get; set; }
        public double MaxZoom { get; set; }
        public double MinZoom { get; set; }
    }
}
