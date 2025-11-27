using System.Collections.Generic;
using System.Drawing;

using FlaUI.Core.Tools;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.UIAutomation.Tests.Tests.Selection.SelectionModes;

using RichCanvasUIA.Client;
using RichCanvasUIA.Client.Automation;
using RichCanvasUIA.Client.TestMocks;

namespace RichCanvas.UIAutomation.Tests.Tests.Dragging
{
    [TestFixture]
    public class DraggingContainerStateTests : RichCanvasTestAppTest
    {
        [Test, RealTimeDragging(true)]
        public void DragSingleContainer_WithRealTimeDraggingEnabled_ShouldUpdateContainerLocationWhileMouseIsMoving()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            int dragOffset = 50;

            // act & assert
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            drawnContainer.StartDragging();

            Point locationBeforeMove = drawnContainer.Location;
            drawnContainer.Move(dragOffset);
            drawnContainer.Location.Should().Be(Point.Add(locationBeforeMove, new Size(dragOffset, dragOffset)).ToCanvasDrawingPoint());

            Point locationBeforeMove2 = drawnContainer.Location;
            drawnContainer.Move(dragOffset);
            drawnContainer.Location.Should().Be(Point.Add(locationBeforeMove2, new Size(dragOffset, dragOffset)).ToCanvasDrawingPoint());

            drawnContainer.EndDragging();
        }

        [Test, RealTimeDragging(false)]
        public void DragSingleContainer_WithRealTimeDraggingDisabled_ShouldNotUpdateContainerLocationWhileMouseIsMoving()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            int dragOffset = 50;

            // act & assert
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            drawnContainer.StartDragging();

            drawnContainer.Move(dragOffset);
            drawnContainer.Location.Should().Be(new Point(DrawingStateDataMocks.DrawnRectangleMock.Left.ToInt(), DrawingStateDataMocks.DrawnRectangleMock.Top.ToInt()));

            drawnContainer.Move(dragOffset);
            drawnContainer.Location.Should().Be(new Point(DrawingStateDataMocks.DrawnRectangleMock.Left.ToInt(), DrawingStateDataMocks.DrawnRectangleMock.Top.ToInt()));

            drawnContainer.EndDragging();
        }

        [Test, RealTimeDragging(false)]
        public void DragSingleContainer_WithRealTimeDraggingDisabled_ShouldUpdateContainerLocationWhenMouseIsReleased()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            int dragOffset = 50;

            // act
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            drawnContainer.Drag(dragOffset);

            // assert
            var containerInitialLocation = new Point(DrawingStateDataMocks.DrawnRectangleMock.Left.ToInt(), DrawingStateDataMocks.DrawnRectangleMock.Top.ToInt());
            drawnContainer.Location.Should().Be(Point.Add(containerInitialLocation, new Size(dragOffset, dragOffset)).ToCanvasDrawingPoint());
        }

        [Test, RealTimeDragging(true), MultipleSelection]
        public void DragMultipleContainers_WithRealTimeSelectionEnabled_ShouldUpdatePositionForAllSelectedContainersWhileMouseIsMoving()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
            List<RichItemContainerModel> containers = SingleSelectionStateDataMocks.SingleSelectionItems;
            RichCanvas.SelectAllItems();
            int dragOffset = 50;

            // act & assert
            RichCanvasContainerAutomation dragContainer = RichCanvas.Items[0];
            dragContainer.StartDragging();

            dragContainer.Move(dragOffset);
            for (int i = 0; i < containers.Count; i++)
            {
                RichItemContainerModel item = containers[i];
                RichCanvasContainerAutomation container = RichCanvas.Items[i];
                container.Location.Should().Be(new Point(item.Left.ToInt() + dragOffset, item.Top.ToInt() + dragOffset).ToCanvasDrawingPoint());
            }

            dragContainer.Move(dragOffset);
            for (int i = 0; i < containers.Count; i++)
            {
                RichItemContainerModel item = containers[i];
                RichCanvasContainerAutomation container = RichCanvas.Items[i];
                container.Location.Should().Be(new Point(item.Left.ToInt() + (dragOffset * 2), item.Top.ToInt() + (dragOffset * 2))
                    // called twice to apply the title bar height twice as containers were moved twice in the scenario
                    .ToCanvasDrawingPoint()
                    .ToCanvasDrawingPoint());
            }

            dragContainer.EndDragging();
        }

        [Test, RealTimeDragging(false), MultipleSelection]
        public void DragMultipleContainers_WithRealTimeSelectionDisabled_ShouldUpdatePositionForAllSelectedContainersWhenMouseIsReleased()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
            List<RichItemContainerModel> containers = SingleSelectionStateDataMocks.SingleSelectionItems;
            RichCanvas.SelectAllItems();
            RichCanvasUIAClientCommunicator.Send("Test");
            int dragOffset = 50;

            // act
            RichCanvasContainerAutomation dragContainer = RichCanvas.Items[0];
            dragContainer.Drag(dragOffset);

            // assert
            for (int i = 0; i < containers.Count; i++)
            {
                RichItemContainerModel item = containers[i];
                RichCanvasContainerAutomation container = RichCanvas.Items[i];
                container.Location.Should().Be(new Point(item.Left.ToInt() + dragOffset, item.Top.ToInt() + dragOffset).ToCanvasDrawingPoint());
            }
        }

        [Test]
        public void DraggingContainer_WhenIsNotDraggable_ShouldNotUpdateLocation()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            RichCanvasContainerAutomation container = RichCanvas.Items[0];
            container.IsDraggable = false;

            // act
            container.Drag(50);

            // assert
            container.Location.X.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Left.ToInt());
            container.Location.Y.Should().Be(DrawingStateDataMocks.DrawnRectangleMock.Top.ToInt());
        }
    }
}
