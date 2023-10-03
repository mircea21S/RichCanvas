using RichCanvas.Gestures;
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

        private TranslateTransform? _translateTransform;
        private ScaleTransform? _scaleTransform;
        private Vector _offset;
        private Vector _negativeOffset;
        private Size _extent;
        private Size _viewport;
        private RichItemsControl? _parent;
        private Point _viewportBottomRightInitial;
        private Point _viewportTopLeftInitial = new Point(0, 0);

        private double? HighestElement => _parent?.ItemsHost?.TopElement?.BoundingBox.Top;

        private double? LowestElement => _parent?.ItemsHost?.BottomElement?.BoundingBox.Bottom;

        private double? MostLeftElement => _parent?.ItemsHost?.LeftElement?.BoundingBox.Left;

        private double? MostRightElement => _parent?.ItemsHost?.RightElement?.BoundingBox.Right;

        #endregion

        #region Internal Properties

        internal RichItemsControl Host => _parent;

        /// <summary>
        /// Positive Vertical offset of <see cref="ScrollOwner"/>
        /// </summary>
        public double TopOffset => HighestElement.HasValue && _scaleTransform != null ?
            Math.Abs(TopLimit - HighestElement.Value) * _scaleTransform.ScaleY : double.NaN;

        /// <summary>
        /// Negative Vertical offset of <see cref="ScrollOwner"/>
        /// </summary>
        public double BottomOffset => LowestElement.HasValue && _scaleTransform != null ?
            (BottomLimit - LowestElement.Value) * _scaleTransform.ScaleY : double.NaN;

        /// <summary>
        /// Positive Horizontal offset of <see cref="ScrollOwner"/>
        /// </summary>
        public double LeftOffset => MostLeftElement.HasValue && _scaleTransform != null ?
            Math.Abs(LeftLimit - MostLeftElement.Value) * _scaleTransform.ScaleY : double.NaN;

        /// <summary>
        /// Negative Horizontal offset of <see cref="ScrollOwner"/>
        /// </summary>
        public double RightOffset => MostRightElement.HasValue && _scaleTransform != null ?
            (RightLimit - MostRightElement.Value) * _scaleTransform.ScaleY : double.NaN;

        /// <summary>
        /// Top limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        public double TopLimit => TranslatePoint(_viewportTopLeftInitial, _parent?.ItemsHost).Y;

        /// <summary>
        /// Bottom limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        public double BottomLimit => TranslatePoint(_viewportBottomRightInitial, _parent?.ItemsHost).Y;

        /// <summary> 
        /// Left limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        public double LeftLimit => TranslatePoint(_viewportTopLeftInitial, _parent?.ItemsHost).X;

        /// <summary>
        /// Right limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        public double RightLimit => TranslatePoint(_viewportBottomRightInitial, _parent?.ItemsHost).X;

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
        public ScrollViewer? ScrollOwner { get; set; }

        /// <inheritdoc/>
        public double VerticalOffset => _offset.Y;

        /// <inheritdoc/>
        public double ViewportHeight => _viewport.Height;

        /// <inheritdoc/>
        public double ViewportWidth => _viewport.Width;

        /// <inheritdoc/>
        public void LineDown()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void LineLeft()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void LineRight()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(-_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void LineUp()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(-_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (visual is RichItemContainer container && container.ShouldBringIntoView)
            {
                var containerLocation = new Vector(container.Left, container.Top);
                var viewportCenter = new Vector(ViewportWidth / 2, ViewportHeight / 2);
                if (_parent != null && ScrollOwner != null)
                {
                    var relativePoint = (Point)(containerLocation * _parent.Scale - viewportCenter);
                    if (_translateTransform != null)
                    {
                        _translateTransform.X = -relativePoint.X;
                        _translateTransform.Y = -relativePoint.Y;
                    }
                    container.ShouldBringIntoView = false;
                    return new Rect(ScrollOwner.RenderSize);
                }
            }
            return new Rect(ScrollOwner?.RenderSize ?? Size.Empty);
        }

        /// <inheritdoc/>
        public void MouseWheelDown()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void MouseWheelLeft()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void MouseWheelRight()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(-_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void MouseWheelUp()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(-_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void PageDown()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void PageLeft()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void PageRight()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanHorizontally(-_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void PageUp()
        {
            if (_parent != null && !_parent.IsZooming && !_parent.DisableScroll)
            {
                PanVertically(-_parent.ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void SetHorizontalOffset(double offset)
        {
            if (_parent != null && (Math.Abs(offset) == _parent.ScrollFactor || Math.Abs(offset) == _parent.AutoPanSpeed || _parent.IsPanning))
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
            else if (_parent != null && offset != _parent.ScrollFactor)
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
            if (_parent != null && (Math.Abs(offset) == _parent.ScrollFactor || Math.Abs(offset) == _parent.AutoPanSpeed || _parent.IsPanning))
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
            else if (_parent != null && offset != _parent.ScrollFactor)
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

        //TODO: review this
        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (ScrollOwner != null)
            {
                // If ScrollBar visibility is set initally/on startup ScrollOwner is not initialized
                // so I explictily set them once when ScrollOwner is not null
                var isScrollBarVisibilityInitalized = _parent?.InitializedScrollBarVisiblity ?? true;
                if (!isScrollBarVisibilityInitalized)
                {
                    _parent?.OnScrollBarVisiblityChanged(_parent.VerticalScrollBarVisibility, true, true);
                    _parent?.OnScrollBarVisiblityChanged(_parent.HorizontalScrollBarVisibility, false, true);
                }

                if (_viewport != arrangeSize)
                {
                    _viewport = arrangeSize;
                    _viewportBottomRightInitial = new Point(_viewport.Width, _viewport.Height);
                    _extent.Width = _viewport.Width + _offset.X + Math.Abs(_negativeOffset.X);
                    _extent.Height = _viewport.Height + _offset.Y + Math.Abs(_negativeOffset.Y);
                }

                if (_parent != null && !_parent.IsDragging && !_parent.IsPanning && !_parent.IsZooming && !_parent.DisableScroll)
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
        /// Panning of the canvas using the difference between the specified points.
        /// </summary>
        /// <param name="initialPosition">Calculated initial mouse position. For example, on click.</param>
        /// <param name="currentMousePosition">Current calculated mouse position.</param>
        public void Pan(Point initialPosition, Point currentMousePosition)
        {
            var deltaHeight = currentMousePosition.Y - initialPosition.Y;
            var deltaWidth = currentMousePosition.X - initialPosition.X;

            if (deltaWidth != 0)
            {
                PanHorizontally(-deltaWidth);
            }

            if (deltaHeight != 0)
            {
                PanVertically(-deltaHeight);
            }
            ScrollOwner?.InvalidateScrollInfo();
        }

        /// <summary>
        /// Zooms at the specified position
        /// </summary>
        /// <param name="position">Mouse position to zoom at</param>
        /// <param name="delta">Determines whether to zoom in or out by the sign</param>
        public void Zoom(Point position, double delta)
        {
            //_zoomGesture?.ZoomToPosition(position, delta, _parent?.ScaleFactor);

            // Scrolling limitation
            //if (!_parent.ExtentSize.IsEmpty && HighestElement < TopLimit && Math.Round(TopOffset) > _parent.ExtentSize.Height)
            //{
            //    _zoomGesture?.ZoomToPosition(position, -delta, _parent?.ScaleFactor);
            //}

            //if (!_parent.ExtentSize.IsEmpty && MostLeftElement < LeftLimit && Math.Round(LeftOffset) > _parent.ExtentSize.Width)
            //{
            //    _zoomGesture?.ZoomToPosition(position, -delta, _parent?.ScaleFactor);
            //}

            //if (!_parent.EnableNegativeScrolling && MostRightElement > RightLimit && Math.Abs(Math.Round(RightOffset)) > 0)
            //{
            //    _zoomGesture?.ZoomToPosition(position, -delta, _parent?.ScaleFactor);
            //}
            //else if (!_parent.ExtentSize.IsEmpty && MostRightElement > RightLimit && Math.Abs(Math.Round(RightOffset)) > _parent.ExtentSize.Width)
            //{
            //    _zoomGesture?.ZoomToPosition(position, -delta, _parent?.ScaleFactor);
            //}

            //if (!_parent.EnableNegativeScrolling && LowestElement > BottomLimit && Math.Abs(Math.Round(BottomOffset)) > 0)
            //{
            //    _zoomGesture?.ZoomToPosition(position, -delta, _parent?.ScaleFactor);
            //}
            //else if (!_parent.ExtentSize.IsEmpty && LowestElement > BottomLimit && Math.Abs(Math.Round(BottomOffset)) > _parent.ExtentSize.Height)
            //{
            //    _zoomGesture?.ZoomToPosition(position, -delta, _parent?.ScaleFactor);
            //}

            //if (!_parent.DisableScroll)
            //{
            //    SetCurrentScroll();
            //}
        }

        /// <summary>
        /// Scrolls and translates vertically the <see cref="ScrollOwner"/> viewport
        /// </summary>
        /// <param name="offset">Scroll factor which determines the speed</param>
        public void PanVertically(double offset)
        {
            if (_parent != null && !_parent.DisableScroll)
            {
                // Scrolling limitation
                if (!_parent.EnableNegativeScrolling && offset < 0 && LowestElement >= BottomLimit && Math.Abs(Math.Round(BottomOffset)) >= 0)
                {
                    _translateTransform.Y -= Math.Abs(BottomOffset);
                    CoerceVerticalOffset();
                    CoerceExtentHeight();
                    return;
                }

                if (!_parent.ExtentSize.IsEmpty && HighestElement <= TopLimit && LowestElement >= BottomLimit && Math.Round(TopOffset) >= _parent.ExtentSize.Height
                    && Math.Abs(Math.Round(BottomOffset)) >= _parent.ExtentSize.Height)
                {
                    CoerceVerticalOffset();
                    return;
                }

                if (!_parent.ExtentSize.IsEmpty && offset > 0 && HighestElement <= TopLimit && Math.Round(TopOffset) >= _parent.ExtentSize.Height)
                {
                    _translateTransform.Y += TopOffset - _parent.ExtentSize.Height;
                    CoerceVerticalOffset();
                    return;
                }

                if (!_parent.ExtentSize.IsEmpty && offset < 0 && LowestElement >= BottomLimit && Math.Abs(Math.Round(BottomOffset)) >= _parent.ExtentSize.Height)
                {
                    _translateTransform.Y -= Math.Abs(BottomOffset) - _parent.ExtentSize.Height;
                    CoerceVerticalOffset();
                    return;
                }

                ScrollVertically(offset);
                SetVerticalOffset(offset);
                ScrollOwner?.InvalidateScrollInfo();
            }
            else if (_parent != null && (_parent.IsPanning || !_parent.DisableAutoPanning))
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
            if (_parent != null && !_parent.DisableScroll)
            {
                // Scrolling limitation
                if (!_parent.EnableNegativeScrolling && offset < 0 && MostRightElement >= RightLimit && Math.Abs(RightOffset) >= 0)
                {
                    _translateTransform.X -= Math.Abs(RightOffset);
                    CoerceHorizontalOffset();
                    CoerceExtentWidth();
                    return;
                }

                if (!_parent.ExtentSize.IsEmpty && MostLeftElement <= LeftLimit && MostRightElement >= RightLimit && Math.Round(LeftOffset) >= _parent.ExtentSize.Width
                    && Math.Abs(Math.Round(RightOffset)) >= _parent.ExtentSize.Width)
                {
                    CoerceHorizontalOffset();
                    return;
                }

                if (!_parent.ExtentSize.IsEmpty && offset > 0 && MostLeftElement <= LeftLimit && Math.Round(LeftOffset) >= _parent.ExtentSize.Width)
                {
                    _translateTransform.X += LeftOffset - _parent.ExtentSize.Width;
                    CoerceHorizontalOffset();
                    return;
                }

                if (!_parent.ExtentSize.IsEmpty && offset < 0 && MostRightElement >= RightLimit && Math.Abs(Math.Round(RightOffset)) >= _parent.ExtentSize.Width)
                {
                    _translateTransform.X -= Math.Abs(RightOffset) - _parent.ExtentSize.Width;
                    CoerceHorizontalOffset();
                    return;
                }

                ScrollHorizontally(offset);
                SetHorizontalOffset(offset);
                ScrollOwner?.InvalidateScrollInfo();
            }
            else if (_parent != null && (_parent.IsPanning || !_parent.DisableAutoPanning))
            {
                ScrollHorizontally(offset);
            }
        }

        internal void Initialize(RichItemsControl richItemsControl)
        {
            _parent = richItemsControl;
            _translateTransform = _parent.TranslateTransform;
            _scaleTransform = _parent.ScaleTransform;
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
            CoerceVerticalOffset();

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
            if (_offset.Y < 0)
            {
                _offset.Y = 0;
            }

            if (LowestElement < BottomLimit && _negativeOffset.Y != 0)
            {
                _negativeOffset.Y = 0;
            }

            if (HighestElement > TopLimit && _offset.Y != 0)
            {
                _offset.Y = 0;
            }

            if (HighestElement < TopLimit && _offset.Y != TopOffset)
            {
                _offset.Y = TopOffset;
            }

            if (LowestElement > BottomLimit && _negativeOffset.Y != BottomOffset)
            {
                _negativeOffset.Y = BottomOffset;
            }

            if (!_parent.EnableNegativeScrolling && _negativeOffset.Y < 0)
            {
                _negativeOffset.Y = 0;
            }

            if (!_parent.ExtentSize.IsEmpty && _offset.Y > _parent.ExtentSize.Height)
            {
                _offset.Y = _parent.ExtentSize.Height;
            }

            if (!_parent.ExtentSize.IsEmpty && _negativeOffset.Y < -_parent.ExtentSize.Height)
            {
                _negativeOffset.Y = -_parent.ExtentSize.Height;
            }
        }

        private void CoerceHorizontalOffset()
        {
            if (_offset.X < 0)
            {
                _offset.X = 0;
            }

            if (MostRightElement < RightLimit && _negativeOffset.X != 0)
            {
                _negativeOffset.X = 0;
            }

            if (MostLeftElement > LeftLimit && _offset.X != 0)
            {
                _offset.X = 0;
            }

            if (MostLeftElement < LeftLimit && _offset.X != LeftOffset)
            {
                _offset.X = LeftOffset;
            }

            if (MostRightElement > RightLimit && _negativeOffset.X != RightOffset)
            {
                _negativeOffset.X = RightOffset;
            }

            if (!_parent.EnableNegativeScrolling && _negativeOffset.X < 0)
            {
                _negativeOffset.X = 0;
            }

            if (!_parent.ExtentSize.IsEmpty && _offset.X > _parent.ExtentSize.Width)
            {
                _offset.X = _parent.ExtentSize.Width;
            }

            if (!_parent.ExtentSize.IsEmpty && _negativeOffset.X < -_parent.ExtentSize.Width)
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
            if (!_parent.EnableNegativeScrolling)
            {
                _extent.Height = _viewport.Height + _offset.Y;
            }
        }

        private void CoerceExtentWidth()
        {
            if (_offset.X == 0 && _negativeOffset.X == 0)
            {
                _extent.Width = ViewportWidth;
            }
            if (!_parent.EnableNegativeScrolling)
            {
                _extent.Width = _viewport.Width + _offset.X;
            }
        }

        private void ScrollVertically(double offset)
        {
            _translateTransform.Y += -offset;
        }

        private void ScrollHorizontally(double offset)
        {
            _translateTransform.X += -offset;
        }

        #endregion
    }
}
