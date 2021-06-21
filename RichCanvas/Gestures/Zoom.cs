using System.Windows;
using System.Windows.Media;

namespace RichCanvas.Gestures
{
    internal class Zoom
    {
        private readonly ScaleTransform _scaleTransform;
        private readonly TranslateTransform _translateTransform;
        private readonly RichItemsControl _context;

        internal double MinScale => _context.MinScale;
        internal double MaxScale => _context.MaxScale;

        internal Zoom(ScaleTransform scaleTransform, TranslateTransform translateTransform, RichItemsControl context)
        {
            _scaleTransform = scaleTransform;
            _translateTransform = translateTransform;
            _context = context;
        }

        internal void ZoomToPosition(Point position, int delta, double factor)
        {
            var previousScaleX = _scaleTransform.ScaleX;
            var previousScaleY = _scaleTransform.ScaleY;
            var originX = (position.X - _translateTransform.X) / _scaleTransform.ScaleX;
            var originY = (position.Y - _translateTransform.Y) / _scaleTransform.ScaleY;

            if (delta > 0)
            {
                _scaleTransform.ScaleY *= factor;
                _scaleTransform.ScaleX *= factor;
            }
            else
            {
                _scaleTransform.ScaleY /= factor;
                _scaleTransform.ScaleX /= factor;
            }
            if (_scaleTransform.ScaleX <= MinScale)
            {
                _scaleTransform.ScaleX = MinScale;
            }
            if (_scaleTransform.ScaleY <= MinScale)
            {
                _scaleTransform.ScaleY = MinScale;
            }
            if (_scaleTransform.ScaleX >= MaxScale)
            {
                _scaleTransform.ScaleX = MaxScale;
            }
            if (_scaleTransform.ScaleY >= MaxScale)
            {
                _scaleTransform.ScaleY = MaxScale;
            }

            if (previousScaleX != _scaleTransform.ScaleX)
            {
                _translateTransform.X = position.X - originX * _scaleTransform.ScaleX;
            }
            if (previousScaleY != _scaleTransform.ScaleY)
            {
                _translateTransform.Y = position.Y - originY * _scaleTransform.ScaleY;
            }
        }

    }
}
