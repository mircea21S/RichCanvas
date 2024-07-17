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
    public class MultipleSelectionStateTests : RichCanvasTestAppTest
    {
        private bool ReleaseRealTimeSelection { get; set; }

        public override void SetUp()
        {
            // enable multiple selection
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            base.SetUp();
        }

        public override void TearDown()
        {
            if (ReleaseRealTimeSelection)
            {
                Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
            }
            // disable multiple selection
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            base.TearDown();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void MultipleSelectionStateDrag_WithRealTimeSelectionEnabled_ShouldSelectItemsWhileDragging(bool inverseDrag)
        {
            // arrange
            ArrangeRealTimeScenario();
            var currentUiItems = RichItemContainerModelMocks.PositionedSelectableItemsListMock;
            Wait.UntilInputIsProcessed();

            // act and assert
            if (inverseDrag)
            {
                var startPoint = new Point((int)currentUiItems[1].BoundingBox.Right + 1, (int)currentUiItems[1].BoundingBox.Bottom + 1);
                Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                    (currentUiItems[1].BoundingBox.TopLeft.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(1)),
                    (currentUiItems[0].BoundingBox.TopLeft.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(2))
                    ]);

                RichItemsControl.SelectedItems.Length.Should().Be(currentUiItems.Count);
            }
            else
            {
                var startPoint = new Point((int)currentUiItems[0].Left - 1, (int)currentUiItems[0].Top - 1);
                Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                   (currentUiItems[0].BoundingBox.BottomRight.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(1)),
                    (currentUiItems[1].BoundingBox.BottomRight.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(2))
                   ]);

                RichItemsControl.SelectedItems.Length.Should().Be(currentUiItems.Count);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void MultipleSelectionStateClickItem_WithEachSelectionType_ShouldSelectItemsWhenClickingThem(bool realTimeSelectionEnabled)
        {
            // arrange
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario();
            }
            else
            {
                // add items for selection
                Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId);
                Wait.UntilInputIsProcessed();
            }
            var currentUiItems = RichItemContainerModelMocks.PositionedSelectableItemsListMock;

            // act and assert
            Mouse.Click(currentUiItems[0].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItems.Length.Should().Be(1);

            Mouse.Click(currentUiItems[1].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItems.Length.Should().Be(2);
        }

        private void ArrangeRealTimeScenario()
        {
            // enable real-time selection
            Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
            ReleaseRealTimeSelection = true;
            // add items for selection
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId);
            Wait.UntilInputIsProcessed();
        }
    }
}
