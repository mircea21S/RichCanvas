using System;
using System.Windows;

namespace RichCanvas.Helpers
{
    internal static class SelectionHelper
    {
        internal static Rect DrawSelectionRectangle(Point position, Point initialPosition)
        {
            double left = position.X < initialPosition.X ? position.X : initialPosition.X;
            double top = position.Y < initialPosition.Y ? position.Y : initialPosition.Y;
            double width = Math.Abs(position.X - initialPosition.X);
            double height = Math.Abs(position.Y - initialPosition.Y);

            return new Rect(left, top, width, height);
        }
    }
}
