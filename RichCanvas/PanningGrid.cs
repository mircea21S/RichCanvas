using RichCanvas.Gestures;
using RichCanvas.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace RichCanvas
{
    /// <summary>
    /// Grid defining scrolling functionalty
    /// </summary>
    public class PanningGrid : Grid, IScrollInfo
    {
        #region Private Fields

        private TranslateTransform _translateTransform;
        private ScaleTransform _scaleTransform;
        private Zoom _zoomGesture;
        private Vector _offset;
        private Vector _negativeOffset;
        private Size _extent;
        private Size _viewport;
        private Point _panInitialPosition;
        private RichItemsControl _parent;
        private Point _viewportBottomRightInitial;
        private Point _viewportTopLeftInitial;

        private double HighestElement => _parent.ItemsHost.TopLimit;

        private double LowestElement => _parent.ItemsHost.BottomLimit;

        private double MostLeftElement => _parent.ItemsHost.LeftLimit;

        private double MostRightElement => _parent.ItemsHost.RightLimit;

        #endregion

        #region Internal Properties
        internal bool NegativeVerticalScrollDisabled => !_parent.EnableNegativeScrolling && LowestElement > BottomLimit;
        internal bool NegativeHorizontalScrollDisabled => !_parent.EnableNegativeScrolling && MostRightElement > RightLimit;

        /// <summary>
        /// Positive Vertical offset of <see cref="ScrollOwner"/>
        /// </summary>
        protected double TopOffset => Math.Abs(TopLimit - HighestElement) * _scaleTransform.ScaleY;

        /// <summary>
        /// Negative Vertical offset of <see cref="ScrollOwner"/>
        /// </summary>
        protected double BottomOffset => (BottomLimit - LowestElement) * _scaleTransform.ScaleY;

        /// <summary>
        /// Positive Horizontal offset of <see cref="ScrollOwner"/>
        /// </summary>
        protected double LeftOffset => Math.Abs(LeftLimit - MostLeftElement) * _scaleTransform.ScaleY;

        /// <summary>
        /// Negative Horizontal offset of <see cref="ScrollOwner"/>
        /// </summary>
        protected double RightOffset => (RightLimit - MostRightElement) * _scaleTransform.ScaleY;

        /// <summary>
        /// Top limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        protected double TopLimit => TranslatePoint(_viewportTopLeftInitial, _parent.ItemsHost).Y;

        /// <summary>
        /// Bottom limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        protected double BottomLimit => TranslatePoint(_viewportBottomRightInitial, _parent.ItemsHost).Y;

        /// <summary> 
        /// Left limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        protected double LeftLimit => TranslatePoint(_viewportTopLeftInitial, _parent.ItemsHost).X;

        /// <summary>
        /// Right limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        protected double RightLimit => TranslatePoint(_viewportBottomRightInitial, _parent.ItemsHost).X;

        protected double ExtentHeightLimit => !double.IsInfinity(_parent.ExtentSize.Height) ? (_parent.ExtentSize.Height + ViewportHeight) * _scaleTransform.ScaleY : _parent.ExtentSize.Height;

        protected double ExtentWidthLimit => !double.IsInfinity(_parent.ExtentSize.Width) ? (_parent.ExtentSize.Width + ViewportWidth) * _scaleTransform.ScaleX : _parent.ExtentSize.Width;

        #endregion

        #region IScrollInfo

        /// <inheritdoc/>
        public bool CanHorizontallyScroll { get; set; }

        /// <inheritdoc/>
        public bool CanVerticallyScroll { get; set; }

        /// <inheritdoc/>
        public double ExtentHeight => _extent.Height;

        /// <inheritdoc/>
        public double ExtentWidth => _extent.Width;

        /// <inheritdoc/>
        public double HorizontalOffset => _offset.X;

        /// <inheritdoc/>
        public ScrollViewer ScrollOwner { get; set; }

        /// <inheritdoc/>
        public double VerticalOffset => _offset.Y;

        /// <inheritdoc/>
        public double ViewportHeight => _viewport.Height;

        /// <inheritdoc/>
        public double ViewportWidth => _viewport.Width;

        /// <inheritdoc/>
        public void LineDown()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public void LineLeft()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public void LineRight()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public void LineUp()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (visual is RichItemContainer container)
            {
                var containerLocation = new Point(container.Left, container.Top);
                var viewportCenter = new Vector(ViewportWidth / 2, ViewportHeight / 2);
                var relativePoint = (Point)((Vector)containerLocation * _parent.Scale - viewportCenter);
                _translateTransform.X = -relativePoint.X;
                _translateTransform.Y = -relativePoint.Y;
                container.ShouldBringIntoView = false;
                return new Rect(ScrollOwner.RenderSize);
            }
            return new Rect(ScrollOwner.RenderSize);
        }

        /// <inheritdoc/>
        public void MouseWheelDown()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public void MouseWheelLeft()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public void MouseWheelRight()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public void MouseWheelUp()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public void PageDown()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public void PageLeft()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public void PageRight()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public void PageUp()
        {
            if (!_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(-_parent.ScrollFactor);
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <inheritdoc/>
        public void SetHorizontalOffset(double offset)
        {
            if (Math.Abs(offset) == _parent.ScrollFactor || Math.Abs(offset) == _parent.AutoPanSpeed || _parent.IsPanning)
            {
                if (MostLeftElement < LeftLimit && MostRightElement > RightLimit)
                {
                    _offset.X += offset;
                    _negativeOffset.X += offset;
                }
                else if (MostRightElement > RightLimit)
                {
                    _negativeOffset.X += offset;
                }
                else if (MostLeftElement < LeftLimit)
                {
                    _offset.X += offset;
                }
                CoerceHorizontalOffset();
                _extent.Width = _viewport.Width + _offset.X + Math.Abs(_negativeOffset.X);
                CoerceExtentWidth();
            }
            // Thumb dragging case
            else if (offset != _parent.ScrollFactor)
            {
                double thumbOffset;
                if (_offset.X < 0)
                {
                    thumbOffset = (_offset.X - offset) - _offset.X;
                }
                else
                {
                    thumbOffset = _offset.X - offset;
                }
                _offset.X = offset;
                _negativeOffset.X -= thumbOffset;
                ScrollHorizontally(-thumbOffset);
            }
        }

        /// <inheritdoc/>
        public void SetVerticalOffset(double offset)
        {
            if (Math.Abs(offset) == _parent.ScrollFactor || Math.Abs(offset) == _parent.AutoPanSpeed || _parent.IsPanning)
            {
                if (HighestElement < TopLimit && LowestElement > BottomLimit)
                {
                    _offset.Y += offset;
                    _negativeOffset.Y += offset;
                }
                else if (LowestElement > BottomLimit)
                {
                    _negativeOffset.Y += offset;
                }
                else if (HighestElement < TopLimit)
                {
                    _offset.Y += offset;
                }
                CoerceVerticalOffset();
                _extent.Height = _viewport.Height + _offset.Y + Math.Abs(_negativeOffset.Y);
                CoerceExtentHeight();
            }
            // Thumb dragging case
            else if (offset != _parent.ScrollFactor)
            {
                double thumbOffset;
                if (_offset.Y < 0)
                {
                    thumbOffset = (_offset.Y - offset) - _offset.Y;
                }
                else
                {
                    thumbOffset = _offset.Y - offset;
                }
                _offset.Y = offset;
                _negativeOffset.Y -= thumbOffset;
                ScrollVertically(-thumbOffset);
            }
        }

        #endregion

        #region Override Methods

        /// <inheritdoc/>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Space) && _parent.IsPanning)
            {
                _panInitialPosition = e.GetPosition(this);
                CaptureMouse();
            }
        }

        /// <inheritdoc/>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && _parent.IsPanning)
            {
                var currentPosition = e.GetPosition(this);
                var deltaHeight = currentPosition.Y - _panInitialPosition.Y;
                var deltaWidth = currentPosition.X - _panInitialPosition.X;

                if (deltaWidth != 0)
                {
                    PanHorizontally(-deltaWidth);
                }

                if (deltaHeight != 0)
                {
                    PanVertically(-deltaHeight);
                }
                ScrollOwner.InvalidateScrollInfo();
                _panInitialPosition = currentPosition;
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
        }

        /// <inheritdoc/>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (_parent.IsZooming && !_parent.DisableZoom)
            {
                var position = e.GetPosition(this);
                Zoom(position, e.Delta);
            }
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (ScrollOwner != null)
            {
                if (_viewport != arrangeSize)
                {
                    _viewportTopLeftInitial = new Point(0, 0);
                    _viewport = arrangeSize;
                    _viewportBottomRightInitial = new Point(_viewport.Width, _viewport.Height);
                    _extent.Width = _viewport.Width + _offset.X + Math.Abs(_negativeOffset.X);
                    _extent.Height = _viewport.Height + _offset.Y + Math.Abs(_negativeOffset.Y);
                }

                if (!_parent.IsDragging && !_parent.IsPanning && !_parent.IsZooming && !_parent.DisableScroll)
                {
                    SetCurrentScroll();
                }

                ScrollOwner.InvalidateScrollInfo();
            }
            return base.ArrangeOverride(arrangeSize);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Zooms at the specified position
        /// </summary>
        /// <param name="position">Mouse position to zoom at</param>
        /// <param name="delta">Determines whether to zoom in or out by the sign</param>
        public void Zoom(Point position, int delta)
        {
            _zoomGesture.ZoomToPosition(position, delta, _parent.ScaleFactor);

            SetCurrentScroll();
        }

        /// <summary>
        /// Scrolls and translates vertically the <see cref="ScrollOwner"/> viewport
        /// </summary>
        /// <param name="offset">Scroll factor which determines the speed</param>
        public void PanVertically(double offset)
        {
            if (!_parent.DisableScroll)
            {
                ScrollVertically(offset);
                SetVerticalOffset(offset);
                ScrollOwner.InvalidateScrollInfo();
            }
            else if (_parent.IsPanning || !_parent.DisableAutoPanning)
            {
                ScrollVertically(offset);
            }
        }

        /// <summary>
        /// Scrolls and translates horizontally the <see cref="ScrollOwner"/> viewport
        /// </summary>
        /// <param name="offset">Scroll factor which determines the speed</param>
        public void PanHorizontally(double offset)
        {
            if (!_parent.DisableScroll)
            {
                ScrollHorizontally(offset);
                SetHorizontalOffset(offset);
                ScrollOwner.InvalidateScrollInfo();
            }
            else if (_parent.IsPanning || !_parent.DisableAutoPanning)
            {
                ScrollHorizontally(offset);
            }
        }

        internal void Initalize(RichItemsControl richItemsControl)
        {
            _parent = richItemsControl;
            _translateTransform = _parent.TranslateTransform;
            _scaleTransform = _parent.ScaleTransform;
            _zoomGesture = new Zoom(_scaleTransform, _translateTransform, _parent);
        }

        internal void SetCurrentScroll()
        {
            if (HighestElement < TopLimit && LowestElement > BottomLimit)
            {
                _offset.Y = TopOffset;
                _negativeOffset.Y = BottomOffset;
            }
            else if (HighestElement < TopLimit)
            {
                _offset.Y = TopOffset;
            }
            else if (LowestElement > BottomLimit)
            {
                _negativeOffset.Y = BottomOffset;
            }

            if (MostLeftElement < LeftLimit && MostRightElement > RightLimit)
            {
                _offset.X = LeftOffset;
                _negativeOffset.X = RightOffset;
            }
            else if (MostLeftElement < LeftLimit)
            {
                _offset.X = LeftOffset;
            }
            else if (MostRightElement > RightLimit)
            {
                _negativeOffset.X = RightOffset;
            }
            CoerceVerticalOffset();
            CoerceHorizontalOffset();

            _extent.Height = _viewport.Height + Math.Abs(_negativeOffset.Y) + _offset.Y;
            _extent.Width = _viewport.Width + Math.Abs(_negativeOffset.X) + _offset.X;

            CoerceExtentHeight();
            CoerceExtentWidth();
            ScrollOwner?.InvalidateScrollInfo();
        }

        #endregion

        #region Private Methods

        private void CoerceVerticalOffset()
        {
            if (!_parent.EnableNegativeScrolling || _negativeOffset.Y > 0)
            {
                _negativeOffset.Y = 0;
            }

            if (_offset.Y < 0)
            {
                _offset.Y = 0;
            }

            if (LowestElement < BottomLimit)
            {
                _negativeOffset.Y = 0;
            }

            if (HighestElement > TopLimit)
            {
                _offset.Y = 0;
            }

            if (!double.IsInfinity(ExtentHeightLimit) && _offset.Y > _parent.ExtentSize.Height)
            {
                _offset.Y = _parent.ExtentSize.Height;
            }

            if (!double.IsInfinity(ExtentHeightLimit) && _negativeOffset.Y < -_parent.ExtentSize.Height)
            {
                _negativeOffset.Y = -_parent.ExtentSize.Height;
            }
        }

        private void CoerceHorizontalOffset()
        {
            if (_negativeOffset.X > 0 || !_parent.EnableNegativeScrolling)
            {
                _negativeOffset.X = 0;
            }

            if (_offset.X < 0)
            {
                _offset.X = 0;
            }

            if (MostRightElement < RightLimit)
            {
                _negativeOffset.X = 0;
            }

            if (MostLeftElement > LeftLimit)
            {
                _offset.X = 0;
            }

            if (!double.IsInfinity(ExtentWidthLimit) && _offset.X > _parent.ExtentSize.Width)
            {
                _offset.X = _parent.ExtentSize.Width;
            }

            if (!double.IsInfinity(ExtentWidthLimit) && _negativeOffset.X < -_parent.ExtentSize.Width)
            {
                _negativeOffset.X = -_parent.ExtentSize.Width;
            }
        }

        private void CoerceExtentHeight()
        {
            if (_offset.Y == 0 && _negativeOffset.Y == 0)
            {
                _extent.Height = ViewportHeight;
            }

            if (!double.IsInfinity(ExtentHeightLimit) && _extent.Height >= ExtentHeightLimit)
            {
                _extent.Height = ExtentHeightLimit;
            }
        }

        private void CoerceExtentWidth()
        {
            if (_offset.X == 0 && _negativeOffset.X == 0)
            {
                _extent.Width = ViewportWidth;
            }

            if (!double.IsInfinity(ExtentWidthLimit) && _extent.Width > ExtentWidthLimit)
            {
                _extent.Width = ExtentWidthLimit;
            }
        }

        private void ScrollVertically(double offset)
        {
            if (NegativeVerticalScrollDisabled && offset < 0)
            {
                return;
            }

            if (!double.IsInfinity(ExtentHeightLimit) && (_offset.Y + offset) >= _parent.ExtentSize.Height)
            {
                return;
            }

            _translateTransform.Y += -offset;
        }

        private void ScrollHorizontally(double offset)
        {
            if (NegativeHorizontalScrollDisabled && offset < 0)
            {
                return;
            }

            if (!double.IsInfinity(ExtentWidthLimit) && (_offset.X + offset) >= _parent.ExtentSize.Width)
            {
                return;
            }

            _translateTransform.X += -offset;
        }

        #endregion

    }
}
