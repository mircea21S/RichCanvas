using RichCanvas;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace RichCanvasDemo.Adorners
{
    public class ResizeAdorner : Adorner
    {
        private readonly VisualCollection _visualCollection;
        private Thumb _topLeftThumb;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visualCollection = new VisualCollection(AdornedElement);
            _topLeftThumb = CreateThumb();
            _topLeftThumb.DragDelta += TopLeftThumbDrag;
            _topLeftThumb.MouseEnter += ThumbMouseOver;
        }

        private void ThumbMouseOver(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void TopLeftThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            var x = e.HorizontalChange;
            var y = e.VerticalChange;
            var container = (RichItemContainer)AdornedElement;

            double width_old = container.Width;
            double width_new = Math.Max(container.Width - x, hitThumb.DesiredSize.Width);
            double left_old = container.Left;
            container.Width = width_new;
            container.Left = left_old - (width_new - width_old);

            double height_old = container.Height;
            double height_new = Math.Max(container.Height - y, hitThumb.DesiredSize.Height);
            double top_old = container.Top;
            container.Height = height_new;
            container.Top = top_old - (height_new - height_old);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var container = (RichItemContainer)AdornedElement;
            _topLeftThumb.Arrange(new Rect(0, 0, 10, 10));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index) => _visualCollection[index];

        protected override int VisualChildrenCount => _visualCollection.Count;

        private Thumb CreateThumb()
        {
            var thumb = new Thumb
            {
                Background = Brushes.DodgerBlue,
                Height = 10,
                Width = 10
            };
            _visualCollection.Add(thumb);
            return thumb;
        }
    }
}
