using System.Collections.Generic;
using System.Drawing;

using FlaUI.Core.Tools;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.UIAutomation.FlaUIClient;
using RichCanvas.UIAutomation.Tests.Extensions;
using RichCanvas.UIAutomation.Tests.Tests.Selection.SelectionModes;

using RichCanvasUIA.Client.UIA_Mode;
using RichCanvasUIA.Client.UIA_Mode.Automation_Models;

namespace RichCanvas.UIAutomation.Tests.Tests.Dragging
{
    [TestFixture]
    public class DraggingContainerStateTests : RichCanvasTestAppTest
    {
        [Test, RealTimeDragging(true)]
        public void DragSingleContainer_WithRealTimeDraggingEnabled_ShouldUpdateContainerLocationWhileMouseIsMoving()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddDrawnRectangle();
            int dragOffset = 50;

            // act & assert
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            drawnContainer.StartDragging(drawnContainer.GetLocationPoint());

            Point locationBeforeMove = drawnContainer.Location;
            drawnContainer.Move(CurrentMousePosition.AddOffset(dragOffset));
            drawnContainer.Location.Should().Be(Point.Add(locationBeforeMove, new Size(dragOffset, dragOffset)));

            Point locationBeforeMove2 = drawnContainer.Location;
            drawnContainer.Move(CurrentMousePosition.AddOffset(dragOffset));
            drawnContainer.Location.Should().Be(Point.Add(locationBeforeMove2, new Size(dragOffset, dragOffset)));

            drawnContainer.EndDragging();
        }

        [Test, RealTimeDragging(false)]
        public void DragSingleContainer_WithRealTimeDraggingDisabled_ShouldNotUpdateContainerLocationWhileMouseIsMoving()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddDrawnRectangle();
            int dragOffset = 50;

            // act & assert
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            drawnContainer.StartDragging(drawnContainer.GetLocationPoint());

            drawnContainer.Move(CurrentMousePosition.AddOffset(dragOffset));
            drawnContainer.Location.Should().Be(new Point(PreDefinedAutomationItemModels.FullyDrawnRectangle.Left.ToInt(), PreDefinedAutomationItemModels.FullyDrawnRectangle.Top.ToInt()));

            drawnContainer.Move(CurrentMousePosition.AddOffset(dragOffset));
            drawnContainer.Location.Should().Be(new Point(PreDefinedAutomationItemModels.FullyDrawnRectangle.Left.ToInt(), PreDefinedAutomationItemModels.FullyDrawnRectangle.Top.ToInt()));

            drawnContainer.EndDragging();
        }

        [Test, RealTimeDragging(false)]
        public void DragSingleContainer_WithRealTimeDraggingDisabled_ShouldUpdateContainerLocationWhenMouseIsReleased()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddDrawnRectangle();
            int dragOffset = 50;

            // act
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            Point startPoint = drawnContainer.GetLocationPoint();
            drawnContainer.Drag(startPoint, startPoint.AddOffset(dragOffset));

            // assert
            var containerInitialLocation = new Point(PreDefinedAutomationItemModels.FullyDrawnRectangle.Left.ToInt(), PreDefinedAutomationItemModels.FullyDrawnRectangle.Top.ToInt());
            drawnContainer.Location.Should().Be(Point.Add(containerInitialLocation, new Size(dragOffset, dragOffset)));
        }

        [Test, RealTimeDragging(true), MultipleSelection]
        public void DragMultipleContainers_WithRealTimeSelectionEnabled_ShouldUpdatePositionForAllSelectedContainersWhileMouseIsMoving()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSingleSelectionTestItems();
            List<RichCanvasContainerAutomationModel> containers = PreDefinedAutomationItemModels.SelectableItemsForSingleSelection;
            RichCanvas.SelectAllItems();
            int dragOffset = 50;

            // act & assert
            RichCanvasContainerAutomation dragContainer = RichCanvas.Items[0];
            dragContainer.StartDragging(dragContainer.GetLocationPoint());

            dragContainer.Move(CurrentMousePosition.AddOffset(dragOffset));
            for (int i = 0; i < containers.Count; i++)
            {
                var item = containers[i];
                RichCanvasContainerAutomation container = RichCanvas.Items[i];
                container.Location.Should().Be(new Point(item.Left.ToInt() + dragOffset, item.Top.ToInt() + dragOffset));
            }

            dragContainer.Move(CurrentMousePosition.AddOffset(dragOffset));
            for (int i = 0; i < containers.Count; i++)
            {
                var item = containers[i];
                RichCanvasContainerAutomation container = RichCanvas.Items[i];
                container.Location.Should().Be(new Point(item.Left.ToInt() + (dragOffset * 2), item.Top.ToInt() + (dragOffset * 2)));
            }

            dragContainer.EndDragging();
        }

        [Test, RealTimeDragging(false), MultipleSelection]
        public void DragMultipleContainers_WithRealTimeSelectionDisabled_ShouldUpdatePositionForAllSelectedContainersWhenMouseIsReleased()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSingleSelectionTestItems();
            List<RichCanvasContainerAutomationModel> containers = PreDefinedAutomationItemModels.SelectableItemsForSingleSelection;
            RichCanvas.SelectAllItems();
            int dragOffset = 50;

            // act
            RichCanvasContainerAutomation dragContainer = RichCanvas.Items[0];
            Point startPoint = dragContainer.GetLocationPoint();
            dragContainer.Drag(startPoint, startPoint.AddOffset(dragOffset));

            // assert
            for (int i = 0; i < containers.Count; i++)
            {
                var item = containers[i];
                RichCanvasContainerAutomation container = RichCanvas.Items[i];
                container.Location.Should().Be(new Point(item.Left.ToInt() + dragOffset, item.Top.ToInt() + dragOffset));
            }
        }

        [Test]
        public void DraggingContainer_WhenIsNotDraggable_ShouldNotUpdateLocation()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddDrawnRectangle();
            RichCanvasContainerAutomation container = RichCanvas.Items[0];
            container.IsDraggable = false;

            // act
            container.Drag(container.GetLocationPoint(), CurrentMousePosition.AddOffset(50));

            // assert
            container.Location.X.Should().Be(PreDefinedAutomationItemModels.FullyDrawnRectangle.Left.ToInt());
            container.Location.Y.Should().Be(PreDefinedAutomationItemModels.FullyDrawnRectangle.Top.ToInt());
        }
    }
}
