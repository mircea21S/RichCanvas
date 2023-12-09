using FlaUI.Core.Input;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using RichCanvas.States;
using RichCanvas.UITests.Tests;
using RichCanvasDemo;
using Point = System.Drawing.Point;

namespace RichCanvas.UITests
{
    [TestFixture]
    public class DrawingStateTests : RichCanvasDemoTest
    {
        [Test]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.TopToBottom)]
        public void DrawAlreadyPositionedContainer_MouseClick_ShouldNotModifyTopAndLeft(HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
        {
            //TODO: create a utitlity method for Points and Mouse to be able to drag into a specified direction
            // fluent: startPoint.DragToRight(point) -> next iteration -> previousPoint.DragToTop(point)
            // for moving the point and dragging: startPoint.Move(x,y).DragToRight(point)
            // or something like that, that could be used throughout testing and mouse using
            var rectangleWidth = 50;
            var rectangleHeight = 50;
            var endPoint = new Point(rectangleWidth, rectangleHeight);

            // act
            Window.InvokeButton(AutomationIds.AddPositionedRectangleButtonId);
            Mouse.Drag(endPoint, new Point(151, 151));
            var drawnContainer = RichItemsControl.Items[0].AsRichItemContainerAutomation();

            // assert
            using (new AssertionScope())
            {
                drawnContainer.RichItemContainerData.Top.Should().Be(100);
                drawnContainer.RichItemContainerData.Left.Should().Be(100);
                drawnContainer.ActualHeight.Should().Be(rectangleWidth - 100);
                drawnContainer.ActualWidth.Should().Be(rectangleHeight - 100);
                RichItemsControl.RichItemsControlData.CurrentState.Should().BeOfType<DrawingState>();
                Window.ClearAllItems();
            }
        }

        [Test]
        public void DrawContainer_MouseDrag_ShouldSetTopAndLeftToMouseClickPosition()
        {
            // arrange
            var richItemsControl = Window.FindFirstDescendant(d => d.ByAutomationId("source")).AsRichItemsControlAutomation();
            var startPoint = new Point(0, RichCavnasDemoTitleBarHeight);
            var rectangleWidth = 50;
            var rectangleHeight = 50;
            var endPoint = PointUtilities.GetEndingPoint(startPoint, rectangleWidth, rectangleHeight);

            // act
            Window.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);
            Mouse.Drag(startPoint, endPoint);
            var drawnContainer = richItemsControl.Items[0].AsRichItemContainerAutomation();

            // assert
            using (new AssertionScope())
            {
                drawnContainer.RichItemContainerData.Top.Should().Be(startPoint.Y - RichCavnasDemoTitleBarHeight);
                drawnContainer.RichItemContainerData.Left.Should().Be(startPoint.X);
                drawnContainer.ActualHeight.Should().Be(rectangleWidth);
                drawnContainer.ActualWidth.Should().Be(rectangleHeight);
                richItemsControl.RichItemsControlData.CurrentState.Should().BeOfType<DrawingState>();
                Window.ClearAllItems();
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
        public void DrawMultipleContainersFromRTLAndTTB_AddButtonClickAndDrag_ContainerSizeIsTheDraggedSize(int rectanglesCount, HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
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

                // move points
                startPoint.MoveX(1, horizontalDirection);
                endPoint.MoveX(rectangleWidth, horizontalDirection);
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
                Window.ClearAllItems();
            }
        }

        [Test]
        public void AddDrawnRectangle_ButtonClick_ContainerSizeIsTheSameAsSpecified()
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
                RichItemsControl.RichItemsControlData.CurrentState.Should().BeNull();
                Window.ClearAllItems();
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