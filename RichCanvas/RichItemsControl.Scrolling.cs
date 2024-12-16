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
        private Size _extent;
        private Point? _viewportLocationBeforeScrolling;
        private bool _isScrolling;

        #endregion

        #region IScrollInfo

        internal IScrollInfo ScrollInfo => this;
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
        public double ViewportHeight => ViewportSize.Height;

        /// <inheritdoc/>
        public double ViewportWidth => ViewportSize.Width;

        /// <inheritdoc/>
        public void LineDown() => ViewportLocation += new Vector(0, ScrollFactor);

        /// <inheritdoc/>
        public void LineLeft() => ViewportLocation -= new Vector(ScrollFactor, 0);

        /// <inheritdoc/>
        public void LineRight() => ViewportLocation += new Vector(ScrollFactor, 0);

        /// <inheritdoc/>
        public void LineUp() => ViewportLocation -= new Vector(0, ScrollFactor);

        /// <inheritdoc/>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (visual is RichItemContainer container && container.ShouldBringIntoView)
            {
                var containerLocation = new Vector(container.Left, container.Top);
                var viewportCenter = new Vector(ViewportWidth / 2, ViewportHeight / 2);
                if (ScrollOwner != null)
                {
                    var relativePoint = (Point)(containerLocation * ViewportZoom - viewportCenter);
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
        public void MouseWheelDown() => LineDown();

        /// <inheritdoc/>
        public void MouseWheelLeft() => LineLeft();

        /// <inheritdoc/>
        public void MouseWheelRight() => LineRight();

        /// <inheritdoc/>
        public void MouseWheelUp() => LineUp();

        /// <inheritdoc/>
        public void PageDown()
            => ViewportLocation = new Point(ViewportLocation.X, ViewportLocation.Y + ViewportSize.Height);

        /// <inheritdoc/>
        public void PageLeft()
            => ViewportLocation = new Point(ViewportLocation.X - ViewportSize.Width, ViewportLocation.Y);

        /// <inheritdoc/>
        public void PageRight()
            => ViewportLocation = new Point(ViewportLocation.X + ViewportSize.Width, ViewportLocation.Y);

        /// <inheritdoc/>
        public void PageUp()
            => ViewportLocation = new Point(ViewportLocation.X, ViewportLocation.Y - ViewportSize.Height);

        /// <inheritdoc/>
        public void SetHorizontalOffset(double offset)
        {
            _offset.X = offset;
            UpdateViewportLocationOnScroll();
        }

        /// <inheritdoc/>
        public void SetVerticalOffset(double offset)
        {
            _offset.Y = offset;
            UpdateViewportLocationOnScroll();
        }

        private void UpdateViewportLocationOnScroll()
        {
            if (!_viewportLocationBeforeScrolling.HasValue)
            {
                _viewportLocationBeforeScrolling = ViewportLocation;
            }

            _isScrolling = true;

            double locationX = Math.Min(ItemsExtent.Left, _viewportLocationBeforeScrolling.Value.X) + HorizontalOffset;
            double locationY = Math.Min(ItemsExtent.Top, _viewportLocationBeforeScrolling.Value.Y) + VerticalOffset;
            ViewportLocation = new Point(locationX, locationY);
            EnsureExtentIsUpdated();
            ScrollOwner?.InvalidateScrollInfo();
            _isScrolling = false;
        }

        private void EnsureExtentIsUpdated()
        {
            var extentWithItems = ItemsExtent;
            extentWithItems.Union(new Rect(ViewportLocation, ViewportSize));

            var scrollOffset = ViewportLocation - ItemsExtent.Location;

            if (_extent.Height + Math.Max(0, scrollOffset.Y) <= extentWithItems.Height)
            {
                _extent.Height = extentWithItems.Height;
            }

            if (_extent.Width + Math.Max(0, scrollOffset.X) <= extentWithItems.Width)
            {
                _extent.Width = extentWithItems.Width;
            }
        }

        private void UpdateScrollbars()
        {
            // setting the ViewportLocation when manually scrolling triggers the ViewportUpdatedEvent which in turn calls this method, hence the !_isScrolling check
            if (ScrollOwner != null && !_isScrolling)
            {
                _viewportLocationBeforeScrolling = null;

                var extent = ItemsExtent;
                extent.Union(new Rect(ViewportLocation, ViewportSize));

                _extent.Height = extent.Height;
                _extent.Width = extent.Width;

                var scrollOffset = ViewportLocation - ItemsExtent.Location;

                _offset.X = Math.Max(0, scrollOffset.X);
                _offset.Y = Math.Max(0, scrollOffset.Y);
                ScrollOwner.InvalidateScrollInfo();
            }
        }
        #endregion
    }
}
