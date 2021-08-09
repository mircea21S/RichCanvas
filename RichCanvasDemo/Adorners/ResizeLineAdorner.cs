using RichCanvas;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace RichCanvasDemo.Adorners
{
    public class ResizeLineAdorner : ResizeAdorner
    {
        public ResizeLineAdorner(UIElement adornedElement) : base(adornedElement)
        {
            AddTopLeftThumb();
            AddBottomRightThumb();
        }

        protected override void BottomRightThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            double x = e.HorizontalChange;
            double y = e.VerticalChange;

            if (ItemContainer.Scale.Y < 1 && ItemContainer.Scale.X < 1)
            {
                UpdateWidth(-x, hitThumb.DesiredSize.Width);
                UpdateHeight(-y, hitThumb.DesiredSize.Height);
            }
            else if (ItemContainer.Scale.Y < 1)
            {
                UpdateWidth(-x, hitThumb.DesiredSize.Width);
                UpdateHeight(y, hitThumb.DesiredSize.Height);
                UpdateTop(y, hitThumb.DesiredSize.Height);
            }
            else if (ItemContainer.Scale.X < 1)
            {
                UpdateWidth(-x, hitThumb.DesiredSize.Width);
                UpdateHeight(y, hitThumb.DesiredSize.Height);
                UpdateTop(y, hitThumb.DesiredSize.Height);
            }
            else
            {
                UpdateHeight(-y, hitThumb.DesiredSize.Height);
                UpdateWidth(-x, hitThumb.DesiredSize.Width);
            }
        }

        protected override void TopLeftThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            double x = e.HorizontalChange;
            double y = e.VerticalChange;
            if (ItemContainer.Scale.Y < 1 && ItemContainer.Scale.X < 1)
            {
                UpdateWidth(x, hitThumb.DesiredSize.Width);
                UpdateHeight(y, hitThumb.DesiredSize.Width);
                UpdateTop(y, hitThumb.DesiredSize.Width);
                UpdateLeft(x, hitThumb.DesiredSize.Width);
            }
            else if (ItemContainer.Scale.Y < 1)
            {
                UpdateWidth(x, hitThumb.DesiredSize.Width);
                UpdateLeft(x, hitThumb.DesiredSize.Width);
                UpdateHeight(-y, hitThumb.DesiredSize.Height);
            }
            else if (ItemContainer.Scale.X < 1)
            {
                UpdateWidth(x, hitThumb.DesiredSize.Width);
                UpdateLeft(x, hitThumb.DesiredSize.Width);
                UpdateHeight(-y, hitThumb.DesiredSize.Height);
            }
            else
            {
                UpdateWidth(x, hitThumb.DesiredSize.Width);
                UpdateHeight(y, hitThumb.DesiredSize.Width);
                UpdateTop(y, hitThumb.DesiredSize.Width);
                UpdateLeft(x, hitThumb.DesiredSize.Width);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var container = (RichItemContainer)AdornedElement;

            var size = new Size(10, 10);
            if (container.Scale.X < 1)
            {
                _bottomRightThumb.Arrange(new Rect(new Point(0, 0), size));
                _topLeftThumb.Arrange(new Rect(new Point(container.Width - size.Width, container.Height - size.Height), size));
            }
            else
            {
                _topLeftThumb.Arrange(new Rect(new Point(0, 0), size));
                _bottomRightThumb.Arrange(new Rect(new Point(container.Width - size.Width, container.Height - size.Height), size));
            }
            //Arrange by scale
            if (container.Scale.Y < 1)
            {
                Container.Arrange(new Rect(new Point((container.Width / 2) - 30, -20), new Size(60, 20)));
            }
            else
            {
                Container.Arrange(new Rect(new Point((container.Width / 2) - 30, container.Height), new Size(60, 20)));
            }
            return finalSize;
        }

    }
}
