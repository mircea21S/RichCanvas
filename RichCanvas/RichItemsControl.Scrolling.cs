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
        private Point _viewportBottomRightInitial;
        private Point _viewportTopLeftInitial = new Point(0, 0);

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
        }

        /// <inheritdoc/>
        public void SetVerticalOffset(double offset)
        {
        }

        private void UpdateScrollbars()
        {
            if (ScrollOwner != null)
            {
                var extent = ItemsHost.Extent;
                extent.Union(new Rect(ViewportLocation, ViewportSize));

                _extent.Height = extent.Height;
                _extent.Width = extent.Width;

                var scrollOffset = ViewportLocation - ItemsHost.Extent.Location;

                _offset.X = Math.Max(0, scrollOffset.X);
                _offset.Y = Math.Max(0, scrollOffset.Y);
                ScrollOwner.InvalidateScrollInfo();
            }
        }
        #endregion
    }
}
