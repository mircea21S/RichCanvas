using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RichCanvas
{
    public class RichCanvas : Panel
    {
        protected override Size MeasureOverride(Size constraint)
        {
            foreach (UIElement child in Children)
            {
                ((RichItemContainer)child).Measure(constraint);
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
