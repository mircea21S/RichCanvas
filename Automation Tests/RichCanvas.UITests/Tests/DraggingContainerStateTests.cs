using FlaUI.Core.Input;
using FluentAssertions;
using NUnit.Framework;
using RichCanvas.Gestures;
using RichCanvas.UITests.Helpers;
using RichCanvasUITests.App.Automation;
using RichCanvasUITests.App.TestMocks;
using System.Drawing;

namespace RichCanvas.UITests.Tests
{
    [TestFixture]
    public class DraggingContainerStateTests : RichCanvasTestAppTest
    {
        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void DragContainer_WithRealTimeDraggingEnabledAndEachSelectionScenario_ShouldUpdateTopLeftPositionWhileDragging(bool canSelectMultipleItems)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
            if (canSelectMultipleItems)
            {
                Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            }

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
                var container = RichCanvas.Items[0];
                container.RichCanvasContainerData.Top.Should().Be(dragPoint1.Y - 1);
                container.RichCanvasContainerData.Left.Should().Be(dragPoint1.X - 1);
            }
            void AssertPosition2UpdatedOnMouseMove()
            {
                var container = RichCanvas.Items[0];
                container.RichCanvasContainerData.Top.Should().Be(dragPoint2.Y - 1);
                container.RichCanvasContainerData.Left.Should().Be(dragPoint2.X - 1);
            }
            void AssertPosition3UpdatedOnMouseMove()
            {
                var container = RichCanvas.Items[0];
                container.RichCanvasContainerData.Top.Should().Be(dragPoint3.Y - 1);
                container.RichCanvasContainerData.Left.Should().Be(dragPoint3.X - 1);
            }

