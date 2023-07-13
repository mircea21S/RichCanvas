using System.Windows.Media;
using System.Windows;
using RichCanvas.States.SelectionStates;
using RichCanvas.States.Dragging;

namespace RichCanvas.Helpers
{
    internal static class SelectionHelper
    {
        private static SelectionStrategy? _selectionStrategy;
        private static DraggingStrategy? _draggingStrategy;

        internal static void SetSelectionStrategy(SelectionStrategy selectionStrategy) => _selectionStrategy = selectionStrategy;

        internal static SelectionStrategy? GetSelectionStrategy() => _selectionStrategy;

        internal static void SetDraggingStrategy(DraggingStrategy draggingStrategy) => _draggingStrategy = draggingStrategy;

        internal static DraggingStrategy? GetDraggingStrategy() => _draggingStrategy;

        internal static RectangleGeometry ToRectangleGeometry(this Rect selectionRectangle)
        {
            //var scaleTransform = (ScaleTransform?)transformGroup?.Children[0];
            //if (scaleTransform != null)
            //{
            //    var currentSelectionTop = scaleTransform.ScaleY < 0 ?
            //        selectionRectangle.Top - selectionRectangle.Height : selectionRectangle.Top;
            //    var currentSelectionLeft = scaleTransform.ScaleX < 0 ?
            //        selectionRectangle.Left - selectionRectangle.Width : selectionRectangle.Left;
            return new RectangleGeometry(selectionRectangle);
            //}
            //return new RectangleGeometry(Rect.Empty);
        }

    }
}
