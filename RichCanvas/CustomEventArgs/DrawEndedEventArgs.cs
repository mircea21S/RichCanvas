using System.Windows;

namespace RichCanvas.CustomEventArgs
{
    public class DrawEndedEventArgs
    {
        public object DataContext { get; }

        public Point DrawEndedMousePosition { get; }

        public DrawEndedEventArgs(object dataContext, Point drawEndedMousePosition)
        {
            DataContext = dataContext;
            DrawEndedMousePosition = drawEndedMousePosition;
        }
    }
}
