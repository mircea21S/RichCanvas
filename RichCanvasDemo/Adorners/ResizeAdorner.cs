using RichCanvas;
using RichCanvasDemo.ViewModels.Base;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RichCanvasDemo.Adorners
{
    public class ResizeAdorner : Adorner
    {
        private readonly VisualCollection _visualCollection;
        private readonly Thumb _topLeftThumb;
        private readonly Thumb _topRightThumb;
        private readonly Thumb _bottomRightThumb;
        private readonly Thumb _bottomLeftThumb;

        internal ContentPresenter Container { get; set; }

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            Container = new ContentPresenter();
            _visualCollection = new VisualCollection(AdornedElement)
            {
                Container
            };
            _topLeftThumb = CreateThumb();
            _topRightThumb = CreateThumb();
            _bottomRightThumb = CreateThumb();
            _bottomLeftThumb = CreateThumb();

            _topLeftThumb.DragDelta += TopLeftThumbDrag;
            _topRightThumb.DragDelta += TopRightThumbDrag;
            _bottomRightThumb.DragDelta += BottomRightThumbDrag;
            _bottomLeftThumb.DragDelta += BottomLeftThumbDrag;
        }

        private void BottomLeftThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            var x = e.HorizontalChange;
            var y = e.VerticalChange;
            var container = (FrameworkElement)AdornedElement;
            var drawable = (Drawable)container.DataContext;

            double oldWidth = drawable.Width;
            double currentWidth = Math.Max(drawable.Width - x, hitThumb.DesiredSize.Width);
            double oldLeft = drawable.Left;
            drawable.Width = currentWidth;
            drawable.Left = oldLeft - (currentWidth - oldWidth);

            double currentHeight = Math.Max(drawable.Height + y, hitThumb.DesiredSize.Height);
            drawable.Height = currentHeight;
        }

        private void BottomRightThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            var x = e.HorizontalChange;
            var y = e.VerticalChange;
            var container = (FrameworkElement)AdornedElement;
            var drawable = (Drawable)container.DataContext;

            double currentHeight = Math.Max(drawable.Height + y, hitThumb.DesiredSize.Height);
            drawable.Height = currentHeight;

            double currentWidth = Math.Max(drawable.Width + x, hitThumb.DesiredSize.Width);
            drawable.Width = currentWidth;
        }

        private void TopRightThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            var x = e.HorizontalChange;
            var y = e.VerticalChange;
            var container = (FrameworkElement)AdornedElement;
            var drawable = (Drawable)container.DataContext;

            double oldHeight = drawable.Height;
            double currentHeight = Math.Max(drawable.Height - y, hitThumb.DesiredSize.Height);
            double oldTop = drawable.Top;
            drawable.Height = currentHeight;
            drawable.Top = oldTop - (currentHeight - oldHeight);

            double currentWidth = Math.Max(drawable.Width + x, hitThumb.DesiredSize.Width);
            drawable.Width = currentWidth;
        }

        private void TopLeftThumbDrag(object sender, DragDeltaEventArgs e)
        {
            var hitThumb = (Thumb)sender;
            var x = e.HorizontalChange;
            var y = e.VerticalChange;
            var container = (FrameworkElement)AdornedElement;
            var drawable = (Drawable)container.DataContext;

            double oldWidth = drawable.Width;
            double currentWidth = Math.Max(drawable.Width - x, hitThumb.DesiredSize.Width);
            double oldLeft = drawable.Left;
            drawable.Width = currentWidth;
            drawable.Left = oldLeft - (currentWidth - oldWidth);

            double oldHeight = drawable.Height;
            double currentHeight = Math.Max(drawable.Height - y, hitThumb.DesiredSize.Height);
            double oldTop = drawable.Top;
            drawable.Height = currentHeight;
            drawable.Top = oldTop - (currentHeight - oldHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var container = (FrameworkElement)AdornedElement;
            var drawable = (Drawable)container.DataContext;
            _topLeftThumb.Arrange(new Rect(0, 0, 10, 10));
            _topRightThumb.Arrange(new Rect(drawable.Width - 10, 0, 10, 10));
            _bottomLeftThumb.Arrange(new Rect(0, drawable.Height - 10, 10, 10));
            _bottomRightThumb.Arrange(new Rect(drawable.Width - 10, drawable.Height - 10, 10, 10));
            Container.Arrange(new Rect(new Point(drawable.Width / 2 - 30, drawable.Height), new Size(60, 20)));
            return finalSize;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            Container.Measure(constraint);
            return Container.DesiredSize;
        }

        //public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        //{
        //    var container = (RichItemContainer)AdornedElement;
        //    var scale = (ScaleTransform)((TransformGroup)container.RenderTransform).Children[0];

        //    ScaleTransform scaleTrans = new ScaleTransform(1 / scale.ScaleX, 1 / scale.ScaleY);

        //    _topLeftThumb.RenderTransform = scaleTrans;

        //    _topLeftThumb.RenderTransformOrigin = new Point(0.5, 0.5);

        //    //MatrixTransform matrixTrans = transform as MatrixTransform;
        //    //if (matrixTrans != null)
        //    //{
        //    //    Matrix matrix = matrixTrans.Matrix;
        //    //    matrix.M11 = 1;
        //    //    matrix.M22 = 1;
        //    //    matrix.OffsetX -= container.Width;
        //    //    return new MatrixTransform(matrix);
        //    //}

        //    return base.GetDesiredTransform(transform);
        //}

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
