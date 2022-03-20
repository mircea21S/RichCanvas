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
        internal double TopLimit { get; set; } = double.PositiveInfinity;
        internal double BottomLimit { get; set; } = double.NegativeInfinity;
        internal double LeftLimit { get; set; } = double.PositiveInfinity;
        internal double RightLimit { get; set; } = double.NegativeInfinity;
        internal RichItemsControl? ItemsOwner { get; set; }
        internal RichItemContainer? BottomElement { get; private set; }
        internal RichItemContainer? RightElement { get; private set; }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size constraint)
        {
            if (ItemsOwner != null && (ItemsOwner.IsDrawing || ItemsOwner.IsSelecting || ItemsOwner.IsDragging))
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

                        if (container.BoundingBox.Bottom > maxY)
                        {
                            BottomElement = container;
                        }
                        if (container.BoundingBox.Right > maxX)
                        {
                            RightElement = container;
                        }
                        minX = Math.Min(minX, container.BoundingBox.Left);
                        minY = Math.Min(minY, container.BoundingBox.Top);
                        maxX = Math.Max(maxX, container.BoundingBox.Right);
                        maxY = Math.Max(maxY, container.BoundingBox.Bottom);
                    }
                }
            }

            TopLimit = minY;
            LeftLimit = minX;
            BottomLimit = maxY;
            RightLimit = maxX;

            if (ItemsOwner != null)
            {
                ItemsOwner.ScrollContainer.SetCurrentScroll();
            }

            return arrangeSize;
        }
    }
}
