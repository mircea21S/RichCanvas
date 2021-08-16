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
        private readonly RichItemsControl _context;
        private readonly List<RichItemContainer> _selections = new List<RichItemContainer>();

        public Selecting(RichItemsControl context)
        {
            _context = context;
            DragBehavior.DragDelta += OnDragDeltaChanged;
        }

        private void OnDragDeltaChanged(Point point)
        {
            foreach (RichItemContainer item in _selections)
            {
                TranslateTransform translateTransform = item.TranslateTransform;
                if (translateTransform != null)
                {
                    translateTransform.X += point.X;
                    translateTransform.Y += point.Y;
                }
            }
        }

        internal void Update(Point endLocation)
        {
            double width = Math.Abs(endLocation.X - _selectionRectangleInitialPosition.X);
            double height = Math.Abs(endLocation.Y - _selectionRectangleInitialPosition.Y);
            _context.SelectionRectangle = new Rect(_selectionRectangleInitialPosition.X, _selectionRectangleInitialPosition.Y, width, height);
        }

        internal void OnMouseDown(Point position) => _selectionRectangleInitialPosition = position;

        internal void OnMouseMove(Point position)
        {
            TransformGroup transformGroup = _context.SelectionRectangleTransform;
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
            if (!_selections.Contains(container))
            {
                _selections.Add(container);
            }

            if (!_context.SelectedItems.Contains(container.DataContext))
            {
                _context.SelectedItems.Add(container.DataContext);
            }
        }

        internal void RemoveSelection(RichItemContainer container)
        {
            if (_selections.Contains(container))
            {
                _selections.Remove(container);
            }
            if (_context.SelectedItems.Contains(container.DataContext))
            {
                _context.SelectedItems.Remove(container.DataContext);
            }
        }

        internal void UnselectAll() => _selections.Clear();

        internal void UpdateSelectionsPosition(bool snap = false)
        {
            for (int i = 0; i < _selections.Count; i++)
            {
                TranslateTransform translateTransform = _selections[i].TranslateTransform;
                RichItemContainer container = _selections[i];

                if (translateTransform != null)
                {
                    container.Left += translateTransform.X;
                    container.Top += translateTransform.Y;
                    translateTransform.X = 0;
                    translateTransform.Y = 0;
                }

                //TODO
                if (snap)
                {
                    _selections[i].Left = Math.Round(_selections[i].Left / _context.GridSpacing) * _context.GridSpacing;
                    _selections[i].Top = Math.Round(_selections[i].Top / _context.GridSpacing) * _context.GridSpacing;
                }

            }

            _context.NeedMeasure = true;
            _context.ItemsHost.InvalidateMeasure();
        }

    }
}
