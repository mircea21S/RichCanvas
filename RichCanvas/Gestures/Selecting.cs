using RichCanvas.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace RichCanvas.Gestures
{
    internal class Selecting
    {
        private Point _selectionRectangleInitialPosition;
        private List<RichItemContainer> _selections = new List<RichItemContainer>();
        private RichItemsControl _context;

        internal bool HasSelections => _selections.Count > 1;

        public Selecting(RichItemsControl context)
        {
            _context = context;
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

        internal void Update(Point endLocation)
        {
            var width = Math.Abs(endLocation.X - _selectionRectangleInitialPosition.X);
            var height = Math.Abs(endLocation.Y - _selectionRectangleInitialPosition.Y);
            _context.SelectionRectangle = new Rect(_selectionRectangleInitialPosition.X, _selectionRectangleInitialPosition.Y, width, height);
        }

        internal void OnMouseDown(Point position)
        {
            _selectionRectangleInitialPosition = position;
        }

        internal void OnMouseMove(Point position)
        {
            var transformGroup = _context.SelectionRectanlgeTransform;
            var scaleTransform = (ScaleTransform)transformGroup.Children[0];

            double width = position.X - _selectionRectangleInitialPosition.X;
            double height = position.Y - _selectionRectangleInitialPosition.Y;

            if (width < 0 && scaleTransform.ScaleX == 1)
            {
                scaleTransform.ScaleX = -1;
            }

            if (height < 0 && scaleTransform.ScaleY == 1)
            {
                scaleTransform.ScaleY = -1;
            }

            if (height > 0 && scaleTransform.ScaleY == -1)
            {
                scaleTransform.ScaleY = 1;
            }
            if (width > 0 && scaleTransform.ScaleX == -1)
            {
                scaleTransform.ScaleX = 1;
            }
            _context.SelectionRectangle = new Rect(_selectionRectangleInitialPosition.X, _selectionRectangleInitialPosition.Y, Math.Abs(width), Math.Abs(height));
        }

        internal void AddSelection(RichItemContainer container)
        {
            if (container.IsSelectable)
            {
                container.IsSelected = true;
                if (!_selections.Contains(container))
                {
                    _selections.Add(container);
                    if (_context.SelectedItems != null)
                    {
                        _context.SelectedItems.Add(container.DataContext);
                    }
                }
            }
        }

        internal void UnselectAll()
        {
            foreach (var selection in _selections)
            {
                selection.IsSelected = false;
            }
            _selections.Clear();
            if (_context.SelectedItems != null)
            {
                _context.SelectedItems.Clear();
            }
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
            _context.ItemsHost.InvalidateMeasure();
        }

    }
}
