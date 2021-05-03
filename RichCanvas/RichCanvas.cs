using System;
using System.Windows;
using System.Windows.Controls;

namespace RichCanvas
{
    public class RichCanvas : Panel
    {
        internal Rect BoundingBox { get; private set; }
        internal RichItemsControl Context { get; set; }
        protected override Size MeasureOverride(Size constraint)
        {
            if (Context.IsDrawing && !Context.NeedMeasure)
            {
                return default;
            }
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            foreach (UIElement child in Children)
            {
                var container = (RichItemContainer)child;
                container.Measure(constraint);
                minX = Math.Min(minX, container.Left);
                minY = Math.Min(minY, container.Top);
                maxX = Math.Max(maxX, container.Left + container.Width);
                maxY = Math.Max(maxY, container.Top + container.Height);
            }
            BoundingBox = new Rect(minX, minY, Math.Abs(maxX), Math.Abs(maxY));
            //Console.WriteLine(BoundingBox.Height + " acum");
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
