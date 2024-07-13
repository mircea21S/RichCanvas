using FlaUI.Core.Input;
using FluentAssertions;
using NUnit.Framework;
using RichCanvasUITests.App.Automation;
using RichCanvasUITests.App.TestMocks;
using System.Drawing;

namespace RichCanvas.UITests.Tests
{
    public class MultipleSelectionStateTests : RichCanvasTestAppTest
    {
        public override void SetUp()
        {
            // enable multiple selection
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            base.SetUp();
        }

        public override void TearDown()
        {
            // disable multiple selection
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            base.TearDown();
        }

        [Test]
        public void MultipleSelectionStateDrag_WithRealTimeSelectionEnabled_ShouldSelectItemsWhileDragging()
        {
            // arrange
                // enable real-time selection
            Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);

                // add items for selection
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId);
            var currentUiItems = RichItemContainerModelMocks.PositionedSelectableItemListMock;
            Wait.UntilInputIsProcessed();

            // act and assert
            Mouse.Position = new Point((int)currentUiItems[0].Left - 1, (int)currentUiItems[0].Top - 1);

            Mouse.Down();

                // select first item
            Mouse.Position = new Point((int)currentUiItems[0].BoundingBox.BottomRight.X + 1, (int)currentUiItems[0].BoundingBox.BottomRight.Y + 1);
            RichItemsControl.SelectedItems.Length.Should().Be(1);

                // select second item
            Mouse.Position = new Point((int)currentUiItems[1].BoundingBox.BottomRight.X + 1, (int)currentUiItems[1].BoundingBox.BottomRight.Y + 1);
            RichItemsControl.SelectedItems.Length.Should().Be(2);

            Mouse.Up();
            RichItemsControl.SelectedItems.Length.Should().Be(2);
        }

    }
}
