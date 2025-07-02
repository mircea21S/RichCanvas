using System.Windows;

namespace RichCanvas.Automation.ControlInformations
{
    /// <summary>
    /// Data transfer object exposing <see cref="RichItemsControl"/> information to UI Automation proejct.
    /// </summary>
    public class RichItemsControlData
    {
        /// <summary>
        /// <see cref="RichItemsControl.TranslateTransform"/>.X property value.
        /// </summary>
        public double TranslateTransformX { get; internal set; }

        /// <summary>
        /// <see cref="RichItemsControl.TranslateTransform"/>.Y property value.
        /// </summary>
        public double TranslateTransformY { get; internal set; }

        /// <summary>
        /// <see cref="RichItemsControl.ItemsExtent"/> property value.
        /// </summary>
        public Rect ItemsExtent { get; internal set; }

        /// <summary>
        /// <see cref="RichItemsControl.ScrollFactor"/> property value.
        /// </summary>
        public double ScrollFactor { get; internal set; }

        /// <summary>
        /// <see cref="RichItemsControl.ViewportLocation"/> property value.
        /// </summary>
        public Point ViewportLocation { get; internal set; }

        /// <summary>
        /// <see cref="RichItemsControl.ViewportSize"/> property value.
        /// </summary>
        public Size ViewportSize { get; internal set; }

        /// <summary>
        /// Size value from <see cref="RichItemsControl.ExtentWidth"/> and <see cref="RichItemsControl.ExtentHeight"/> properties values.
        /// </summary>
        public Size ViewportExtent { get; internal set; }

        /// <summary>
        /// <see cref="RichItemsControl.ViewportZoom"/> property value.
        /// </summary>
        public double ViewportZoom { get; internal set; }

        /// <summary>
        /// <see cref="RichItemsControl.ScaleFactor"/> property value.
        /// </summary>
        public double ScaleFactor { get; internal set; }

        /// <summary>
        /// <see cref="RichItemsControl.MousePosition"/> property value.
        /// </summary>
        public Point MousePosition { get; internal set; }

        /// <summary>
        /// <see cref="RichItemsControl.MaxScale"/> property value.
        /// </summary>
        public double MaxZoom { get; internal set; }

        /// <summary>
        /// <see cref="RichItemsControl.MinScale"/> property value.
        /// </summary>
        public double MinZoom { get; internal set; }
    }
}
