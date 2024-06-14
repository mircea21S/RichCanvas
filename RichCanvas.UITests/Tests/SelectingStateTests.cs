using FlaUI.Core.Input;
using FluentAssertions;
using NUnit.Framework;
using RichCanvasUITests.App.Automation;
using RichCanvasUITests.App.TestMocks;
using System;
using System.Drawing;
using System.Linq;

namespace RichCanvas.UITests.Tests
{
    [TestFixture]
    public class SelectingStateTests : RichCanvasTestAppTest
    {
        // TODO: use the actual UI APP static "mocks" on acting and asserting
        [Test]
        public void SelectingStateDrag_WithRealTimeSelectionEnabledAndMultipleSelection_ShouldSelectItemsWhileDragging()
        {
            // arrange
            // enable real-time selection
            Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
            // enable multiple selection
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
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

        [Test]
        public void SelectingStateDrag_WithRealTimeSelectionEnabledAndSingleSelection_ShouldSelectFirstInteresectingItem()
        {
            // arrange
            // enable real-time selection
            Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
            // add items for selection
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId);
            Wait.UntilInputIsProcessed();

            // act and assert
            Mouse.Position = new Point(30, 30);

            Mouse.Down();

            Mouse.Position = new Point(100, 100);
            RichItemsControl.SelectedItems.Length.Should().Be(1);
            RichItemsControl.SelectedItem.Should().NotBeNull();

            Mouse.Position = new Point(200, 200);
            RichItemsControl.SelectedItems.Length.Should().Be(1);
            RichItemsControl.SelectedItem.Should().NotBeNull();

            Mouse.Up();
            RichItemsControl.SelectedItems.Length.Should().Be(1);
            RichItemsControl.SelectedItem.Should().NotBeNull();
            RichItemsControl.SelectedItems.SingleOrDefault().Should().Be(RichItemsControl.Items.FirstOrDefault());
        }

        [Test]
        public void SelectingStateDrag_WithRealTimeSelectionEnabledAndSingleSelection_ShouldUpdateSelectedItemIfSelectionAreaDoesNotContainCurrentOne()
        {
            // arrange
            // enable real-time selection
            Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
            // add items for selection
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
            Wait.UntilInputIsProcessed();

            // act and assert
            Mouse.Position = new Point(200, 75);

            Mouse.Down();

            Mouse.Position = new Point(80, 85);
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]);

            Mouse.Position = new Point(80, 330);
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]);

            Mouse.Position = new Point(105, 330);
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[2]);

            Mouse.Position = new Point(105, 260);
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[1]);

            Mouse.Up();
        }
    }
}
