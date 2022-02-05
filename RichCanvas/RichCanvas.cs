using System;
using System.Windows;
using System.Windows.Controls;

namespace RichCanvas
{
    /// <summary>
    /// ItemsHost of <see cref="RichItemsControl"/>
    /// </summary>
    public class RichCanvas : Panel
    {
        private bool _boundingBoxInitialized;

        internal double TopLimit { get; private set; } = double.PositiveInfinity;
        internal double BottomLimit { get; private set; } = double.NegativeInfinity;
        internal double LeftLimit { get; private set; } = double.PositiveInfinity;
        internal double RightLimit { get; private set; } = double.NegativeInfinity;

        internal RichItemsControl ItemsOwner { get; set; }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size constraint)
        {
            if (ItemsOwner.IsDrawing || ItemsOwner.IsSelecting)
            {
                return default;
            }

            foreach (UIElement child in InternalChildren)
            {
                var container = (RichItemContainer)child;
                container.Measure(constraint);
            }

            return default;
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            foreach (UIElement child in InternalChildren)
            {
                if (child is RichItemContainer container)
                {
                    child.Arrange(new Rect(new Point(container.Left, container.Top), child.DesiredSize));

                    if (container.IsValid())
                    {
                        container.CalculateBoundingBox();

                        _boundingBoxInitialized = true;
                        minX = Math.Min(minX, container.BoundingBox.Left);
                        minY = Math.Min(minY, container.BoundingBox.Top);
                        maxX = Math.Max(maxX, container.BoundingBox.Right);
                        maxY = Math.Max(maxY, container.BoundingBox.Bottom);
                    }
                }
            }

            if (_boundingBoxInitialized)
            {
                TopLimit = minY;
                LeftLimit = minX;
                BottomLimit = maxY;
                RightLimit = maxX;
                ItemsOwner.ViewportRect = new Rect(LeftLimit, TopLimit, 0, 0);
                ItemsOwner.ScrollContainer.SetCurrentScroll();
            }
            return arrangeSize;
        }
    }
}
