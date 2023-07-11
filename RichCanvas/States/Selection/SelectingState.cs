using RichCanvas.Gestures;
using RichCanvas.Helpers;
using System;
using System.Windows;
using System.Windows.Input;

namespace RichCanvas.States.SelectionStates
{
    public class SelectingState : CanvasState
    {
        private Point _selectionRectangleInitialPosition;
        private readonly SelectionStrategy? _selectionStrategy = SelectionHelper.GetSelectionStrategy();

        public SelectingState(RichItemsControl parent) : base(parent)
        {
        }

        public override void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (!Parent.SelectionEnabled)
            {
                return;
            }

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

        private void DrawSelectionRectangle(Point position)
        {
            //TransformGroup? transformGroup = Parent.SelectionRectangleTransform;
            //var scaleTransform = (ScaleTransform?)transformGroup?.Children[0];

            double left = position.X < _selectionRectangleInitialPosition.X ? position.X : _selectionRectangleInitialPosition.X;
            double top = position.Y < _selectionRectangleInitialPosition.Y ? position.Y : _selectionRectangleInitialPosition.Y;
            double width = Math.Abs(position.X - _selectionRectangleInitialPosition.X);
            double height = Math.Abs(position.Y - _selectionRectangleInitialPosition.Y);

            Parent.SelectionRectangle = new Rect(left, top, width, height);

            //if (scaleTransform != null)
            //{
            //    if (width < 0 && scaleTransform.ScaleX == 1)
            //    {
            //        scaleTransform.ScaleX = -1;
            //    }

            //    if (height < 0 && scaleTransform.ScaleY == 1)
            //    {
            //        scaleTransform.ScaleY = -1;
            //    }

            //    if (height > 0 && scaleTransform.ScaleY == -1)
            //    {
            //        scaleTransform.ScaleY = 1;
            //    }

            //    if (width > 0 && scaleTransform.ScaleX == -1)
            //    {
            //        scaleTransform.ScaleX = 1;
            //    }
            //}

        }
    }
}
