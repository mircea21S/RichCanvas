using System.Drawing;
using System.Linq;

using FlaUI.Core.Input;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.Gestures;
using RichCanvas.UITests.Helpers;

using RichCanvasUITests.App.Automation;
using RichCanvasUITests.App.TestMocks;

namespace RichCanvas.UITests.Tests.Selection
{
    [TestFixture]
    public class SingleSelectionStateTests : RichCanvasTestAppTest
    {
        // TODO: use the actual UI APP static "mocks" on acting and asserting instead of magic Points
        public bool ReleaseRealTimeSelection { get; private set; }

        public override void TearDown()
        {
            base.TearDown();
            if (ReleaseRealTimeSelection)
            {
                // un-toggle real-time selection button
                Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
                ReleaseRealTimeSelection = false;
            }
        }

        [Test]
        public void SingleSelectionStateWithRealTimeSelectionEnabled_Drag_ShouldSelectFirstInteresectingItem()
        {
            // arrange
            ArrangeSelectionScenario();

            // act and assert
            var startPoint = new Point(30, 30);
            Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                (new Point(150, 150), () => RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0])),
                (new Point(250, 250), () => RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0])),
                (new Point(350, 350), () => RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]))
                ]);

            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]);
        }

        [Test]
        public void SingleSelectionStateWithRealTimeSelectionEnabled_Drag_SelectedItemsShouldContainOnlyOneElement()
        {
            // arrange
            ArrangeSelectionScenario();

            // act and assert
            var startPoint = new Point(200, 75);
            Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                (new Point(80, 85), () => Wait.UntilInputIsProcessed()),
                (new Point(80, 330), null)
                ]);

            RichCanvas.SelectedItems.Length.Should().Be(1);
            RichCanvas.SelectedItems.SingleOrDefault().Should().Be(RichCanvas.Items[0]);
        }

        [Test]
        public void SingleSelectionStateWithRealTimeSelectionEnabled_Drag_ShouldUpdateSelectedItemIfSelectionAreaDoesNotContainCurrentOne()
        {
            // arrange
            ArrangeSelectionScenario();

            // act and assert
            var startPoint = new Point(200, 75);
            Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                (new Point(80, 85), () => RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0])),
                (new Point(80, 330), () => RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0])),
                (new Point(105, 330), () => RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[2])),
                (new Point(105, 260), () => RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[1]))
                ]);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateWithSelectionAbility_ClickItems_ShouldSelectLatestClickedItem(bool realTimeSelectionEnabled)
        {
            // arrange
            ArrangeSelectionScenario(realTimeSelectionEnabled);

            // act and assert
            Input.WithGesture(RichCanvasGestures.Select)
                .Click(SingleSelectionStateDataMocks.SingleSelectionItems[0].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]);

            Input.WithGesture(RichCanvasGestures.Select)
                .Click(SingleSelectionStateDataMocks.SingleSelectionItems[1].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[1]);

            Input.WithGesture(RichCanvasGestures.Select)
                .Click(SingleSelectionStateDataMocks.SingleSelectionItems[2].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[2]);
        }

        [Test]
        public void SingleSelectionStateWithRealTimeSelectionDisabled_Drag_ShouldSelectNothingWhileMouseMoves()
        {
            // arrange
            AddSingleSelectionItems();

            // act and assert
            var startPoint = new Point(30, 30);
            Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                (new Point(100, 100), () =>
                {
                    RichCanvas.SelectedItem.Should().BeNull();
                    RichCanvas.SelectedItems.Should().BeEmpty();
                }),
                (new Point(200, 200), () =>
                {
                    RichCanvas.SelectedItem.Should().BeNull();
                    RichCanvas.SelectedItems.Should().BeEmpty();
                }),
                (new Point(300, 300), () =>
                {
                    RichCanvas.SelectedItem.Should().BeNull();
                    RichCanvas.SelectedItems.Should().BeEmpty();
                }),
                ]);
        }

        [Test]
        public void SingleSelectionStateWithRealTimeSelectionDisabled_Drag_ShouldSelectOneItemOnMouseUp()
        {
            // arrange
            AddSingleSelectionItems();

            // act
            Input.WithGesture(RichCanvasGestures.Select).Drag(new Point(30, 30), new Point(300, 300));

            // assert
            RichCanvas.SelectedItem.Should().NotBeNull();
            RichCanvas.SelectedItems.Length.Should().Be(1);
        }

        [Test]
        public void SingleSelectionStateWithRealTimeSelectionDisabled_Drag_ShouldSelectLastItemAddedToItemsSourceOnMouseUp()
        {
            // arrange
            AddSingleSelectionItems();

            // act
            Input.WithGesture(RichCanvasGestures.Select).Drag(new Point(30, 30), new Point(300, 300));

            // assert
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[2]);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateWithSelectionAbility_SetSelectedItemThroughBinding_ShouldSelectThatItem(bool realTimeSelectionEnabled)
        {
            // arrange
            ArrangeSelectionScenario(realTimeSelectionEnabled);

            // act & assert
            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId1);
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]);
            RichCanvas.SelectedItem.RichCanvasContainerData.IsSelected.Should().BeTrue();

            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId2);
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[1]);
            RichCanvas.SelectedItem.RichCanvasContainerData.IsSelected.Should().BeTrue();

            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId3);
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[2]);
            RichCanvas.SelectedItem.RichCanvasContainerData.IsSelected.Should().BeTrue();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateWithSelectionAbility_SetSelectedItemThroughBindingThenClickOnOtherItem_ShouldSelectClickedItem(bool realTimeSelectionEnabled)
        {
            // arrange
            ArrangeSelectionScenario(realTimeSelectionEnabled);
            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId2);
            // act & assert
            RichCanvas.SelectedItem.Should().NotBeNull();
            RichCanvas.Items[1].RichCanvasContainerData.IsSelected.Should().BeTrue();

            Input.WithGesture(RichCanvasGestures.Select).Click(SingleSelectionStateDataMocks.SingleSelectionItems[1].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichCanvas.SelectedItem.Should().NotBeNull();
            RichCanvas.Items[1].RichCanvasContainerData.IsSelected.Should().BeTrue();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateWithSelectionAbility_SetSelectedItemThroughBindingThenClickOnEmptyCanvas_ShouldClearSelectedItem(bool realTimeSelectionEnabled)
        {
            // arrange
            ArrangeSelectionScenario(realTimeSelectionEnabled);
            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId3);
            // act & assert
            Input.WithGesture(RichCanvasGestures.Select).Click(new Point(105, 105));
            RichCanvas.SelectedItem.Should().BeNull();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateWithSelectedItemAndSelectionAbility_ClearAllItems_ShouldClearSelection(bool realTimeSelectionEnabled)
        {
            // arrange
            IgnoreItemsClearOnTearDown = true;
            ArrangeSelectionScenario(realTimeSelectionEnabled);
            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId1);
            // act & assert
            Window.ClearAllItems();
            RichCanvas.SelectedItem.Should().BeNull();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateWithSelectedItem_WhenSetCanSelectMultipleItemsTrue_ShouldSetSelectedItemNull(bool realTimeSelectionEnabled)
        {
            // arrange
            ArrangeSelectionScenario(realTimeSelectionEnabled);
            System.Collections.Generic.List<RichCanvasUITests.App.RichItemContainerModel> currentUiItems = SingleSelectionStateDataMocks.SingleSelectionItems;

            // act
            Input.WithGesture(RichCanvasGestures.Select).Click(currentUiItems[0].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);

            // assert
            RichCanvas.SelectedItem.Should().BeNull();
            RichCanvas.SelectedItems.Should().BeEmpty();
            // toggle button again preparing for teardown
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateWithSelectedItem_WhenSetCanSelectMultipleItemsTrue_ShouldBeAbleToSelectMultipleItems(bool realTimeSelectionEnabled)
        {
            // arrange
            ArrangeSelectionScenario(realTimeSelectionEnabled);
            System.Collections.Generic.List<RichCanvasUITests.App.RichItemContainerModel> currentUiItems = SingleSelectionStateDataMocks.SingleSelectionItems;
            Input.WithGesture(RichCanvasGestures.Select).Click(currentUiItems[0].Center.AsDrawingPoint().ToCanvasDrawingPoint());

            // act
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            Input.WithGesture(RichCanvasGestures.Select).Click(currentUiItems[0].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            Input.WithGesture(RichCanvasGestures.Select).Click(currentUiItems[1].Center.AsDrawingPoint().ToCanvasDrawingPoint());

            // assert
            RichCanvas.SelectedItem.Should().BeNull();
            RichCanvas.SelectedItems.Length.Should().Be(2);
            // toggle button again preparing for teardown
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
        }

        private void ArrangeSelectionScenario(bool realTimeSelectionEnabled = true)
        {
            if (realTimeSelectionEnabled)
            {
                // enable real-time selection
                Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
                ReleaseRealTimeSelection = true;
                AddSingleSelectionItems();
            }
            else
            {
                AddSingleSelectionItems();
            }
        }

        private void AddSingleSelectionItems()
        {
            // add items for selection
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
        }
    }
}
