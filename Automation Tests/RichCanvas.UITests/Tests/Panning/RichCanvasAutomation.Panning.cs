using System;
using System.Drawing;

using RichCanvas.Gestures;
using RichCanvas.UITests.Helpers;
using RichCanvas.UITests.Tests.Scrolling;

using RichCanvasUITests.App.Automation;

namespace RichCanvas.UITests
{
    public partial class RichCanvasAutomation
    {
        public void Pan(Point fromPoint, Point toPoint)
        {
            Input.WithGesture(RichCanvasGestures.Pan).Drag(fromPoint, toPoint);
        }

        public void PanItemOutsideViewport(RichItemContainerAutomation itemContainer, Direction direction, int outsideDistance, System.Windows.Size visualViewportSize)
        {
            Point panningStartPoint = direction switch
            {
                Direction.Right => new Point(itemContainer.BoundingRectangle.Right, itemContainer.BoundingRectangle.Top),
                Direction.Left => new Point(itemContainer.BoundingRectangle.Right, itemContainer.BoundingRectangle.Top),
                Direction.Up => new Point(itemContainer.BoundingRectangle.Left, itemContainer.BoundingRectangle.Top),
                Direction.Down => new Point(itemContainer.BoundingRectangle.Left, itemContainer.BoundingRectangle.Bottom),
                _ => throw new NotImplementedException()
            };
            Point outsideViewportPoint = direction switch
            {
                Direction.Right => new Point((int)visualViewportSize.Width + outsideDistance, itemContainer.BoundingRectangle.Top),
                Direction.Left => new Point(itemContainer.BoundingRectangle.Left - outsideDistance, itemContainer.BoundingRectangle.Top),
                Direction.Up => new Point(itemContainer.BoundingRectangle.Left, -outsideDistance),
                Direction.Down => new Point(itemContainer.BoundingRectangle.Left, (int)visualViewportSize.Height + outsideDistance),
                _ => throw new NotImplementedException()
            };
            Pan(panningStartPoint, outsideViewportPoint.ToCanvasDrawingPoint());
        }

        public void ResetViewportLocation() => ParentWindow.InvokeButton(AutomationIds.ResetViewportLocationButtonId);
    }
}
