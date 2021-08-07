using System;
using System.Windows;
using System.Windows.Media;

namespace RichCanvas.Gestures
{
    internal class Drawing
    {
        private readonly RichItemsControl _context;

        internal RichItemContainer CurrentItem { get; private set; }

        public Drawing(RichItemsControl context)
        {
            _context = context;
        }
        internal void OnMouseDown(RichItemContainer container, Point position)
        {
            if (!container.TopPropertySet)
            {
                container.Top = position.Y;
            }
            if (!container.LeftPropertySet)
            {
                container.Left = position.X;
            }
            CurrentItem = container;
        }
        internal void OnMouseMove(Point position)
        {
            ScaleTransform scaleTransform = CurrentItem.ScaleTransform;

            double width = position.X - CurrentItem.Left;
            double height = position.Y - CurrentItem.Top;
            _context.NeedMeasure = false;

            CurrentItem.Width = width == 0 ? 1 : Math.Abs(width);
            CurrentItem.Height = height == 0 ? 1 : Math.Abs(height);

            if (scaleTransform != null)
            {
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
            }

        }

        internal RichItemContainer OnMouseUp()
        {
            CurrentItem.IsDrawn = true;

            SetItemPosition();

            SnapToGrid();

            return CurrentItem;
        }

        internal double GetCurrentTop() => CurrentItem.ScaleTransform?.ScaleY > 0 ? CurrentItem.Top : CurrentItem.Top - CurrentItem.Height;

        internal double GetCurrentLeft() => CurrentItem.ScaleTransform?.ScaleX > 0 ? CurrentItem.Left : CurrentItem.Left - CurrentItem.Width;

        internal double GetCurrentRight() => CurrentItem.ScaleTransform?.ScaleX > 0
                ? CurrentItem.Width + CurrentItem.Left
                : CurrentItem.Left - CurrentItem.Width + CurrentItem.Width;

        internal double GetCurrentBottom() => CurrentItem.ScaleTransform?.ScaleY > 0
                ? CurrentItem.Height + CurrentItem.Top
                : CurrentItem.Top - CurrentItem.Height + CurrentItem.Height;

        internal bool IsMeasureNeeded() => CurrentItem.ScaleTransform?.ScaleX >= 0 && CurrentItem.ScaleTransform?.ScaleY >= 0;

        internal void Dispose() => CurrentItem = null;

        private void SetItemPosition()
        {
            ScaleTransform scaleTransformItem = CurrentItem.ScaleTransform;

            if (scaleTransformItem != null)
            {
                CurrentItem.RenderTransformOrigin = new Point(0.5, 0.5);
                if (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY > 0)
                {
                    CurrentItem.Left -= CurrentItem.Width;
                }
                else if (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY < 0)
                {
                    CurrentItem.Left -= CurrentItem.Width;
                    CurrentItem.Top -= CurrentItem.Height;
                }
                else if (scaleTransformItem.ScaleX > 0 && scaleTransformItem.ScaleY < 0)
                {
                    CurrentItem.Top -= CurrentItem.Height;
                }
                CurrentItem.Scale = new Point(scaleTransformItem.ScaleX, scaleTransformItem.ScaleY);
            }
        }

        private void SnapToGrid()
        {
            if (_context.EnableGrid && _context.EnableSnapping)
            {
                CurrentItem.Left = Math.Round(CurrentItem.Left / _context.GridSpacing) * _context.GridSpacing;
                CurrentItem.Top = Math.Round(CurrentItem.Top / _context.GridSpacing) * _context.GridSpacing;
            }
        }

    }
}
