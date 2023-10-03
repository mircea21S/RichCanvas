using RichCanvas.Helpers;
using System;
using System.Windows;
using System.Windows.Input;

namespace RichCanvas.States.SelectionStates
{
    public class SelectingState : CanvasState
    {
        private Point _selectionRectangleInitialPosition;
        private readonly SelectionStrategy? _selectionStrategy;

        public SelectingState(RichItemsControl parent) : base(parent)
        {
            _selectionStrategy = SelectionHelper.GetSelectionStrategy();
        }

        public override void HandleMouseDown(MouseButtonEventArgs e)
        {
            Parent.SelectionRectangle = new Rect();
            Parent.IsSelecting = true;
            _selectionRectangleInitialPosition = e.GetPosition(Parent.ItemsHost);

            if (Parent.BaseSelectedItems.Count > 0 || Parent.SelectedItem != null)
            {
                Parent.UnselectAll();
            }
        }

        public override void HandleMouseMove(MouseEventArgs e)
        {
            if (!Parent.IsSelecting)
            {
                return;
            }

            DrawSelectionRectangle(e.GetPosition(Parent.ItemsHost));

            if (Parent.RealTimeSelectionEnabled)
            {
                _selectionStrategy?.MouseMoveOnRealTimeSelection();
            }
            _selectionStrategy?.MouseMoveOnDeferredSelection();
        }


        public override void HandleMouseUp(MouseButtonEventArgs e)
        {
            if (!Parent.IsSelecting)
            {
                return;
            }

            if (Parent.RealTimeSelectionEnabled)
            {
                _selectionStrategy?.MouseUpOnRealTimeSelection();
            }
            _selectionStrategy?.MouseUpOnDeferredSelection();

            Parent.IsSelecting = false;
        }

        public override void HandleAutoPanning(Point mousePosition, bool heightChanged = false)
        {
            DrawSelectionRectangle(mousePosition);
        }

        private void DrawSelectionRectangle(Point position)
        {
            double left = position.X < _selectionRectangleInitialPosition.X ? position.X : _selectionRectangleInitialPosition.X;
            double top = position.Y < _selectionRectangleInitialPosition.Y ? position.Y : _selectionRectangleInitialPosition.Y;
            double width = Math.Abs(position.X - _selectionRectangleInitialPosition.X);
            double height = Math.Abs(position.Y - _selectionRectangleInitialPosition.Y);

            Parent.SelectionRectangle = new Rect(left, top, width, height);
        }
    }
}
