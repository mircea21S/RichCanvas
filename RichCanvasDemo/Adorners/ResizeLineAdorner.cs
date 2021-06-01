using RichCanvas;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RichCanvasDemo.Adorners
{
    public class ResizeLineAdorner : ResizeAdorner
    {
        private readonly VisualCollection _visualCollection;
        private readonly Thumb _topLeftThumb;
        private readonly Thumb _bottomRightThumb;

        public ResizeLineAdorner(UIElement adornedElement) : base(adornedElement)
        {
            Container = new ContentPresenter();
            _visualCollection = new VisualCollection(AdornedElement)
            {
                Container
            };
            _topLeftThumb = CreateThumb();
            _bottomRightThumb = CreateThumb();

            _topLeftThumb.DragDelta += TopLeftThumbDrag;
            _bottomRightThumb.DragDelta += BottomRightThumbDrag;
        }

        private void BottomRightThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            var x = e.HorizontalChange;
            var y = e.VerticalChange;
            var container = (RichItemContainer)AdornedElement;

            double currentHeight = Math.Max(container.Height + y, hitThumb.DesiredSize.Height);
            container.Height = currentHeight;

            double currentWidth = Math.Max(container.Width + x, hitThumb.DesiredSize.Width);
            container.Width = currentWidth;
        }

        private void TopLeftThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            var x = e.HorizontalChange;
            var y = e.VerticalChange;
            var container = (RichItemContainer)AdornedElement;

            double oldWidth = container.Width;
            double currentWidth = Math.Max(container.Width - x, hitThumb.DesiredSize.Width);
            double oldLeft = container.Left;
            container.Width = currentWidth;
            container.Left = oldLeft - (currentWidth - oldWidth);

            double oldHeight = container.Height;
            double currentHeight = Math.Max(container.Height - y, hitThumb.DesiredSize.Height);
            double oldTop = container.Top;
            container.Height = currentHeight;
            container.Top = oldTop - (currentHeight - oldHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var container = (RichItemContainer)AdornedElement;
            _topLeftThumb.Arrange(new Rect(0, 0, 10, 10));
            _bottomRightThumb.Arrange(new Rect(container.Width - 10, container.Height - 10, 10, 10));
            Container.Arrange(new Rect(new Point(container.Width / 2 - 30, container.Height), new Size(60, 20)));
            return finalSize;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            Container.Measure(constraint);
            return Container.DesiredSize;
        }

        protected override Visual GetVisualChild(int index) => _visualCollection[index];

        protected override int VisualChildrenCount => _visualCollection.Count;

        private Thumb CreateThumb()
        {
            var thumb = new Thumb
            {
                Background = Brushes.DodgerBlue,
                Height = 10,
                Width = 10,
                BorderThickness = new Thickness(1, 1, 1, 1),
                BorderBrush = Brushes.DodgerBlue
            };
            _visualCollection.Add(thumb);
            return thumb;
        }
    }
}
