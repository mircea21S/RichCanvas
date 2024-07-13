using FlaUI.Core.Input;
using FluentAssertions;
using NUnit.Framework;
using RichCanvasUITests.App.Automation;
using RichCanvasUITests.App.TestMocks;
using System.Drawing;
using System.Linq;

namespace RichCanvas.UITests.Tests
{
    [TestFixture]
    public class SingleSelectionStateTests : RichCanvasTestAppTest
    {
        // TODO: use the actual UI APP static "mocks" on acting and asserting
        public bool ReleaseRealTimeSelection { get; private set; }

        public override void TearDown()
        {
            if (ReleaseRealTimeSelection)
            {
                // un-toggle real-time selection button
                Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
            }
            base.TearDown();
        }

        [Test]
        public void SingleSelectionStateDrag_WithRealTimeSelectionEnabled_ShouldSelectFirstInteresectingItem()
        {
            // arrange
            ArrangeRealTimeScenario();
            ReleaseRealTimeSelection = true;

            // act and assert
            Mouse.Position = new Point(30, 30);

            Mouse.Down();

            Mouse.Position = new Point(150, 150);
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]);

            Mouse.Position = new Point(250, 250);
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]);

            Mouse.Position = new Point(350, 350);
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]);

            Mouse.Up();
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]);
        }

        [Test]
        public void SingleSelectionStateDrag_WithRealTimeSelectionEnabled_SelectedItemsShouldContainOnlyOneElement()
        {
            // arrange
            ArrangeRealTimeScenario();
            ReleaseRealTimeSelection = true;

            // act and assert
            Mouse.Position = new Point(200, 75);
            Mouse.Down();
            Mouse.Position = new Point(80, 85);
            Wait.UntilInputIsProcessed();
            Mouse.Position = new Point(80, 330);
            Mouse.Up();

            RichItemsControl.SelectedItems.Length.Should().Be(1);
            RichItemsControl.SelectedItems.SingleOrDefault().Should().Be(RichItemsControl.Items[0]);
        }

        [Test]
        public void SingleSelectionStateDrag_WithRealTimeSelectionEnabled_ShouldUpdateSelectedItemIfSelectionAreaDoesNotContainCurrentOne()
        {
            // arrange
            ArrangeRealTimeScenario();
            ReleaseRealTimeSelection = true;

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

        [Test]
        public void SingleSelectionStateDrag_WithRealTimeSelectionEnabled_ShouldSelectEachClickedItem()
        {
            // arrange
            ArrangeRealTimeScenario();
            ReleaseRealTimeSelection = true;

            // act and assert
            Mouse.Click(RichItemContainerModelMocks.SingleSelectionRealTimeDragTestItems[0].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]);

            Mouse.Click(RichItemContainerModelMocks.SingleSelectionRealTimeDragTestItems[1].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[1]);

            Mouse.Click(RichItemContainerModelMocks.SingleSelectionRealTimeDragTestItems[2].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[2]);

        }

        private void ArrangeRealTimeScenario()
        {
            // enable real-time selection
            Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
            // add items for selection
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
            Wait.UntilInputIsProcessed();
        }
    }
}
