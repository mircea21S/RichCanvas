using System;
using System.Windows;
using System.Windows.Media;

namespace RichCanvas.Gestures
{
    internal class Zoom
    {
        private readonly ScaleTransform _scaleTransform;
        private readonly TranslateTransform _translateTransform;

        internal static double ScaleX { get; set; }
        internal static double ScaleY { get; set; }
        public Zoom(ScaleTransform scaleTransform, TranslateTransform translateTransform)
        {
            _scaleTransform = scaleTransform;
            _translateTransform = translateTransform;
        }
        internal void ZoomToPosition(Point position, int delta)
        {
            var originX = (position.X - _translateTransform.X) / _scaleTransform.ScaleX;
            var originY = (position.Y - _translateTransform.Y) / _scaleTransform.ScaleY;

            if (delta > 0)
            {
                _scaleTransform.ScaleY *= 1.1;
                _scaleTransform.ScaleX *= 1.1;
            }
            else
            {
                _scaleTransform.ScaleY /= 1.1;
                _scaleTransform.ScaleX /= 1.1;
            }
            if (_scaleTransform.ScaleX < 0 || _scaleTransform.ScaleX == 0)
            {
                _scaleTransform.ScaleX = 0.1;
            }
            if (_scaleTransform.ScaleY < 0 || _scaleTransform.ScaleY == 0)
            {
                _scaleTransform.ScaleY = 0.1;
            }
            _translateTransform.X = position.X - originX * _scaleTransform.ScaleX;
            _translateTransform.Y = position.Y - originY * _scaleTransform.ScaleY;
        }
        private int Round(double value, int factor)
        {
            return (int)Math.Round(
             (value / factor),
             MidpointRounding.AwayFromZero) * factor;
        }
    }
}
