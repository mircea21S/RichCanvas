using FlaUI.Core.Input;
using FluentAssertions;
using NUnit.Framework;
using RichCanvasUITests.App.Automation;
using System.Drawing;
using System.Linq;

namespace RichCanvas.UITests.Tests
{
    [TestFixture]
    public class SingleSelectionStateTests : RichCanvasTestAppTest
    {
        // TODO: use the actual UI APP static "mocks" on acting and asserting
        [Test]
        public void SingleSelectionStateDrag_WithRealTimeSelectionEnabled_ShouldSelectFirstInteresectingItem()
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
        public void SingleSelectionStateDrag_WithRealTimeSelectionEnabled_ShouldUpdateSelectedItemIfSelectionAreaDoesNotContainCurrentOne()
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
