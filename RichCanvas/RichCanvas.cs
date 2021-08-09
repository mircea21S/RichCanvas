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

        protected override Size MeasureOverride(Size constraint)
        {
            if ((ItemsOwner.IsDrawing || ItemsOwner.IsSelecting) && !ItemsOwner.NeedMeasure)
            {
                return default;
            }

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            foreach (UIElement child in InternalChildren)
            {
                var container = (RichItemContainer)child;
                container.Measure(constraint);
                if (container.IsValid())
                {
                    _boundingBoxInitialized = true;
                    minX = Math.Min(minX, container.Left);
                    minY = Math.Min(minY, container.Top);
                    maxX = Math.Max(maxX, container.Left + container.Width);
                    maxY = Math.Max(maxY, container.Top + container.Height);
                }
            }
            if (_boundingBoxInitialized)
            {
                TopLimit = minY;
                LeftLimit = minX;
                BottomLimit = maxY;
                RightLimit = maxX;
                ItemsOwner.ViewportRect = new Rect(LeftLimit, TopLimit, 0, 0);
                ItemsOwner.AdjustScroll();
            }

            return default;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                if (child is RichItemContainer container)
                {
                    child.Arrange(new Rect(new Point(container.Left, container.Top), child.DesiredSize));
                }
            }
            return arrangeSize;
        }
    }
}
