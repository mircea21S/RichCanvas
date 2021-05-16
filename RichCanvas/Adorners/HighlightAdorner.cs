using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace RichCanvas.Adorners
{
    public class HighlightAdorner : Adorner
    {
        internal ContentPresenter Container { get; set; }

        public HighlightAdorner(UIElement adornedElement) : base(adornedElement) => Container = new ContentPresenter();

        protected override Size MeasureOverride(Size constraint)
        {
            Container.Measure(constraint);
            var container = (RichItemContainer)(AdornedElement);
            //if (container.HasLine)
            //{
                //return line's size(thickness?)
            //}
            return container.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //var contaier = (RichItemContainer)(AdornedElement);
            //if container has line change the adorner arrange
            Container.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index) => Container;

        protected override int VisualChildrenCount => 1;
    }
}
