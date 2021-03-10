using System;
using System.Windows;
using System.Windows.Input;

namespace RichCanvas.Gestures
{
    internal class Selecting
    {
        private Point _selectionRectangleInitialPosition;

        internal RichItemsControl Context { get; set; }
        internal void OnMouseDown(MouseEventArgs e)
        {
            var position = e.GetPosition(Context.ItemsHost);
            _selectionRectangleInitialPosition = position;
        }
        internal void OnMouseMove(MouseEventArgs e)
        {
            var position = e.GetPosition(Context.ItemsHost);
            double width = position.X - _selectionRectangleInitialPosition.X;
            double height = position.Y - _selectionRectangleInitialPosition.Y;
            var left = _selectionRectangleInitialPosition.X;
            var top = _selectionRectangleInitialPosition.Y;

            if (width < 0)
            {
                left = position.X;
            }

            if (height < 0)
            {
                top = position.Y;
            }
            Context.SelectionRectangle = new Rect(left, top, Math.Abs(width), Math.Abs(height));
        }
    }
}
