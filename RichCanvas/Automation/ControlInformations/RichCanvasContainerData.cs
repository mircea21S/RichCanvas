using System;

namespace RichCanvas.Automation.ControlInformations
{
    /// <summary>
    /// Data transfer object exposing <see cref="RichCanvasContainer"/> information to UI Automation proejct.
    /// </summary>
    public class RichCanvasContainerData
    {
        /// <summary>
        /// Bound <see cref="RichCanvasContainer"/>.DataContext type.
        /// </summary>
        public Type? DataContextType { get; set; }

        /// <summary>
        /// <see cref="RichCanvasContainer.Top"/> property value.
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// <see cref="RichCanvasContainer.Left"/> property value.
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// <see cref="RichCanvasContainer.IsSelected"/> property value.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// <see cref="RichCanvasContainer.Scale"/> property value.
        /// </summary>
        public double ScaleX { get; set; }

        /// <summary>
        /// <see cref="RichCanvasContainer.Scale"/> property value.
        /// </summary>
        public double ScaleY { get; set; }
    }
}
