using System.Drawing;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.UIAutomation.Tests.Tests.Selection.SelectionModes;

namespace RichCanvas.UIAutomation.Tests.Tests.Selection
{
    [TestFixture]
    public class MultipleSelectionStateTests : RichCanvasTestAppTest
    {
        [TestCase(true)]
        [TestCase(false)]
        [MultipleSelection]
        [RealTimeSelection(true)]
        [Test]
        public void SelectItems_WhenRealTimeSelectionEnabled_ShouldSelectItemsWhileMouseIsMoving(bool inverseDrag)
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddConsecutiveItemsForRealTimeSelection();
            int mostRightPoint = RichCanvas.Items.Max(x => x.BoundingRectangle.Right);
            int mostBottomPoint = RichCanvas.Items.Max(x => x.BoundingRectangle.Bottom);
            int mostLeftPoint = RichCanvas.Items.Min(x => x.Location.X);
            int mostTopPoint = RichCanvas.Items.Min(x => x.Location.Y);

            // act and assert
            if (inverseDrag)
            {
                var startPoint = new Point(mostRightPoint + 1, mostBottomPoint + 1);
                RichCanvas.StartSelection(startPoint);

                int selectedItems = 0;
                for (int i = RichCanvas.Items.Length - 1; i >= 0; i--)
                {
                    RichCanvasContainerAutomation item = RichCanvas.Items[i];
                    RichCanvas.Select(new System.Windows.Point(item.Location.X, item.Location.Y));
                    selectedItems++;
                    RichCanvas.SelectedItems.Length.Should().Be(selectedItems);
                }

                RichCanvas.EndSelection();
            }
            else
            {
                var startPoint = new Point(mostLeftPoint - 5, mostTopPoint - 5);
                RichCanvas.StartSelection(startPoint);

                int selectedItems = 0;
                for (int i = 0; i < RichCanvas.Items.Length; i++)
                {
                    RichCanvasContainerAutomation item = RichCanvas.Items[i];
                    RichCanvas.Select(new System.Windows.Point(item.BoundingRectangle.Right, item.BoundingRectangle.Bottom));
                    selectedItems++;
                    RichCanvas.SelectedItems.Length.Should().Be(selectedItems);
                }

                RichCanvas.EndSelection();
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        [MultipleSelection]
        [RealTimeSelection(false)]
        [Test]
        public void SelectItems_WhenRealTimeSelectionDisabled_ShouldNotSelectItemsWhileMouseIsMoving(bool inverseDrag)
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddConsecutiveItemsForRealTimeSelection();
            int mostRightPoint = RichCanvas.Items.Max(x => x.BoundingRectangle.Right);
            int mostBottomPoint = RichCanvas.Items.Max(x => x.BoundingRectangle.Bottom);
            int mostLeftPoint = RichCanvas.Items.Min(x => x.Location.X);
            int mostTopPoint = RichCanvas.Items.Min(x => x.Location.Y);

            // act and assert
            if (inverseDrag)
            {
                var startPoint = new Point(mostRightPoint + 1, mostBottomPoint + 1);
                RichCanvas.StartSelection(startPoint);
                for (int i = RichCanvas.Items.Length - 1; i >= 0; i--)
                {
                    RichCanvasContainerAutomation item = RichCanvas.Items[i];
                    RichCanvas.Select(new System.Windows.Point(item.Location.X, item.Location.Y));
                    RichCanvas.SelectedItems.Length.Should().Be(0);
                }

                RichCanvas.EndSelection();
            }
            else
            {
                var startPoint = new Point(mostLeftPoint - 5, mostTopPoint - 5);
                RichCanvas.StartSelection(startPoint);
                for (int i = 0; i < RichCanvas.Items.Length; i++)
                {
                    RichCanvasContainerAutomation item = RichCanvas.Items[i];
                    RichCanvas.Select(new System.Windows.Point(item.BoundingRectangle.Right, item.BoundingRectangle.Bottom));
                    RichCanvas.SelectedItems.Length.Should().Be(0);
                }

                RichCanvas.EndSelection();
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        [MultipleSelection]
        [RealTimeSelection(false)]
        [Test]
        public void SelectItems_WhenRealTimeSelectionDisabled_ShouldSelectItemsWhenMouseIsReleased(bool inverseDrag)
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddConsecutiveItemsForRealTimeSelection();
            int mostRightPoint = RichCanvas.Items.Max(x => x.BoundingRectangle.Right);
            int mostBottomPoint = RichCanvas.Items.Max(x => x.BoundingRectangle.Bottom);
            int mostLeftPoint = RichCanvas.Items.Min(x => x.Location.X);
            int mostTopPoint = RichCanvas.Items.Min(x => x.Location.Y);

            // act and assert
            if (inverseDrag)
            {
                var startPoint = new Point(mostRightPoint + 1, mostBottomPoint + 1);
                RichCanvas.StartSelection(startPoint);
                for (int i = RichCanvas.Items.Length - 1; i >= 0; i--)
                {
                    RichCanvasContainerAutomation item = RichCanvas.Items[i];
                    RichCanvas.Select(new System.Windows.Point(item.Location.X, item.Location.Y));
                }

                RichCanvas.EndSelection();
                RichCanvas.SelectedItems.Length.Should().Be(RichCanvas.Items.Length);
            }
            else
            {
                var startPoint = new Point(mostLeftPoint - 5, mostTopPoint - 5);
                RichCanvas.StartSelection(startPoint);
                for (int i = 0; i < RichCanvas.Items.Length; i++)
                {
                    RichCanvasContainerAutomation item = RichCanvas.Items[i];
                    RichCanvas.Select(new System.Windows.Point(item.BoundingRectangle.Right, item.BoundingRectangle.Bottom));
                }

                RichCanvas.EndSelection();
                RichCanvas.SelectedItems.Length.Should().Be(RichCanvas.Items.Length);
            }
        }

        [MultipleSelection]
        [RealTimeSelection(true)]
        [Test]
        public void ClickingItems_WhenRealTimeSelectionEnabled_ShouldAddAllItemsToSelection()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddConsecutiveItemsForRealTimeSelection();

            // act and assert
            int selectedItems = 0;
            foreach (RichCanvasContainerAutomation item in RichCanvas.Items)
            {
                item.Select();
                selectedItems++;
                RichCanvas.SelectedItems.Length.Should().Be(selectedItems);
            }
        }

        [MultipleSelection]
        [RealTimeSelection(false)]
        [Test]
        public void ClickingItems_WhenRealTimeSelectionDisabled_ShouldAddAllItemsToSelection()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddConsecutiveItemsForRealTimeSelection();

            // act and assert
            int selectedItems = 0;
            foreach (RichCanvasContainerAutomation item in RichCanvas.Items)
            {
                item.Select();
                selectedItems++;
                RichCanvas.SelectedItems.Length.Should().Be(selectedItems);
            }
        }

        [MultipleSelection]
        [Test]
        public void SetIsSelectedOnItems_ShouldAddAllItemsToSelection()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddConsecutiveItemsForRealTimeSelection();

            // act & assert
            foreach (RichCanvasContainerAutomation item in RichCanvas.Items)
            {
                item.IsSelected = true;
            }
            RichCanvas.SelectedItems.Should().HaveCount(RichCanvas.Items.Length);
        }
    }
}
