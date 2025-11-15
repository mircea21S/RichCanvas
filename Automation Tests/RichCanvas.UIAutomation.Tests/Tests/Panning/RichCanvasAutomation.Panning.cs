using System;
using System.Drawing;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.Tests.Helpers;
using RichCanvas.UIAutomation.Tests.Tests.Scrolling;

using RichCanvasUIA.Client.Automation;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation
    {
        public void Pan(out System.Windows.Vector distanceVector)
        {
            var panInputGesture = InputMapper.MapToFlaUIInput(RichCanvasGestures.Pan);
            var startPoint = GetRandomPointOnRichCanvas();
            var endPoint = GetRandomPointOnRichCanvas();
            distanceVector = endPoint.AsWindowsPoint() - startPoint.AsWindowsPoint();

            panInputGesture.Drag(startPoint.ToCanvasDrawingPoint(), endPoint.ToCanvasDrawingPoint());
        }

        public void Pan(double x, double y)
            => Patterns.Transform.Pattern.Move(x, y);

        public void PanItemOutsideViewport(RichCanvasContainerAutomation itemContainer, Direction direction, int outsideDistance, System.Windows.Size visualViewportSize)
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
            Input.WithGesture(RichCanvasGestures.Pan).Drag(panningStartPoint, outsideViewportPoint.ToCanvasDrawingPoint());
        }

        public void ResetViewportLocation()
        {
            ParentWindow.InvokeButton(AutomationIds.ResetViewportLocationButtonId);
        }
    }
}
