using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace RichCanvasDemo.Adorners
{
    public class LineHoverAdorner : Adorner
    {
        internal ContentPresenter Container { get; set; }

        public LineHoverAdorner(UIElement adornedElement) : base(adornedElement) => Container = new ContentPresenter();

        protected override Size MeasureOverride(Size constraint)
        {
            Container.Measure(constraint);
            return Container.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var line = (System.Windows.Shapes.Line)AdornedElement;
            Container.Arrange(new Rect(new Point(line.X1, line.Y1), line.DesiredSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index) => Container;

        protected override int VisualChildrenCount => 1;
    }
}
