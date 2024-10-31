using RichCanvas.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RichCanvas
{
    /// <summary>
    /// Grid defining scrolling functionalty
    /// </summary>
    public partial class RichItemsControl : IScrollInfo
    {
        #region Private Fields

        private Vector _offset;
        private Vector _negativeOffset;
        private Size _extent;
        private Size _viewport;
        private Point _viewportBottomRightInitial;
        private Point _viewportTopLeftInitial = new Point(0, 0);

        private double? HighestElement => ItemsHost?.TopElement?.BoundingBox.Top;

        private double? LowestElement => ItemsHost?.BottomElement?.BoundingBox.Bottom;

        private double? MostLeftElement => ItemsHost?.LeftElement?.BoundingBox.Left;

        private double? MostRightElement => ItemsHost?.RightElement?.BoundingBox.Right;

        #endregion

        #region Internal Properties

        /// <summary>
        /// Positive Vertical offset of <see cref="ScrollOwner"/>
        /// </summary>
        public double TopOffset => HighestElement.HasValue && ScaleTransform != null ?
            Math.Abs(TopLimit - HighestElement.Value) * ScaleTransform.ScaleY : double.NaN;

        /// <summary>
        /// Negative Vertical offset of <see cref="ScrollOwner"/>
        /// </summary>
        public double BottomOffset => LowestElement.HasValue && ScaleTransform != null ?
            (BottomLimit - LowestElement.Value) * ScaleTransform.ScaleY : double.NaN;

        /// <summary>
        /// Positive Horizontal offset of <see cref="ScrollOwner"/>
        /// </summary>
        public double LeftOffset => MostLeftElement.HasValue && ScaleTransform != null ?
            Math.Abs(LeftLimit - MostLeftElement.Value) * ScaleTransform.ScaleY : double.NaN;

        /// <summary>
        /// Negative Horizontal offset of <see cref="ScrollOwner"/>
        /// </summary>
        public double RightOffset => MostRightElement.HasValue && ScaleTransform != null ?
            (RightLimit - MostRightElement.Value) * ScaleTransform.ScaleY : double.NaN;

        /// <summary>
        /// Top limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        public double TopLimit => TranslatePoint(_viewportTopLeftInitial, ItemsHost).Y;

        /// <summary>
        /// Bottom limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        public double BottomLimit => TranslatePoint(_viewportBottomRightInitial, ItemsHost).Y;

        /// <summary> 
        /// Left limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        public double LeftLimit => TranslatePoint(_viewportTopLeftInitial, ItemsHost).X;

        /// <summary>
        /// Right limit of <see cref="ScrollOwner"/> viewport
        /// </summary>
        public double RightLimit => TranslatePoint(_viewportBottomRightInitial, ItemsHost).X;

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
            if (!IsZooming && !DisableScroll)
            {
                ScrollVertically(ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void LineLeft()
        {
            if (!IsZooming && !DisableScroll)
            {
                PanHorizontally(ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void LineRight()
        {
            if (!IsZooming && !DisableScroll)
            {
                PanHorizontally(-ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void LineUp()
        {
            if (!IsZooming && !DisableScroll)
            {
                ScrollVertically(-ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (visual is RichItemContainer container && container.ShouldBringIntoView)
            {
                var containerLocation = new Vector(container.Left, container.Top);
                var viewportCenter = new Vector(ViewportWidth / 2, ViewportHeight / 2);
                if (ScrollOwner != null)
                {
                    var relativePoint = (Point)(containerLocation * Scale - viewportCenter);
                    if (TranslateTransform != null)
                    {
                        TranslateTransform.X = -relativePoint.X;
                        TranslateTransform.Y = -relativePoint.Y;
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
            if (!IsZooming && !DisableScroll)
            {
                ScrollVertically(ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void MouseWheelLeft()
        {
            if (!IsZooming && !DisableScroll)
            {
                PanHorizontally(ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void MouseWheelRight()
        {
            if (!IsZooming && !DisableScroll)
            {
                PanHorizontally(-ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void MouseWheelUp()
        {
            if (!IsZooming && !DisableScroll)
            {
                ScrollVertically(-ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void PageDown()
        {
            if (!IsZooming && !DisableScroll)
            {
                ScrollVertically(ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void PageLeft()
        {
            if (!IsZooming && !DisableScroll)
            {
                PanHorizontally(ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void PageRight()
        {
            if (!IsZooming && !DisableScroll)
            {
                PanHorizontally(-ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void PageUp()
        {
            if (!IsZooming && !DisableScroll)
            {
                ScrollVertically(-ScrollFactor);
            }
        }

        /// <inheritdoc/>
        public void SetHorizontalOffset(double offset)
        {
            if ((Math.Abs(offset) == ScrollFactor || Math.Abs(offset) == AutoPanSpeed ||     IsPanning))
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
            else if (offset != ScrollFactor)
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
                //ScrollHorizontally(-thumbOffset);
            }
        }

        /// <inheritdoc/>
        public void SetVerticalOffset(double offset)
        {
            if ((Math.Abs(offset) == ScrollFactor || Math.Abs(offset) == AutoPanSpeed || IsPanning))
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
            else if (offset != ScrollFactor)
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
                var isScrollBarVisibilityInitalized = InitializedScrollBarVisiblity;
                if (!isScrollBarVisibilityInitalized)
                {
                    OnScrollBarVisiblityChanged(VerticalScrollBarVisibility, true, true);
                    OnScrollBarVisiblityChanged(HorizontalScrollBarVisibility, false, true);
                }

                if (_viewport != arrangeSize)
                {
                    _viewport = arrangeSize;
                    _viewportBottomRightInitial = new Point(_viewport.Width, _viewport.Height);
                    _extent.Width = _viewport.Width + _offset.X + Math.Abs(_negativeOffset.X);
                    _extent.Height = _viewport.Height + _offset.Y + Math.Abs(_negativeOffset.Y);
                }

                if (!IsDragging && !IsPanning && !IsZooming && !DisableScroll)
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
        /// <param name="delta">Point represnting vertical change and horizontal change.</param>
        public void Scroll(Point delta)
        {
            if (delta.X != 0)
            {
                PanHorizontally(-delta.X);
            }

            if (delta.Y != 0)
            {
                ScrollVertically(-delta.Y);
            }

            ScrollOwner?.InvalidateScrollInfo();
        }

        /// <summary>
        /// Scrolls and translates vertically the <see cref="ScrollOwner"/> viewport
        /// </summary>
        /// <param name="offset">Scroll factor which determines the speed</param>
        public void ScrollVertically(double offset)
        {
            //TODO: same for PanHorizontally
            if (DisableScroll || ItemsHost.HasTouchedNegativeLimit(new Point(0, offset))
                || ItemsHost.HasTouchedExtentSizeLimit(new Point(0, offset)))
            {
                CoerceVerticalOffset();
                CoerceExtentHeight();
                //TODO: Coerce ViewportLocation
                return;
            }

            // Scrolling limitation
            //if (!_parent.EnableNegativeScrolling && offset < 0 && LowestElement >= BottomLimit && Math.Abs(Math.Round(BottomOffset)) >= 0)
            //{
            //    _translateTransform.Y -= Math.Abs(BottomOffset);
            //    CoerceVerticalOffset();
            //    CoerceExtentHeight();
            //    return;
            //}

            //if (!_parent.ExtentSize.IsEmpty && HighestElement <= TopLimit && LowestElement >= BottomLimit && Math.Round(TopOffset) >= _parent.ExtentSize.Height
            //    && Math.Abs(Math.Round(BottomOffset)) >= _parent.ExtentSize.Height)
            //{
            //    CoerceVerticalOffset();
            //    return;
            //}

            //if (!_parent.ExtentSize.IsEmpty && offset > 0 && HighestElement <= TopLimit && Math.Round(TopOffset) >= _parent.ExtentSize.Height)
            //{
            //    _translateTransform.Y += TopOffset - _parent.ExtentSize.Height;
            //    CoerceVerticalOffset();
            //    return;
            //}

            //if (!_parent.ExtentSize.IsEmpty && offset < 0 && LowestElement >= BottomLimit && Math.Abs(Math.Round(BottomOffset)) >= _parent.ExtentSize.Height)
            //{
            //    _translateTransform.Y -= Math.Abs(BottomOffset) - _parent.ExtentSize.Height;
            //    CoerceVerticalOffset();
            //    return;
            //}

            SetVerticalOffset(offset);
            ScrollOwner?.InvalidateScrollInfo();
        }

        /// <summary>
        /// Scrolls and translates horizontally the <see cref="ScrollOwner"/> viewport
        /// </summary>
        /// <param name="offset">Scroll factor which determines the speed</param>
        public void PanHorizontally(double offset)
        {
            if (!DisableScroll)
            {
                // Scrolling limitation
                if (!EnableNegativeScrolling && offset < 0 && MostRightElement >= RightLimit && Math.Abs(RightOffset) >= 0)
                {
                    TranslateTransform.X -= Math.Abs(RightOffset);
                    CoerceHorizontalOffset();
                    CoerceExtentWidth();
                    return;
                }

                if (!ExtentSize.IsEmpty && MostLeftElement <= LeftLimit && MostRightElement >= RightLimit && Math.Round(LeftOffset) >= ExtentSize.Width
                    && Math.Abs(Math.Round(RightOffset)) >= ExtentSize.Width)
                {
                    CoerceHorizontalOffset();
                    return;
                }

                if (!ExtentSize.IsEmpty && offset > 0 && MostLeftElement <= LeftLimit && Math.Round(LeftOffset) >= ExtentSize.Width)
                {
                    TranslateTransform.X += LeftOffset - ExtentSize.Width;
                    CoerceHorizontalOffset();
                    return;
                }

                if (!ExtentSize.IsEmpty && offset < 0 && MostRightElement >= RightLimit && Math.Abs(Math.Round(RightOffset)) >= ExtentSize.Width)
                {
                    TranslateTransform.X -= Math.Abs(RightOffset) - ExtentSize.Width;
                    CoerceHorizontalOffset();
                    return;
                }

                SetHorizontalOffset(offset);
                ScrollOwner?.InvalidateScrollInfo();
            }
            //else if (_parent != null && (_parent.IsPanning || !_parent.DisableAutoPanning))
            //{
            //    ScrollHorizontally(offset);
            //}
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

            if (!EnableNegativeScrolling && _negativeOffset.Y < 0)
            {
                _negativeOffset.Y = 0;
            }

            if (!ExtentSize.IsEmpty && _offset.Y > ExtentSize.Height)
            {
                _offset.Y = ExtentSize.Height;
            }

            if (!ExtentSize.IsEmpty && _negativeOffset.Y < -ExtentSize.Height)
            {
                _negativeOffset.Y = -ExtentSize.Height;
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

            if (!EnableNegativeScrolling && _negativeOffset.X < 0)
            {
                _negativeOffset.X = 0;
            }

            if (!ExtentSize.IsEmpty && _offset.X > ExtentSize.Width)
            {
                _offset.X = ExtentSize.Width;
            }

            if (!ExtentSize.IsEmpty && _negativeOffset.X < -ExtentSize.Width)
            {
                _negativeOffset.X = -ExtentSize.Width;
            }
        }

        private void CoerceExtentHeight()
        {
            if (_offset.Y == 0 && _negativeOffset.Y == 0)
            {
                _extent.Height = ViewportHeight;
            }
            if (!EnableNegativeScrolling)
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
            if (!EnableNegativeScrolling)
            {
                _extent.Width = _viewport.Width + _offset.X;
            }
        }
        #endregion
    }
}
