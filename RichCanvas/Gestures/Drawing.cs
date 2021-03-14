using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace RichCanvas.Gestures
{
    internal class Drawing
    {
        private readonly RichItemsControl _context;
        private readonly ScaleTransform _scaleTransform;
        private readonly List<RichItemContainer> _inView;
        private RichItemContainer _currentItem;
        public Drawing(RichItemsControl context)
        {
            _context = context;
            _scaleTransform = (ScaleTransform)((TransformGroup)_context.AppliedTransform).Children[0];
            _inView = new List<RichItemContainer>();
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

            var scaleTransformItem = (ScaleTransform)((TransformGroup)_currentItem.RenderTransform).Children[0];
            _currentItem.Quadrant = scaleTransformItem.ScaleX > 0 && scaleTransformItem.ScaleY > 0 ? 1 :
                (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY > 0) ? 2 :
                (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY < 0) ? 3 :
                (scaleTransformItem.ScaleX > 0 && scaleTransformItem.ScaleY < 0) ? 4 : 0;

            if (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY > 0)
            {
                _currentItem.Left -= _currentItem.Width;
            }
            if (scaleTransformItem.ScaleX < 0 && scaleTransformItem.ScaleY < 0)
            {
                _currentItem.Left -= _currentItem.Width;
                _currentItem.Top -= _currentItem.Height;
            }
            if (scaleTransformItem.ScaleX > 0 && scaleTransformItem.ScaleY < 0)
            {
                _currentItem.Top -= _currentItem.Height;
            }

            _inView.Add(_currentItem);
            if (_inView.Count > 0)
            {
                _context.BottomLimit = _inView.Select(c => c.Height + c.Top).Max();
                _context.RightLimit = _inView.Select(c => c.Width + c.Left).Max();
                _context.TopLimit = _inView.Select(c => c.Top).Min();
                _context.LeftLimit = _inView.Select(c => c.Left).Min();
            }
            return _currentItem;
        }
    }
}
