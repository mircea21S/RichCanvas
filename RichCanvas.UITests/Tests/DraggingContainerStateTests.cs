using FluentAssertions;
using NUnit.Framework;
using RichCanvas.Gestures;
using RichCanvas.UITests.Helpers;
using RichCanvasUITests.App.Automation;
using RichCanvasUITests.App.TestMocks;
using System.Drawing;

namespace RichCanvas.UITests.Tests
{
    public class DraggingContainerStateTests : RichCanvasTestAppTest
    {
        [Test]
        public void DragContainer_WithRealTimeDraggingEnabled_ShouldUpdateTopLeftPositionWhileDragging()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            Window.InvokeButton(AutomationIds.RealTimeDraggingToggleButtonId);
            var pointOnRectangle = new Point((int)DrawingStateDataMocks.DrawnRectangleMock.Left + 1, (int)DrawingStateDataMocks.DrawnRectangleMock.Top + 1);
            var dragPoint1 = new Point(pointOnRectangle.X + 50, pointOnRectangle.Y + 50);
            var dragPoint2 = new Point(dragPoint1.X + 50, dragPoint1.Y + 50);
            var dragPoint3 = new Point(dragPoint2.X + 50, dragPoint2.Y + 50);

            var canvasPointOnRectangle = pointOnRectangle.ToCanvasDrawingPoint();
            var canvasDragPoint1 = dragPoint1.ToCanvasDrawingPoint();
            var canvasDragPoint2 = dragPoint2.ToCanvasDrawingPoint();
            var canvasDragPoint3 = dragPoint3.ToCanvasDrawingPoint();

            // act
            Input.WithGesture(RichCanvasGestures.Drag).DefferedDrag(canvasPointOnRectangle, [
                (canvasDragPoint1, AssertPosition1UpdatedOnMouseMove),
                (canvasDragPoint2, AssertPosition2UpdatedOnMouseMove),
                (canvasDragPoint3, AssertPosition3UpdatedOnMouseMove),
                ]);

            // assert
            void AssertPosition1UpdatedOnMouseMove()
            {
                var container = RichItemsControl.Items[0];
                container.RichItemContainerData.Top.Should().Be(dragPoint1.Y - 1);
                container.RichItemContainerData.Left.Should().Be(dragPoint1.X - 1);
            }
            void AssertPosition2UpdatedOnMouseMove()
            {
                var container = RichItemsControl.Items[0];
                container.RichItemContainerData.Top.Should().Be(dragPoint2.Y - 1);
                container.RichItemContainerData.Left.Should().Be(dragPoint2.X - 1);
            }
            void AssertPosition3UpdatedOnMouseMove()
            {
                var container = RichItemsControl.Items[0];
                container.RichItemContainerData.Top.Should().Be(dragPoint3.Y - 1);
                container.RichItemContainerData.Left.Should().Be(dragPoint3.X - 1);
            }

            var container = RichItemsControl.Items[0];
            container.RichItemContainerData.Top.Should().Be(dragPoint3.Y - 1);
            container.RichItemContainerData.Left.Should().Be(dragPoint3.X - 1);
        }

        [Test]
        public void DragContainer_WithRealTimeDraggingDisabled_ShouldNotUpdateTopLeftPositionWhileDragging()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            var pointOnRectangle = new Point((int)DrawingStateDataMocks.DrawnRectangleMock.Left + 1, (int)DrawingStateDataMocks.DrawnRectangleMock.Top + 1);
            var dragPoint1 = new Point(pointOnRectangle.X + 50, pointOnRectangle.Y + 50);
            var dragPoint2 = new Point(dragPoint1.X + 50, dragPoint1.Y + 50);
            var dragPoint3 = new Point(dragPoint2.X + 50, dragPoint2.Y + 50);

            var canvasPointOnRectangle = pointOnRectangle.ToCanvasDrawingPoint();
            var canvasDragPoint1 = dragPoint1.ToCanvasDrawingPoint();
            var canvasDragPoint2 = dragPoint2.ToCanvasDrawingPoint();
            var canvasDragPoint3 = dragPoint3.ToCanvasDrawingPoint();

            // act
            Input.WithGesture(RichCanvasGestures.Drag).DefferedDrag(canvasPointOnRectangle, [
                (canvasDragPoint1, AssertPosition1UpdatedOnMouseMove),
                (canvasDragPoint2, AssertPosition2UpdatedOnMouseMove),
                (canvasDragPoint3, AssertPosition3UpdatedOnMouseMove),
                ]);

            // assert
            void AssertPosition1UpdatedOnMouseMove()
            {
                var container = RichItemsControl.Items[0];
                container.RichItemContainerData.Top.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Top);
                container.RichItemContainerData.Left.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Left);
            }
            void AssertPosition2UpdatedOnMouseMove()
            {
                var container = RichItemsControl.Items[0];
                container.RichItemContainerData.Top.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Top);
                container.RichItemContainerData.Left.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Left);
            }
            void AssertPosition3UpdatedOnMouseMove()
            {
                var container = RichItemsControl.Items[0];
                container.RichItemContainerData.Top.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Top);
                container.RichItemContainerData.Left.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Left);
            }
        }

        [Test]
        public void DragContainer_WithRealTimeDraggingDisabled_ShouldUpdateTopLeftPositionWhenDragHasFinished()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            var pointOnRectangle = new Point((int)DrawingStateDataMocks.DrawnRectangleMock.Left + 1, (int)DrawingStateDataMocks.DrawnRectangleMock.Top + 1);
            var endDraggingPoint = new Point(pointOnRectangle.X + 100, pointOnRectangle.Y + 100);
            var canvasPointOnRectangle = pointOnRectangle.ToCanvasDrawingPoint();
            var canvasEndDraggingPoint = endDraggingPoint.ToCanvasDrawingPoint();

            // act
            Input.WithGesture(RichCanvasGestures.Drag).Drag(canvasPointOnRectangle, canvasEndDraggingPoint);

            // assert
            var container = RichItemsControl.Items[0];
            container.RichItemContainerData.Top.Should().Be(endDraggingPoint.Y - 1);
            container.RichItemContainerData.Left.Should().Be(endDraggingPoint.X - 1);
        }
    }
}
