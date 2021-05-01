using System;
using System.Windows.Input;
using System.Windows.Media;

namespace RichCanvas.Gestures
{
    internal class Drawing
    {
        private readonly RichItemsControl _context;
        private RichItemContainer _currentItem;

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
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;

            double width = position.X - _currentItem.Left;
            double height = position.Y - _currentItem.Top;

            if (width < 0)
            {
                scaleTransform.ScaleX = -1;
            }

            if (height < 0)
            {
                scaleTransform.ScaleY = -1;
            }
            _currentItem.Width = Math.Abs(width);
            _currentItem.Height = Math.Abs(height);
        }


        internal RichItemContainer OnMouseUp()
        {
            _currentItem.IsDrawn = true;

            SetItemPosition();
            _context.ItemsHost.InvalidateArrange();

            return _currentItem;
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
        }

        internal void UpdateCurrentItem(int height = 0, int width = 0)
        {
            _currentItem.Height += height;
            _currentItem.Width += width;
        }

        internal double GetCurrentItemTop()
        {
            return _currentItem.Top;
        }
        internal double GetCurrentItemLeft()
        {
            return _currentItem.Left;
        }
        internal double GetCurrentItemHeight()
        {
            return _currentItem.Height;
        }
        internal double GetCurrentItemWidth()
        {
            return _currentItem.Width;
        }
    }
}
