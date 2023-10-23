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
            => new RectangleGeometry(selectionRectangle);

    }
}
