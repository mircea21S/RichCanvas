using FlaUI.Core.AutomationElements;
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
    public class SelectingStateTests : RichCanvasTestAppTest
    {
        [Test]
        public void RichItemsControlAndSelectingState_AddMultipleItemsAndRealTimeSelectionDisabledThenClickAndDrag_ShouldSelectAllContainers()
        {
            // arrange
            var startPoint = new Point(0, 30);
            var endPoint = new Point(500, 500);
            var startPointFixed = startPoint.AsFlaUIFixedPoint();
            var endPointFixed = endPoint.AsFlaUIFixedPoint();

            // act
            // TODO: implement a way to set the DataContext with a mock for RichItemsControl to change the DPs values
            var buton = Window.FindFirstDescendant(AutomationIds.AddSelectableItemsButtonId).AsButton();
            buton.RegisterAutomationEvent(EventLibrary.Invoke.InvokedEvent, FlaUI.Core.Definitions.TreeScope.Element, (element, evnt) =>
            {

            });
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId);
            RichItemsControl.Items[0].RegisterAutomationEvent(EventLibrary.SelectionItem.ElementSelectedEvent, FlaUI.Core.Definitions.TreeScope.Element, (element, evnt) =>
            {

            });
            Input.WithGesture(RichCanvasGestures.Select).Drag(startPointFixed, endPointFixed);

            // assert
            RichItemsControl.SelectedItems.Length.Should().Be(RichItemContainerModelMocks.PositionedSelectableItemListMock.Count);
        }
    }
}
