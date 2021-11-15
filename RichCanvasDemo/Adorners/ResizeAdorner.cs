using RichCanvas;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace RichCanvasDemo.Adorners
{
    public class ResizeAdorner : Adorner
    {
        private readonly VisualCollection _visualCollection;
        protected Thumb _topLeftThumb;
        protected Thumb _topRightThumb;
        protected Thumb _bottomRightThumb;
        protected Thumb _bottomLeftThumb;

        internal ContentPresenter Container { get; set; }

        protected RichItemContainer ItemContainer => (RichItemContainer)AdornedElement;

        protected void AddTopLeftThumb()
        {
            _topLeftThumb = CreateThumb();
            _topLeftThumb.DragDelta += TopLeftThumbDrag;
        }
        protected void AddBottomRightThumb()
        {
            _bottomRightThumb = CreateThumb();
            _bottomRightThumb.DragDelta += BottomRightThumbDrag;
        }
        protected void AddTopRightThumb()
        {
            _topRightThumb = CreateThumb();
            _topRightThumb.DragDelta += TopRightThumbDrag;
        }
        protected void AddBottomLeftThumb()
        {
            _bottomLeftThumb = CreateThumb();
            _bottomLeftThumb.DragDelta += BottomLeftThumbDrag;
        }

        protected void UpdateWidth(double offset, double minWidth)
        {
            double currentWidth = Math.Max(ItemContainer.Width - offset, minWidth);
            ItemContainer.Width = currentWidth;
        }
        protected void UpdateHeight(double offset, double minHeight)
        {
            double currentHeight = Math.Max(ItemContainer.Height - offset, minHeight);
            ItemContainer.Height = currentHeight;
        }
        protected void UpdateLeft(double offset, double minValue)
        {
            double currentWidth = Math.Max(ItemContainer.Width - offset, minValue);
            ItemContainer.Left -= currentWidth - ItemContainer.Width;
        }
        protected void UpdateTop(double offset, double minValue)
        {
            double currentHeight = Math.Max(ItemContainer.Height - offset, minValue);
            ItemContainer.Top -= currentHeight - ItemContainer.Height;
        }

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            Container = new ContentPresenter();
            _visualCollection = new VisualCollection(AdornedElement)
            {
                Container
            };
            AddTopLeftThumb();
            AddBottomLeftThumb();
            AddTopRightThumb();
            AddBottomRightThumb();
        }

        protected virtual void BottomLeftThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            double x = e.HorizontalChange;
            double y = e.VerticalChange;

            UpdateWidth(x, hitThumb.DesiredSize.Width);
            UpdateLeft(x, hitThumb.DesiredSize.Width);
            UpdateHeight(-y, hitThumb.DesiredSize.Height);
        }

        protected virtual void BottomRightThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            double x = e.HorizontalChange;
            double y = e.VerticalChange;

            UpdateHeight(-y, hitThumb.DesiredSize.Height);
            UpdateWidth(-x, hitThumb.DesiredSize.Width);
        }

        protected virtual void TopRightThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            double x = e.HorizontalChange;
            double y = e.VerticalChange;
            UpdateHeight(y, hitThumb.DesiredSize.Height);
            UpdateTop(y, hitThumb.DesiredSize.Height);
            UpdateWidth(-x, hitThumb.DesiredSize.Width);
        }

        protected virtual void TopLeftThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            var p = new Point(e.HorizontalChange, e.VerticalChange);
            double x = p.X;
            double y = p.Y;

            UpdateWidth(x, hitThumb.DesiredSize.Width);
            UpdateLeft(x, hitThumb.DesiredSize.Width);
            UpdateHeight(y, hitThumb.DesiredSize.Height);
            UpdateTop(y, hitThumb.DesiredSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //arrange thumbs according to Transforms
            if (ItemContainer.Scale.X < 1 && ItemContainer.Scale.Y < 1)
            {
                _topLeftThumb.Arrange(new Rect(ItemContainer.Width - 10, ItemContainer.Height - 10, 10, 10));
                _bottomRightThumb.Arrange(new Rect(0, 0, 10, 10));
                _topRightThumb?.Arrange(new Rect(0, ItemContainer.Height - 10, 10, 10));
                _bottomLeftThumb?.Arrange(new Rect(ItemContainer.Width - 10, 0, 10, 10));
            }
            else if (ItemContainer.Scale.X < 1)
            {
                _bottomRightThumb.Arrange(new Rect(0, ItemContainer.Height - 10, 10, 10));
                _topRightThumb?.Arrange(new Rect(0, 0, 10, 10));
                _topLeftThumb.Arrange(new Rect(ItemContainer.Width - 10, 0, 10, 10));
                _bottomLeftThumb?.Arrange(new Rect(ItemContainer.Width - 10, ItemContainer.Height - 10, 10, 10));
            }
            else if (ItemContainer.Scale.Y < 1)
            {
                _topLeftThumb.Arrange(new Rect(0, ItemContainer.Height - 10, 10, 10));
                _bottomLeftThumb?.Arrange(new Rect(0, 0, 10, 10));
                _topRightThumb?.Arrange(new Rect(ItemContainer.Width - 10, ItemContainer.Height - 10, 10, 10));
                _bottomRightThumb.Arrange(new Rect(ItemContainer.Width - 10, 0, 10, 10));
            }
            else
            {
                _topLeftThumb.Arrange(new Rect(0, 0, 10, 10));
                _topRightThumb?.Arrange(new Rect(ItemContainer.Width - 10, 0, 10, 10));
                _bottomLeftThumb?.Arrange(new Rect(0, ItemContainer.Height - 10, 10, 10));
                _bottomRightThumb.Arrange(new Rect(ItemContainer.Width - 10, ItemContainer.Height - 10, 10, 10));
            }
            //Arrange by scale
            if (ItemContainer.Scale.Y < 1)
            {
                Container.Arrange(new Rect(new Point((ItemContainer.Width / 2) - 30, -20), new Size(60, 20)));
            }
            else
            {
                Container.Arrange(new Rect(new Point((ItemContainer.Width / 2) - 30, ItemContainer.Height), new Size(60, 20)));
            }
            return finalSize;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            Container.Measure(constraint);
            return Container.DesiredSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            // invert scale to get correct HorizontalChange and VerticalChange
            var invertedScale = new ScaleTransform(1 / ItemContainer.Scale.X / ItemContainer.Host.Scale, 1 / ItemContainer.Scale.Y / ItemContainer.Host.Scale);

            foreach (Thumb thumb in _visualCollection.OfType<Thumb>())
            {
                thumb.RenderTransform = invertedScale;
                thumb.RenderTransformOrigin = new Point(0.5, 0.5);
            }
            // invert scale on ContentPresenter to display correctly
            Container.RenderTransform = invertedScale;
            Container.RenderTransformOrigin = new Point(0.5, 0.5);

            return base.GetDesiredTransform(transform);
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
