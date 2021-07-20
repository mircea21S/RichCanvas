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
            container.Top = position.Y;
            container.Left = position.X;
            CurrentItem = container;
        }
        internal void OnMouseMove(Point position)
        {
            var transformGroup = (TransformGroup)CurrentItem.RenderTransform;
            var scaleTransform = (ScaleTransform)transformGroup.Children[0];

            double width = position.X - CurrentItem.Left;
            double height = position.Y - CurrentItem.Top;
            _context.NeedMeasure = false;

            CurrentItem.Width = Math.Abs(width);
            CurrentItem.Height = Math.Abs(height);

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

        internal RichItemContainer OnMouseUp()
        {
            CurrentItem.IsDrawn = true;

            SetItemPosition();

            return CurrentItem;
        }

        internal double GetCurrentTop()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)CurrentItem.RenderTransform).Children[0];

            return scaleTransformItem.ScaleY > 0 ? CurrentItem.Top : CurrentItem.Top - CurrentItem.Height;
        }

        internal double GetCurrentLeft()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)CurrentItem.RenderTransform).Children[0];

            return scaleTransformItem.ScaleX > 0 ? CurrentItem.Left : CurrentItem.Left - CurrentItem.Width;
        }

        internal double GetCurrentRight()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)CurrentItem.RenderTransform).Children[0];

            return scaleTransformItem.ScaleX > 0
                ? CurrentItem.Width + CurrentItem.Left
                : CurrentItem.Left - CurrentItem.Width + CurrentItem.Width;
        }

        internal double GetCurrentBottom()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)CurrentItem.RenderTransform).Children[0];

            return scaleTransformItem.ScaleY > 0
                ? CurrentItem.Height + CurrentItem.Top
                : CurrentItem.Top - CurrentItem.Height + CurrentItem.Height;
        }

        internal bool IsMeasureNeeded()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)CurrentItem.RenderTransform).Children[0];
            if (scaleTransformItem.ScaleX < 0 || scaleTransformItem.ScaleY < 0)
            {
                return false;
            }
            return true;
        }

        internal void Dispose()
        {
            CurrentItem = null;
        }

        private void SetItemPosition()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)CurrentItem.RenderTransform).Children[0];
            var translateTransformItem = (TranslateTransform)((TransformGroup)CurrentItem.RenderTransform).Children[1];
            if (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY > 0)
            {
                CurrentItem.Left -= CurrentItem.Width;
                translateTransformItem.X += CurrentItem.Width;
            }
            else if (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY < 0)
            {
                CurrentItem.Left -= CurrentItem.Width;
                CurrentItem.Top -= CurrentItem.Height;
                scaleTransformItem.ScaleX = 1;
                scaleTransformItem.ScaleY = 1;
            }
            else if (scaleTransformItem.ScaleX > 0 && scaleTransformItem.ScaleY < 0)
            {
                CurrentItem.Top -= CurrentItem.Height;
                translateTransformItem.Y += CurrentItem.Height;
            }

            if (_context.EnableGrid && _context.EnableSnapping)
            {
                CurrentItem.Left = Math.Round(CurrentItem.Left / _context.GridSpacing) * _context.GridSpacing;
                CurrentItem.Top = Math.Round(CurrentItem.Top / _context.GridSpacing) * _context.GridSpacing;
            }
        }

    }
}
