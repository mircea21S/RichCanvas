using FlaUI.Core.Input;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using RichCanvas.States;
using RichCanvas.UITests.Tests;
using RichCanvasTestApp;
using Point = System.Drawing.Point;

namespace RichCanvas.UITests
{
    [TestFixture]
    public class DrawingStateTests : RichCanvasDemoTest
    {
        [Test]
        public void RichItemContainerAllowScaleChangeToUpdatePosition_AddFakeRectangleWithLeftSetAndMouseClick_ShouldModifyTopAndSize()
        {
            // arrange
            var rectangleWidth = 350;
            var rectangleHeight = 350;
            var endPoint = new Point(rectangleWidth, rectangleHeight);

            // act
            Window.InvokeButton(AutomationIds.AddPositionedRectangleButtonId);
            Mouse.Click(endPoint);
            var drawnContainer = RichItemsControl.Items[0].AsRichItemContainerAutomation();

            // assert
            using (new AssertionScope())
            {
                RichItemsControl.RichItemsControlData.CurrentState.Should().BeOfType<DrawingState>();
                drawnContainer.RichItemContainerData.Top.Should().Be(endPoint.ToCanvasPoint().Y);
                drawnContainer.RichItemContainerData.Left.Should().Be(100);
                drawnContainer.ActualHeight.Should().Be(RichItemContainer.DefaultHeight);
                drawnContainer.ActualWidth.Should().Be(endPoint.X - 100);
            }
        }

        [Test]
        public void DrawAlreadyPositionedContainer_MouseDrag_ShouldNotModifyTopAndLeft()
        {
            // arrange
            var rectangleWidth = 350;
            var rectangleHeight = 350;
            var endPoint = new Point(rectangleWidth, rectangleHeight);

            // act
            Window.InvokeButton(AutomationIds.AddPositionedRectangleButtonId);
            Mouse.Click(endPoint);
            var drawnContainer = RichItemsControl.Items[0].AsRichItemContainerAutomation();

            // assert
            using (new AssertionScope())
            {
                RichItemsControl.RichItemsControlData.CurrentState.Should().BeOfType<DrawingState>();
                drawnContainer.RichItemContainerData.Top.Should().Be(endPoint.ToCanvasPoint().Y);
                drawnContainer.RichItemContainerData.Left.Should().Be(100);
                drawnContainer.ActualHeight.Should().Be(RichItemContainer.DefaultHeight);
                drawnContainer.ActualWidth.Should().Be(endPoint.X - 100);
            }
        }

        /// <summary>
        /// Test to draw multiple rectangles in specified <paramref name="horizontalDirection"/> and <paramref name="verticalDirection"/> and verify their ActualSize.
        /// <br></br>
        /// <i>Note: <paramref name="horizontalDirection"/> modifies <see cref="RichItemsControl.ScaleTransform"/>.ScaleX
        /// <br></br>
        /// <paramref name="verticalDirection"/> modifies <see cref="RichItemsControl.ScaleTransform"/>.ScaleY 
        /// </i>
        /// </summary>
        /// <param name="rectanglesCount"></param>
        /// <param name="horizontalDirection"></param>
        /// <param name="verticalDirection"></param>
        [Test]
        [TestCase(3, HorizontalDirection.LeftToRight, VerticalDirection.TopToBottom)]
        [TestCase(4, HorizontalDirection.LeftToRight, VerticalDirection.TopToBottom)]
        [TestCase(1, HorizontalDirection.LeftToRight, VerticalDirection.TopToBottom)]

        [TestCase(3, HorizontalDirection.RightToLeft, VerticalDirection.TopToBottom)]
        [TestCase(4, HorizontalDirection.RightToLeft, VerticalDirection.TopToBottom)]
        [TestCase(1, HorizontalDirection.RightToLeft, VerticalDirection.TopToBottom)]

        [TestCase(3, HorizontalDirection.LeftToRight, VerticalDirection.BottomToTop)]
        [TestCase(4, HorizontalDirection.LeftToRight, VerticalDirection.BottomToTop)]
        [TestCase(1, HorizontalDirection.LeftToRight, VerticalDirection.BottomToTop)]

