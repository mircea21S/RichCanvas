using System.Windows;

namespace RichCanvas.UIAutomation.Core.ControlInformations
{
    /// <summary>
    /// Data transfer object exposing <see cref="RichCanvas"/> information to UI Automation proejct.
    /// </summary>
    public class RichCanvasData : ICloneable
    {
        /// <summary>
        /// <see cref="RichCanvas.TranslateTransform"/>.X property value.
        /// </summary>
        public double TranslateTransformX { get; set; }

        /// <summary>
        /// <see cref="RichCanvas.TranslateTransform"/>.Y property value.
        /// </summary>
        public double TranslateTransformY { get; set; }

        /// <summary>
        /// <see cref="RichCanvas.ItemsExtent"/> property value.
        /// </summary>
        public Rect ItemsExtent { get; set; }

        /// <summary>
        /// <see cref="RichCanvas.ScrollFactor"/> property value.
        /// </summary>
        public double ScrollFactor { get; set; }

        /// <summary>
        /// <see cref="RichCanvas.ViewportLocation"/> property value.
        /// </summary>
        public Point ViewportLocation { get; set; }

        /// <summary>
        /// <see cref="RichCanvas.ViewportSize"/> property value.
        /// </summary>
        public Size ViewportSize { get; set; }

        /// <summary>
        /// Size value from <see cref="RichCanvas.ExtentWidth"/> and <see cref="RichCanvas.ExtentHeight"/> properties values.
        /// </summary>
        public Size ViewportExtent { get; set; }

        /// <summary>
        /// <see cref="RichCanvas.ViewportZoom"/> property value.
        /// </summary>
        public double ViewportZoom { get; set; }

        /// <summary>
        /// <see cref="RichCanvas.ScaleFactor"/> property value.
        /// </summary>
        public double ScaleFactor { get; set; }

        /// <summary>
        /// <see cref="RichCanvas.MousePosition"/> property value.
        /// </summary>
        public Point MousePosition { get; set; }

        /// <summary>
        /// <see cref="RichCanvas.MaxScale"/> property value.
        /// </summary>
        public double MaxZoom { get; set; }

        /// <summary>
        /// <see cref="RichCanvas.MinScale"/> property value.
        /// </summary>
        public double MinZoom { get; set; }

        public bool RealTimeDraggingEnabled { get; set; }
        public bool RealTimeSelectionEnabled { get; set; }
        public bool CanSelectMultipleItems { get; set; }

        public object Clone()
        {
            return new RichCanvasData
            {
                ItemsExtent = ItemsExtent,
                ScrollFactor = ScrollFactor,
                MaxZoom = MaxZoom,
                MinZoom = MinZoom,
                TranslateTransformX = TranslateTransformX,
                TranslateTransformY = TranslateTransformY,
                MousePosition = MousePosition,
                ScaleFactor = ScaleFactor,
                ViewportExtent = ViewportExtent,
                ViewportLocation = ViewportLocation,
                ViewportSize = ViewportSize,
                ViewportZoom = ViewportZoom,
                RealTimeDraggingEnabled = RealTimeDraggingEnabled,
                RealTimeSelectionEnabled = RealTimeSelectionEnabled,
                CanSelectMultipleItems = CanSelectMultipleItems
            };
        }
    }
}
