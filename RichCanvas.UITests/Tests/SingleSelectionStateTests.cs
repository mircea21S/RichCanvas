using FlaUI.Core.Input;
using FluentAssertions;
using NUnit.Framework;
using RichCanvas.Gestures;
using RichCanvas.UITests.Helpers;
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

            // act and assert
            var startPoint = new Point(30, 30);
            Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                (new Point(150, 150), () => RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0])),
                (new Point(250, 250), () => RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0])),
                (new Point(350, 350), () => RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]))
                ]);

            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]);
        }

        [Test]
        public void SingleSelectionStateDrag_WithRealTimeSelectionEnabled_SelectedItemsShouldContainOnlyOneElement()
        {
            // arrange
            ArrangeRealTimeScenario();

            // act and assert
            var startPoint = new Point(200, 75);
            Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                (new Point(80, 85), () => Wait.UntilInputIsProcessed()),
                (new Point(80, 330), null)
                ]);

            RichItemsControl.SelectedItems.Length.Should().Be(1);
            RichItemsControl.SelectedItems.SingleOrDefault().Should().Be(RichItemsControl.Items[0]);
        }

        [Test]
        public void SingleSelectionStateDrag_WithRealTimeSelectionEnabled_ShouldUpdateSelectedItemIfSelectionAreaDoesNotContainCurrentOne()
        {
            // arrange
            ArrangeRealTimeScenario();

            // act and assert
            var startPoint = new Point(200, 75);
            Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                (new Point(80, 85), () => RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0])),
                (new Point(80, 330), () => RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0])),
                (new Point(105, 330), () => RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[2])),
                (new Point(105, 260), () => RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[1]))
                ]);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateDrag_WithEachSelectionType_ShouldSelectEachClickedItem(bool realTimeSelectionEnabled)
        {
            // arrange
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario();
            }
            else
            {
                // add items for selection
                Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
                Wait.UntilInputIsProcessed();
            }

            // act and assert
            Mouse.Click(RichItemContainerModelMocks.SingleSelectionRealTimeDragTestItems[0].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]);

            Mouse.Click(RichItemContainerModelMocks.SingleSelectionRealTimeDragTestItems[1].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[1]);

            Mouse.Click(RichItemContainerModelMocks.SingleSelectionRealTimeDragTestItems[2].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[2]);
        }

        [Test]
        public void SingleSelectionStateDrag_WithRealTimeSelectionDisabled_ShouldSelectNothingWhileMouseMoves()
        {
            // arrange
            // add items for selection
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
            Wait.UntilInputIsProcessed();

            // act and assert
            var startPoint = new Point(30, 30);
            Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                (new Point(100, 100), () => 
                {
                    RichItemsControl.SelectedItem.Should().BeNull();
                    RichItemsControl.SelectedItems.Should().BeEmpty();
                }),
                (new Point(200, 200), () =>
                {
                    RichItemsControl.SelectedItem.Should().BeNull();
                    RichItemsControl.SelectedItems.Should().BeEmpty();
                }),
                (new Point(300, 300), () =>
                {
                    RichItemsControl.SelectedItem.Should().BeNull();
                    RichItemsControl.SelectedItems.Should().BeEmpty();
                }),
                ]);
        }

        [Test]
        public void SingleSelectionStateDrag_WithRealTimeSelectionDisabled_ShouldSelectOneItemOnMouseUp()
        {
            // arrange
            // add items for selection
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
            Wait.UntilInputIsProcessed();

            // act
            Mouse.Drag(new Point(30, 30), new Point(300, 300));

            // assert
            RichItemsControl.SelectedItem.Should().NotBeNull();
            RichItemsControl.SelectedItems.Length.Should().Be(1);
        }

        [Test]
        public void SingleSelectionStateDrag_WithRealTimeSelectionDisabled_ShouldSelectLastItemAddedToItemsSourceOnMouseUp()
        {
            // arrange
            // add items for selection
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
            Wait.UntilInputIsProcessed();

            // act
            Mouse.Drag(new Point(30, 30), new Point(300, 300));

            // assert
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[2]);
        }

        private void ArrangeRealTimeScenario()
        {
            // enable real-time selection
            Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
            ReleaseRealTimeSelection = true;
            // add items for selection
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
            Wait.UntilInputIsProcessed();
        }
    }
}
