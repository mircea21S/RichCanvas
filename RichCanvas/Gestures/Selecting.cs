using RichCanvas.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RichCanvas.Gestures
{
    internal class Selecting
    {
        private Point _selectionRectangleInitialPosition;
        private List<RichItemContainer> _selections = new List<RichItemContainer>();

        internal RichItemsControl Context { get; set; }
      
        internal bool HasSelections => _selections.Count > 0;
        public Selecting()
        {
            DragBehavior.DragDelta += OnDragDeltaChanged;
        }

        private void OnDragDeltaChanged(Point point)
        {
            foreach (var item in _selections)
            {
                var transformGroup = (TransformGroup)item.RenderTransform;
                var translateTransform = (TranslateTransform)transformGroup.Children[1];

                translateTransform.X += point.X;
                translateTransform.Y += point.Y;
            }
        }

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
        internal void AddSelection(RichItemContainer container)
        {
            _selections.Add(container);
        }

        internal void UnselectAll()
        {
            foreach (var selection in _selections)
            {
                selection.IsSelected = false;
            }
            _selections.Clear();
        }

        internal void UpdateSelectionsPosition()
        {
            for (int i = 0; i < _selections.Count; i++)
            {
                var transformGroup = (TransformGroup)_selections[i].RenderTransform;
                var translateTransform = (TranslateTransform)transformGroup.Children[1];
                _selections[i].Top += translateTransform.Y;
                _selections[i].Left += translateTransform.X;

                translateTransform.X = 0;
                translateTransform.Y = 0;
            }
            Context.ItemsHost.InvalidateMeasure();
        }
    }
}