        [TestCase(3, HorizontalDirection.RightToLeft, VerticalDirection.BottomToTop)]
        [TestCase(4, HorizontalDirection.RightToLeft, VerticalDirection.BottomToTop)]
        [TestCase(1, HorizontalDirection.RightToLeft, VerticalDirection.BottomToTop)]
        public void DrawMultipleContainers_AddButtonClickAndDragMouse_ContainerSizeIsTheDraggedSizeAndPositionRelativeToScaleTransform(int rectanglesCount, HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
        {
            var rectangleWidth = 50;
            var rectangleHeight = 50;

            var startPoint = ViewportCenter;
            var endPoint = PointUtilities.GetEndingPoint(startPoint, rectangleWidth, rectangleHeight, horizontalDirection, verticalDirection);

            // act
            for (int i = 0; i < rectanglesCount; i++)
            {
                // add rectangle to ItemsSource
                Window.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);

                // draw the rectanlge
                Mouse.Drag(startPoint, endPoint);
                var itemContainer = RichItemsControl.Items[RichItemsControl.Items.Length - 1];
                var startPointRelativeToCanvas = startPoint.ToCanvasPoint();

                var expectedTopPosition = itemContainer.RichItemContainerData.ScaleY == 1 ? startPointRelativeToCanvas.Y :
                    startPointRelativeToCanvas.Y - itemContainer.ActualHeight;
                var expectedLeftPosition = itemContainer.RichItemContainerData.ScaleX == 1 ? startPointRelativeToCanvas.X :
                    startPointRelativeToCanvas.X - itemContainer.ActualWidth;
                itemContainer.RichItemContainerData.Top.Should().Be(expectedTopPosition);
                itemContainer.RichItemContainerData.Left.Should().Be(expectedLeftPosition);

                // move points
                startPoint = startPoint.MoveX(rectangleWidth + 1, horizontalDirection);
                endPoint = endPoint.MoveX(rectangleWidth + 1, horizontalDirection);
            }

            // assert
            using (new AssertionScope())
            {
                RichItemsControl.RichItemsControlData.CurrentState.Should().BeOfType<DrawingState>();
                RichItemsControl.Items.Length.Should().Be(rectanglesCount);
                foreach (var item in RichItemsControl.Items)
                {
                    item.RichItemContainerData.ScaleX.Should().Be(horizontalDirection == HorizontalDirection.LeftToRight ? 1 : -1);
                    item.RichItemContainerData.ScaleY.Should().Be(verticalDirection == VerticalDirection.TopToBottom ? 1 : -1);
                    item.ActualWidth.Should().Be(rectangleWidth);
                    item.ActualHeight.Should().Be(rectangleHeight);
                }
            }
        }

        [Test]
        public void AddPositionedItem_ClickButtonAddDrawnRectangle_ContainerSizeIsTheSameAsSpecified()
        {
            // act
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            var drawnContainer = RichItemsControl.Items[0].AsRichItemContainerAutomation();

            // assert
            using (new AssertionScope())
            {
                // values pre-defined in RichCanvasDemo MainWindowViewModel
                drawnContainer.RichItemContainerData.Top.Should().Be(100);
                drawnContainer.RichItemContainerData.Left.Should().Be(100);
                drawnContainer.ActualHeight.Should().Be(100);
                drawnContainer.ActualWidth.Should().Be(100);
            }
        }
    }

    internal static class PointUtilities
    {
        internal static Point GetEndingPoint(Point startPoint, int rectangleWidth, int rectangleHeight, HorizontalDirection horizontalDirection = HorizontalDirection.LeftToRight, VerticalDirection verticalDirection = VerticalDirection.TopToBottom)
        {
            Point endPoint = Point.Empty;
            if (horizontalDirection == HorizontalDirection.LeftToRight && verticalDirection == VerticalDirection.TopToBottom)
            {
                endPoint = new Point(startPoint.X + rectangleWidth, startPoint.Y + rectangleHeight);
            }
            else if (horizontalDirection == HorizontalDirection.RightToLeft && verticalDirection == VerticalDirection.TopToBottom)
            {
                endPoint = new Point(startPoint.X - rectangleWidth, startPoint.Y + rectangleHeight);
            }
            if (horizontalDirection == HorizontalDirection.LeftToRight && verticalDirection == VerticalDirection.BottomToTop)
            {
                endPoint = new Point(startPoint.X + rectangleWidth, startPoint.Y - rectangleHeight);
            }
            else if (horizontalDirection == HorizontalDirection.RightToLeft && verticalDirection == VerticalDirection.BottomToTop)
            {
                endPoint = new Point(startPoint.X - rectangleWidth, startPoint.Y - rectangleHeight);
            }
            return endPoint;
        }
    }

    public enum HorizontalDirection
    {
        LeftToRight,
        RightToLeft
    }
    public enum VerticalDirection
    {
        TopToBottom,
        BottomToTop
    }
}