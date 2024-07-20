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
        public void SingleSelectionStateWithRealTimeSelectionEnabled_Drag_SelectedItemsShouldContainOnlyOneElement()
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
        public void SingleSelectionStateWithRealTimeSelectionEnabled_Drag_ShouldUpdateSelectedItemIfSelectionAreaDoesNotContainCurrentOne()
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
        public void SingleSelectionStateWithSelectionAbility_ClickItems_ShouldSelectLatestClickedItem(bool realTimeSelectionEnabled)
        {
            // arrange
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario();
            }
            else
            {
                AddSingleSelectionItems();
            }

            // act and assert
            Input.WithGesture(RichCanvasGestures.Select)
                .Click(SingleSelectionStateDataMocks.SingleSelectionItems[0].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]);

            Input.WithGesture(RichCanvasGestures.Select)
                .Click(SingleSelectionStateDataMocks.SingleSelectionItems[1].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[1]);

            Input.WithGesture(RichCanvasGestures.Select)
                .Click(SingleSelectionStateDataMocks.SingleSelectionItems[2].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[2]);
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
        public void SingleSelectionStateWithRealTimeSelectionDisabled_Drag_ShouldSelectOneItemOnMouseUp()
        {
            // arrange
            AddSingleSelectionItems();

            // act
            Input.WithGesture(RichCanvasGestures.Select).Drag(new Point(30, 30), new Point(300, 300));

            // assert
            RichItemsControl.SelectedItem.Should().NotBeNull();
            RichItemsControl.SelectedItems.Length.Should().Be(1);
        }

        [Test]
        public void SingleSelectionStateWithRealTimeSelectionDisabled_Drag_ShouldSelectLastItemAddedToItemsSourceOnMouseUp()
        {
            // arrange
            AddSingleSelectionItems();

            // act
            Input.WithGesture(RichCanvasGestures.Select).Drag(new Point(30, 30), new Point(300, 300));

            // assert
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[2]);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateWithSelectionAbility_SetSelectedItemThroughBinding_ShouldSelectThatItem(bool realTimeSelectionEnabled)
        {
            // arrange
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario();
            }
            else
            {
                AddSingleSelectionItems();
            }

            // act & assert
            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId1);
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[0]);
            RichItemsControl.SelectedItem.RichItemContainerData.IsSelected.Should().BeTrue();

            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId2);
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[1]);
            RichItemsControl.SelectedItem.RichItemContainerData.IsSelected.Should().BeTrue();

            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId3);
            RichItemsControl.SelectedItem.Should().Be(RichItemsControl.Items[2]);
            RichItemsControl.SelectedItem.RichItemContainerData.IsSelected.Should().BeTrue();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateWithSelectionAbility_SetSelectedItemThroughBindingThenClickOnOtherItem_ShouldSelectClickedItem(bool realTimeSelectionEnabled)
        {
            // arrange
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario();
            }
            else
            {
                AddSingleSelectionItems();
            }
            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId2);
            // act & assert
            RichItemsControl.SelectedItem.Should().NotBeNull();
            RichItemsControl.Items[1].RichItemContainerData.IsSelected.Should().BeTrue();

            Input.WithGesture(RichCanvasGestures.Select).Click(SingleSelectionStateDataMocks.SingleSelectionItems[1].Center.AsDrawingPoint().ToCanvasDrawingPoint());
            RichItemsControl.SelectedItem.Should().NotBeNull();
            RichItemsControl.Items[1].RichItemContainerData.IsSelected.Should().BeTrue();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateWithSelectionAbility_SetSelectedItemThroughBindingThenClickOnEmptyCanvas_ShouldClearSelectedItem(bool realTimeSelectionEnabled)
        {
            // arrange
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario();
            }
            else
            {
                AddSingleSelectionItems();
            }
            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId3);
            // act & assert
            Input.WithGesture(RichCanvasGestures.Select).Click(new Point(105, 105));
            RichItemsControl.SelectedItem.Should().BeNull();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void SingleSelectionStateWithSelectedItemAndSelectionAbility_ClearAllItems_ShouldClearSelection(bool realTimeSelectionEnabled)
        {
            // arrange
            IgnoreItemsClearOnTearDown = true;
            if (realTimeSelectionEnabled)
            {
                ArrangeRealTimeScenario();
            }
            else
            {
                AddSingleSelectionItems();
            }
            Window.InvokeButton(AutomationIds.SetSingleSelectedItemButtonId1);
            // act & assert
            Window.ClearAllItems();
            RichItemsControl.SelectedItem.Should().BeNull();
        }

        // [] Test selection happens without problems when SelectedItem property is not binded
        // [] Test SelectedItems is populated on Single Selection in case people just want to set CanSelectMultipleItems to 'false' and just basically limit SelectedItems to have a single item
        // without using the separated SelectedItem property.

        private void ArrangeRealTimeScenario()
        {
            // enable real-time selection
            Window.ToggleButton(AutomationIds.RealTimeSelectionToggleButtonId);
            ReleaseRealTimeSelection = true;
            AddSingleSelectionItems();
        }

        private void AddSingleSelectionItems()
        {
            // add items for selection
            Window.InvokeButton(AutomationIds.AddTestSingleSelectionItemsButtonId);
            Wait.UntilInputIsProcessed();
        }
    }
}
