using System;
using System.Windows.Input;
using System.Windows.Media;

namespace RichCanvas.Gestures
{
    internal class Drawing
    {
        private readonly RichItemsControl _context;
        private RichItemContainer _currentItem;
        internal RichItemContainer CurrentItem => _currentItem;

        public Drawing(RichItemsControl context)
        {
            _context = context;
        }
        internal void OnMouseDown(RichItemContainer container, MouseEventArgs args)
        {
            var position = args.GetPosition(_context.ItemsHost);
            container.Top = position.Y;
            container.Left = position.X;
            _currentItem = container;
        }
        internal void OnMouseMove(MouseEventArgs args)
        {
            var position = args.GetPosition(_context.ItemsHost);
            var transformGroup = (TransformGroup)_currentItem.RenderTransform;
            var scaleTransform = (ScaleTransform)transformGroup.Children[0];

            double width = position.X - _currentItem.Left;
            double height = position.Y - _currentItem.Top;
            _context.NeedMeasure = false;

            _currentItem.Width = Math.Abs(width);
            _currentItem.Height = Math.Abs(height);

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
            _currentItem.IsDrawn = true;

            SetItemPosition();

            _context.ItemsHost.InvalidateMeasure();
            _context.ItemsHost.InvalidateArrange();

            return _currentItem;
        }

        internal double GetCurrentTop()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)_currentItem.RenderTransform).Children[0];

            if (scaleTransformItem.ScaleY > 0)
            {
                return _currentItem.Top;
            }
            else
            {
                return _currentItem.Top - _currentItem.Height;
            }
        }

        internal double GetCurrentLeft()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)_currentItem.RenderTransform).Children[0];

            if (scaleTransformItem.ScaleX > 0)
            {
                return _currentItem.Left;
            }
            else
            {
                return _currentItem.Left - _currentItem.Width;
            }
        }

        internal double GetCurrentRight()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)_currentItem.RenderTransform).Children[0];

            if (scaleTransformItem.ScaleX > 0)
            {
                return _currentItem.Width + _currentItem.Left;
            }
            else
            {
                return (_currentItem.Left - _currentItem.Width) + _currentItem.Width;
            }
        }

        internal double GetCurrentBottom()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)_currentItem.RenderTransform).Children[0];

            if (scaleTransformItem.ScaleY > 0)
            {
                return _currentItem.Height + _currentItem.Top;
            }
            else
            {
                return (_currentItem.Top - _currentItem.Height) + _currentItem.Height;
            }
        }

        internal bool IsMeasureNeeded()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)_currentItem.RenderTransform).Children[0];
            if (scaleTransformItem.ScaleX < 0 || scaleTransformItem.ScaleY < 0)
            {
                return false;
            }
            return true;
        }

        internal void Dispose()
        {
            _currentItem = null;
        }

        private void SetItemPosition()
        {
            var scaleTransformItem = (ScaleTransform)((TransformGroup)_currentItem.RenderTransform).Children[0];
            var translateTransformItem = (TranslateTransform)((TransformGroup)_currentItem.RenderTransform).Children[1];
            if (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY > 0)
            {
                _currentItem.Left -= _currentItem.Width;
                translateTransformItem.X += _currentItem.Width;
            }
            else if (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY < 0)
            {
                _currentItem.Left -= _currentItem.Width;
                _currentItem.Top -= _currentItem.Height;
                scaleTransformItem.ScaleX = 1;
                scaleTransformItem.ScaleY = 1;
            }
            else if (scaleTransformItem.ScaleX > 0 && scaleTransformItem.ScaleY < 0)
            {
                _currentItem.Top -= _currentItem.Height;
                translateTransformItem.Y += _currentItem.Height;
            }

            if (_context.EnableGrid)
            {
                _currentItem.Left = Math.Round(_currentItem.Left / _context.GridSpacing) * _context.GridSpacing;
                _currentItem.Top = Math.Round(_currentItem.Top / _context.GridSpacing) * _context.GridSpacing;
            }
        }

    }
}
