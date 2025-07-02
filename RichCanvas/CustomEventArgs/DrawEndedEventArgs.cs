using System.Windows;

namespace RichCanvas.CustomEventArgs
{
    /// <summary>
    /// Arguments for the <see cref="RichItemsControl.DrawingEnded"/> routed event.
    /// </summary>
    public class DrawEndedEventArgs
    {
        /// <summary>
        /// Gets the DataContext of the last drawn <see cref="RichItemContainer"/>'s Content.
        /// </summary>
        public object DataContext { get; }

        /// <summary>
        /// Gets mouse position where drawing has ended.
        /// </summary>
        public Point DrawEndedMousePosition { get; }

        /// <summary>
        /// Initializes new <see cref="DrawEndedEventArgs"/>.
        /// </summary>
        /// <param name="dataContext">DataContext of the last drawn <see cref="RichItemContainer"/>'s Content</param>
        /// <param name="drawEndedMousePosition">Mouse position where drawing has ended.</param>
        public DrawEndedEventArgs(object dataContext, Point drawEndedMousePosition)
        {
            DataContext = dataContext;
            DrawEndedMousePosition = drawEndedMousePosition;
        }
    }
}
