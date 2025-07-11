using System;

namespace RichCanvas.Automation.ControlInformations
{
    /// <summary>
    /// Data transfer object exposing <see cref="RichCanvasContainer"/> information to UI Automation proejct.
    /// </summary>
    public class RichItemContainerData
    {
        /// <summary>
        /// Bound <see cref="RichCanvasContainer"/>.DataContext type.
        /// </summary>
        public Type? DataContextType { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvasContainer.Top"/> property value.
        /// </summary>
        public double Top { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvasContainer.Left"/> property value.
        /// </summary>
        public double Left { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvasContainer.IsSelected"/> property value.
        /// </summary>
        public bool IsSelected { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvasContainer.Scale"/> property value.
        /// </summary>
        public double ScaleX { get; internal set; }

        /// <summary>
        /// <see cref="RichCanvasContainer.Scale"/> property value.
        /// </summary>
        public double ScaleY { get; internal set; }
    }
}
