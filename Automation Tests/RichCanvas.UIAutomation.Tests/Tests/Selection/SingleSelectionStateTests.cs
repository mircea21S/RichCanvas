using System.Drawing;

using FlaUI.Core.Input;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.UIAutomation.Tests.Tests.Selection.SelectionModes;
using RichCanvas.UIAutomation.Tests.Utilities;

namespace RichCanvas.UIAutomation.Tests.Tests.Selection
{
    [SingleSelection]
    [TestFixture]
    public class SingleSelectionStateTests : RichCanvasTestAppTest
    {
        public bool ReleaseRealTimeSelection { get; private set; }

        [RealTimeSelection(true)]
        [Test]
        public void SelectArea_WhenRealTimeSelectionEnabled_ShouldSelectFirstInteresectingItem()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSingleSelectionTestItems();

            // act and assert
            var startPoint = new Point(30, 30);
            RichCanvas.StartSelection(startPoint);

            RichCanvas.Select(new Point(150, 150));
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]);

            RichCanvas.Select(new Point(250, 250));
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]);

            RichCanvas.Select(new Point(350, 350));
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]);

            RichCanvas.EndSelection();
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]);
        }

        [RealTimeSelection(true)]
        [Test]
        public void SelectArea_WhenRealTimeSelectionEnabled_ShouldUpdateSelectedItemIfSelectionAreaDoesNotContainCurrentOne()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSingleSelectionTestItems();

            // act and assert
            var startPoint = new Point(200, 75);
            RichCanvas.StartSelection(startPoint);

            RichCanvas.Select(new Point(80, 85));
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]);

            RichCanvas.Select(new Point(80, 330));
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]);

            RichCanvas.Select(new Point(105, 330));
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[2]);

            RichCanvas.Select(new Point(105, 260));
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[1]);

            RichCanvas.EndSelection();
        }

        [Test]
        public void ClickItems_ShouldSelectLatestClickedItem()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSingleSelectionTestItems();

            // act and assert
            foreach (RichCanvasContainerAutomation item in RichCanvas.Items)
            {
                Mouse.Click(new UIAClientAppPoint(item.Location.X + 1, item.Location.Y + 1));
                RichCanvas.SelectedItem.Should().Be(item);
            }
        }

        [RealTimeSelection(false)]
        [Test]
        public void SelectItems_WhenRealTimeSelectionDisabled_ShouldNotSelectItemWhileMouseIsMoving()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSingleSelectionTestItems();

            // act and assert
            var startPoint = new Point(30, 30);
            RichCanvas.StartSelection(startPoint);

            RichCanvas.Select(new Point(100, 100));
            RichCanvas.SelectedItem.Should().BeNull();
            RichCanvas.SelectedItems.Should().BeEmpty();

            RichCanvas.Select(new Point(200, 200));
            RichCanvas.SelectedItem.Should().BeNull();
            RichCanvas.SelectedItems.Should().BeEmpty();

            RichCanvas.Select(new Point(300, 300));
            RichCanvas.SelectedItem.Should().BeNull();
            RichCanvas.SelectedItems.Should().BeEmpty();

            RichCanvas.EndSelection();
        }

        [RealTimeSelection(false)]
        [Test]
        public void SelectItems_WhenRealTimeSelectionDisabled_ShouldSelectItemWhenMouseIsReleased()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSingleSelectionTestItems();

            // act
            var startPoint = new Point(30, 30);
            RichCanvas.StartSelection(startPoint);
            RichCanvas.Select(new Point(300, 300));
            RichCanvas.EndSelection();

            // assert
            RichCanvas.SelectedItem.Should().NotBeNull();
            RichCanvas.SelectedItems.Length.Should().Be(1);
        }

        [RealTimeSelection(false)]
        [Test]
        public void SelectItems_WhenRealTimeSelectionDisabled_ShouldSelectLastItemAddedToItemsSourceWhenMouseIsReleased()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSingleSelectionTestItems();

            // act
            var startPoint = new Point(30, 30);
            RichCanvas.StartSelection(startPoint);
            RichCanvas.Select(new Point(300, 300));
            RichCanvas.EndSelection();

            // assert
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[2]);
        }

        [Test]
        public void ProgrammaticallySetSelectedItemThroughBinding_ShouldSelectThatItem()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSingleSelectionTestItems();

            // act & assert
            RichCanvas.Items[0].IsSelected = true;
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[0]);

            RichCanvas.Items[1].IsSelected = true;
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[1]);

            RichCanvas.Items[2].IsSelected = true;
            RichCanvas.SelectedItem.Should().Be(RichCanvas.Items[2]);
        }
    }
}
