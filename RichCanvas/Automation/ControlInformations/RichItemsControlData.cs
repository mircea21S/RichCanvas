using System.Windows;

namespace RichCanvas.Automation.ControlInformations
{
    /// <summary>
    /// Data transfer object exposing <see cref="RichCanvas"/> information to UI Automation proejct.
    /// </summary>
    public class RichItemsControlData
    {
        /// <summary>
        /// <see cref="RichCanvas.TranslateTransform"/>.X property value.
        /// </summary>
        public double TranslateTransformX { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvas.TranslateTransform"/>.Y property value.
        /// </summary>
        public double TranslateTransformY { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvas.ItemsExtent"/> property value.
        /// </summary>
        public Rect ItemsExtent { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvas.ScrollFactor"/> property value.
        /// </summary>
        public double ScrollFactor { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvas.ViewportLocation"/> property value.
        /// </summary>
        public Point ViewportLocation { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvas.ViewportSize"/> property value.
        /// </summary>
        public Size ViewportSize { get; internal set; }

        /// <summary>
        /// Size value from <see cref="RichCanvas.ExtentWidth"/> and <see cref="RichCanvas.ExtentHeight"/> properties values.
        /// </summary>
        public Size ViewportExtent { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvas.ViewportZoom"/> property value.
        /// </summary>
        public double ViewportZoom { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvas.ScaleFactor"/> property value.
        /// </summary>
        public double ScaleFactor { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvas.MousePosition"/> property value.
        /// </summary>
        public Point MousePosition { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvas.MaxScale"/> property value.
        /// </summary>
        public double MaxZoom { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvas.MinScale"/> property value.
        /// </summary>
        public double MinZoom { get; internal set; }
    }
}
