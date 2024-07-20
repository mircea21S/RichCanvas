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
            base.SetUp();
            // enable multiple selection
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
        }

        public override void TearDown()
        {
            base.TearDown();
            if (ReleaseRealTimeSelection)
            {
                Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
                ReleaseRealTimeSelection = false;
            }
            // disable multiple selection
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void MultipleSelectionStateWithRealTimeSelectionEnabled_DragInSpecifiedDirection_ShouldSelectItemsWhileDragging(bool inverseDrag)
        {
            // arrange
            ArrangeRealTimeScenario();
            var currentUiItems = MultipleSelectionStateDataMocks.MultipleSelectionCloselyPositionedDummyItems;

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
        public void MultipleSelectionStateWithSelectionAbility_ClickItem_ShouldSelectItemsWhenClicked(bool realTimeSelectionEnabled)
        {
            // arrange
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario();
            }
            else
            {
                AddMultipleItemsForSelection();
            }
            var currentUiItems = MultipleSelectionStateDataMocks.MultipleSelectionCloselyPositionedDummyItems;

            // act and assert
            Input.WithGesture(RichCanvasGestures.Select)
                .Click(currentUiItems[0].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItems.Length.Should().Be(1);

            Input.WithGesture(RichCanvasGestures.Select)
               .Click(currentUiItems[1].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItems.Length.Should().Be(2);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void MultipleSelectionStateWithSelectionAbility_ClickingItems_ShouldSelectItemsWithoutUnselectingAny(bool realTimeSelectionEnabled)
        {
            // arrange
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario();
            }
            else
            {
                AddMultipleItemsForSelection();
            }
            var currentUiItems = MultipleSelectionStateDataMocks.MultipleSelectionCloselyPositionedDummyItems;

            // act and assert
            Input.WithGesture(RichCanvasGestures.Select)
                .Click(currentUiItems[0].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItems.Length.Should().Be(1);

            Input.WithGesture(RichCanvasGestures.Select)
               .Click(currentUiItems[1].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItems.Length.Should().Be(2);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void MultipleSelectionStateWithSelectionAbility_AddToSelectedItemsBindedCollection_ShouldSelectAddedItems(bool realTimeSelectionEnabled)
        {
            // arrange
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario(AutomationIds.AddSelectableItemsButtonId2);
            }
            else
            {
                AddMultipleItemsForSelection(AutomationIds.AddSelectableItemsButtonId2);
            }

            // act & assert
            Window.InvokeButton(AutomationIds.SelectFirst1ItemsNotSelectedButtonId);
            RichItemsControl.SelectedItems.Length.Should().Be(1);
            RichItemsControl.Items[0].RichItemContainerData.IsSelected.Should().BeTrue();

            Window.InvokeButton(AutomationIds.SelectFirst2ItemsNotSelectedButtonId);
            RichItemsControl.SelectedItems.Length.Should().Be(3);
            RichItemsControl.Items[0].RichItemContainerData.IsSelected.Should().BeTrue();
            RichItemsControl.Items[1].RichItemContainerData.IsSelected.Should().BeTrue();
            RichItemsControl.Items[2].RichItemContainerData.IsSelected.Should().BeTrue();

            Window.InvokeButton(AutomationIds.SelectFirst3ItemsNotSelectedButtonId);
            RichItemsControl.SelectedItems.Length.Should().Be(6);
            RichItemsControl.Items[0].RichItemContainerData.IsSelected.Should().BeTrue();
            RichItemsControl.Items[1].RichItemContainerData.IsSelected.Should().BeTrue();
            RichItemsControl.Items[2].RichItemContainerData.IsSelected.Should().BeTrue();
            RichItemsControl.Items[3].RichItemContainerData.IsSelected.Should().BeTrue();
            RichItemsControl.Items[4].RichItemContainerData.IsSelected.Should().BeTrue();
            RichItemsControl.Items[5].RichItemContainerData.IsSelected.Should().BeTrue();
        }

        [TestCase(true, 1)]
        [TestCase(true, 2)]
        [TestCase(true, 4)]
        [TestCase(false, 1)]
        [TestCase(false, 2)]
        [TestCase(false, 4)]
        [Test]
        public void MultipleSelectionStateWithSelectionAbility_SetSelectedItemsThroughBindingThenClickOnItems_ShouldSelectClickedItems(bool realTimeSelectionEnabled, int itemsClicked)
        {
            // arrange
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario(AutomationIds.AddSelectableItemsButtonId2);
            }
            else
            {
                AddMultipleItemsForSelection(AutomationIds.AddSelectableItemsButtonId2);
            }
            Window.InvokeButton(AutomationIds.SelectFirst3ItemsNotSelectedButtonId);
            // act & assert
            RichItemsControl.SelectedItems.Should().HaveCount(3);

            if (itemsClicked == 1)
            {
                Input.WithGesture(RichCanvasGestures.Select).Click(MultipleSelectionStateDataMocks.MultipleSelectionDummyItems[3].Center.AsDrawingPoint().ToCanvasDrawingPoint());
                RichItemsControl.Items[3].RichItemContainerData.IsSelected.Should().BeTrue();
            }
            else
            {
                var selectionCount = RichItemsControl.SelectedItems.Length;
                for (int i = 1; i <= itemsClicked; i++)
                {
                    Input.WithGesture(RichCanvasGestures.Select).Click(MultipleSelectionStateDataMocks.MultipleSelectionDummyItems[selectionCount + i].Center.AsDrawingPoint().ToCanvasDrawingPoint());
                    RichItemsControl.Items[selectionCount + i].RichItemContainerData.IsSelected.Should().BeTrue();
                }
            }
            RichItemsControl.SelectedItems.Should().HaveCount(3 + itemsClicked);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void MultipleSelectionStateWithSelectionAbility_SetSelectedItemsThroughBindingThenClickOnEmptyCanvas_ShouldClearSelectedItems(bool realTimeSelectionEnabled)
        {
            // arrange
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario(AutomationIds.AddSelectableItemsButtonId2);
            }
            else
            {
                AddMultipleItemsForSelection(AutomationIds.AddSelectableItemsButtonId2);
            }
            Window.InvokeButton(AutomationIds.SelectFirst5ItemsNotSelectedButtonId);

            // act & assert
            Input.WithGesture(RichCanvasGestures.Select).Click(new Point(105, 105));
            RichItemsControl.SelectedItems.Should().BeEmpty();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void MultipleSelectionStateWithRealTimeSelectionDisabled_DragInSpecifiedDirection_ShouldSelectItemsOnMouseUp(bool inverseDrag)
        {
            // arrange
            AddMultipleItemsForSelection(AutomationIds.AddSelectableItemsButtonId2);
            var currentUiItems = MultipleSelectionStateDataMocks.MultipleSelectionDummyItems;

            // act and assert
            if (inverseDrag)
            {
                var startPoint = new Point((int)currentUiItems[7].BoundingBox.Right + 1, (int)currentUiItems[7].BoundingBox.Bottom + 1);
                Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                        (currentUiItems[5].BoundingBox.TopLeft.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(0)),
                        (currentUiItems[3].BoundingBox.TopLeft.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(0)),
                        (currentUiItems[1].BoundingBox.TopLeft.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(0)),
                        (currentUiItems[0].BoundingBox.TopLeft.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(0))
                    ]);

                RichItemsControl.SelectedItems.Length.Should().Be(currentUiItems.Count);
            }
            else
            {
                var startPoint = new Point((int)currentUiItems[0].Left - 1, (int)currentUiItems[0].Top - 1);
                Input.WithGesture(RichCanvasGestures.Select).DefferedDrag(startPoint, [
                       (currentUiItems[1].BoundingBox.BottomRight.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(0)),
                       (currentUiItems[3].BoundingBox.BottomRight.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(0)),
                       (currentUiItems[5].BoundingBox.BottomRight.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(0)),
                       (currentUiItems[7].BoundingBox.BottomRight.AsDrawingPoint(), () => RichItemsControl.SelectedItems.Length.Should().Be(0))
                   ]);

                RichItemsControl.SelectedItems.Length.Should().Be(currentUiItems.Count);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void MultipleSelectionStateWithSelectedElementsAndSelectionAbility_ClearAllItems_ShouldClearSelection(bool realTimeSelectionEnabled)
        {
            // arrange
            IgnoreItemsClearOnTearDown = true;
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario(AutomationIds.AddSelectableItemsButtonId2);
            }
            else
            {
                AddMultipleItemsForSelection(AutomationIds.AddSelectableItemsButtonId2);
            }
            Window.InvokeButton(AutomationIds.SelectFirst5ItemsNotSelectedButtonId);

            // act & assert
            Window.ClearAllItems();
            RichItemsControl.SelectedItems.Should().BeEmpty();
        }

        // [] Test selection happens without problems when SelectedItems property is not binded

        private void ArrangeRealTimeScenario(string buttonId = AutomationIds.AddSelectableItemsButtonId1)
        {
            // enable real-time selection
            Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
            ReleaseRealTimeSelection = true;
            AddMultipleItemsForSelection(buttonId);
        }

        private void AddMultipleItemsForSelection(string buttonId = AutomationIds.AddSelectableItemsButtonId1)
        {
            // add items for selection
            Window.InvokeButton(buttonId);
            Wait.UntilInputIsProcessed();
        }
    }
}