            var container = RichCanvas.Items[0];
            container.RichCanvasContainerData.Top.Should().Be(dragPoint3.Y - 1);
            container.RichCanvasContainerData.Left.Should().Be(dragPoint3.X - 1);
            if (canSelectMultipleItems)
            {
                Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            }
            Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void DragContainer_WithRealTimeDraggingDisabledAndEachSelectionScenario_ShouldNotUpdateTopLeftPositionWhileDragging(bool canSelectMultipleItems)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            if (canSelectMultipleItems)
            {
                Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            }

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
                var container = RichCanvas.Items[0];
                container.RichCanvasContainerData.Top.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Top);
                container.RichCanvasContainerData.Left.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Left);
            }
            void AssertPosition2UpdatedOnMouseMove()
            {
                var container = RichCanvas.Items[0];
                container.RichCanvasContainerData.Top.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Top);
                container.RichCanvasContainerData.Left.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Left);
            }
            void AssertPosition3UpdatedOnMouseMove()
            {
                var container = RichCanvas.Items[0];
                container.RichCanvasContainerData.Top.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Top);
                container.RichCanvasContainerData.Left.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Left);
            }
            if (canSelectMultipleItems)
            {
                Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void DragContainer_WithRealTimeDraggingDisabledAndEachSelectionScenario_ShouldUpdateTopLeftPositionWhenDragHasFinished(bool canSelectMultipleItems)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            if (canSelectMultipleItems)
            {
                Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            }

            var pointOnRectangle = new Point((int)DrawingStateDataMocks.DrawnRectangleMock.Left + 1, (int)DrawingStateDataMocks.DrawnRectangleMock.Top + 1);
            var endDraggingPoint = new Point(pointOnRectangle.X + 100, pointOnRectangle.Y + 100);
            var canvasPointOnRectangle = pointOnRectangle.ToCanvasDrawingPoint();
            var canvasEndDraggingPoint = endDraggingPoint.ToCanvasDrawingPoint();

            // act
            Input.WithGesture(RichCanvasGestures.Drag).Drag(canvasPointOnRectangle, canvasEndDraggingPoint);
            Wait.UntilInputIsProcessed();

            // assert
            var container = RichCanvas.Items[0];
            container.RichCanvasContainerData.Top.Should().Be(endDraggingPoint.Y - 1);
            container.RichCanvasContainerData.Left.Should().Be(endDraggingPoint.X - 1);
            if (canSelectMultipleItems)
            {
                Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void DragMultipleContainer_WhenCanSelectMultipleItemsIsFalseAndWithEachDraggingScenario_ShouldSelectOnlyOneContainerPerDrag(bool realTimeDraggingEnabled)
        {
            // arrange
            if (realTimeDraggingEnabled)
            {
                Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
            }
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);

            // act & assert
            var (startPoint0, endPoint0) = GetDragPointsForContainer(0);
            Input.WithGesture(RichCanvasGestures.Drag).Drag(startPoint0, endPoint0);
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]);
            RichCanvas.SelectedItems.Length.Should().Be(1);

            var (startPoint1, endPoint1) = GetDragPointsForContainer(1);
            Input.WithGesture(RichCanvasGestures.Drag).Drag(startPoint1, endPoint1);
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[1]);
            RichCanvas.SelectedItems.Length.Should().Be(1);

            var (startPoint2, endPoint2) = GetDragPointsForContainer(2);
            Input.WithGesture(RichCanvasGestures.Drag).Drag(startPoint2, endPoint2);
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[2]);
            RichCanvas.SelectedItems.Length.Should().Be(1);

            if (realTimeDraggingEnabled)
            {
                Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void DragMultipleContainer_WhenCanSelectMultipleItemsIsTrueAndWithEachDraggingScenario_ShouldAddToSelectionEachDraggedContainer(bool realTimeDraggingEnabled)
        {
            // arrange
            if (realTimeDraggingEnabled)
            {
                Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
            }
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);

            // act & assert
            var (startPoint0, endPoint0) = GetDragPointsForContainer(0);
            Input.WithGesture(RichCanvasGestures.Drag).Drag(startPoint0, endPoint0);
            RichCanvas.SelectedItems.Length.Should().Be(1);

            var (startPoint1, endPoint1) = GetDragPointsForContainer(1);
            Input.WithGesture(RichCanvasGestures.Drag).Drag(startPoint1, endPoint1);
            RichCanvas.SelectedItems.Length.Should().Be(2);

            var (startPoint2, endPoint2) = GetDragPointsForContainer(2);
            Input.WithGesture(RichCanvasGestures.Drag).Drag(startPoint2, endPoint2);
            RichCanvas.SelectedItems.Length.Should().Be(3);

            if (realTimeDraggingEnabled)
            {
                Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
            }
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
        }

        private (Point StartPoint, Point EndPoint) GetDragPointsForContainer(int index)
        {
            var containers = SingleSelectionStateDataMocks.SingleSelectionItems;
            var firstContainerStartPoint = new Point((int)containers[index].Left + 1, (int)containers[index].Top + 1);
            var firstContainerEndPoint = new Point(firstContainerStartPoint.X + 100, firstContainerStartPoint.Y + 10);
            var fristContainerCanvasStartPoint = firstContainerStartPoint.ToCanvasDrawingPoint();
            var firstContainerCanvasEndPoint = firstContainerEndPoint.ToCanvasDrawingPoint();
            return (fristContainerCanvasStartPoint, firstContainerCanvasEndPoint);
        }

        [Test]
        public void DragMultipleSelectedContainers_WithRealTimeSelectionEnabled_ShouldUpdatePositionForAllSelectedContainersOnDrag()
        {
            // arrange
            Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);

            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
            var containers = SingleSelectionStateDataMocks.SingleSelectionItems;
            Window.InvokeButton(AutomationIds.SelectAllItemsButtonId);

            // act
            var pointOnFirstContainer = new Point((int)containers[0].Left + 1, (int)containers[0].Top + 1);
            var dragPoint1 = new Point(pointOnFirstContainer.X + 50, pointOnFirstContainer.Y + 50);
            var dragPoint2 = new Point(dragPoint1.X + 50, dragPoint1.Y + 50);
            var dragPoint3 = new Point(dragPoint2.X + 50, dragPoint2.Y + 50);

            var canvasPointOnRectangle = pointOnFirstContainer.ToCanvasDrawingPoint();
            var canvasDragPoint1 = dragPoint1.ToCanvasDrawingPoint();
            var canvasDragPoint2 = dragPoint2.ToCanvasDrawingPoint();
            var canvasDragPoint3 = dragPoint3.ToCanvasDrawingPoint();

            Input.WithGesture(RichCanvasGestures.Drag).DefferedDrag(canvasPointOnRectangle, [
                (canvasDragPoint1, AssertPosition1UpdatedOnMouseMove),
                (canvasDragPoint2, AssertPosition2UpdatedOnMouseMove),
                (canvasDragPoint3, AssertPosition3UpdatedOnMouseMove),
                ]);

            // assert
            void AssertPosition1UpdatedOnMouseMove()
            {
                var container = RichCanvas.Items[0];
                var container1 = RichCanvas.Items[1];
                var container2 = RichCanvas.Items[2];
                container.RichCanvasContainerData.Top.Should().Be(dragPoint1.Y - 1);
                container.RichCanvasContainerData.Left.Should().Be(dragPoint1.X - 1);
                container1.RichCanvasContainerData.Top.Should().Be(containers[1].Top + (dragPoint1.Y - pointOnFirstContainer.Y));
                container1.RichCanvasContainerData.Left.Should().Be(containers[1].Left + (dragPoint1.X - pointOnFirstContainer.X));
                container2.RichCanvasContainerData.Top.Should().Be(containers[2].Top + (dragPoint1.Y - pointOnFirstContainer.Y));
                container2.RichCanvasContainerData.Left.Should().Be(containers[2].Left + (dragPoint1.X - pointOnFirstContainer.X));
            }
            void AssertPosition2UpdatedOnMouseMove()
            {
                var container = RichCanvas.Items[0];
                var container1 = RichCanvas.Items[1];
                var container2 = RichCanvas.Items[2];
                container.RichCanvasContainerData.Top.Should().Be(dragPoint2.Y - 1);
                container.RichCanvasContainerData.Left.Should().Be(dragPoint2.X - 1);
                container1.RichCanvasContainerData.Top.Should().Be(containers[1].Top + (dragPoint2.Y - pointOnFirstContainer.Y));
                container1.RichCanvasContainerData.Left.Should().Be(containers[1].Left + (dragPoint2.X - pointOnFirstContainer.X));
                container2.RichCanvasContainerData.Top.Should().Be(containers[2].Top + (dragPoint2.Y - pointOnFirstContainer.Y));
                container2.RichCanvasContainerData.Left.Should().Be(containers[2].Left + (dragPoint2.X - pointOnFirstContainer.X));
            }
            void AssertPosition3UpdatedOnMouseMove()
            {
                var container = RichCanvas.Items[0];
                var container1 = RichCanvas.Items[1];
                var container2 = RichCanvas.Items[2];
                container.RichCanvasContainerData.Top.Should().Be(dragPoint3.Y - 1);
                container.RichCanvasContainerData.Left.Should().Be(dragPoint3.X - 1);
                container1.RichCanvasContainerData.Top.Should().Be(containers[1].Top + (dragPoint3.Y - pointOnFirstContainer.Y));
                container1.RichCanvasContainerData.Left.Should().Be(containers[1].Left + (dragPoint3.X - pointOnFirstContainer.X));
                container2.RichCanvasContainerData.Top.Should().Be(containers[2].Top + (dragPoint3.Y - pointOnFirstContainer.Y));
                container2.RichCanvasContainerData.Left.Should().Be(containers[2].Left + (dragPoint3.X - pointOnFirstContainer.X));
            }

            Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
        }
    }
}
