using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using RichCanvas.States;
using RichCanvas.UITests.Tests;
using RichCanvasTestApp;
using RichCanvasTestApp.Common;
using RichCanvasTestApp.ViewModels;
using RichCanvasTestApp.ViewModels.Base;
using RichCanvasTestApp.ViewModelsMocks;
using Point = System.Drawing.Point;

namespace RichCanvas.UITests
{
    [TestFixture]
    public class DrawingStateTests : RichCanvasTestAppTest
    {
        [Test]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.BottomToTop)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.BottomToTop)]
        public void RichItemContainerAllowScaleToUpdatePositionFalse_AddWithLeftSetAndMouseClick_ShouldNotModifyTopAndLeft(HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
        {
            // arrange
            var mockRectangle = RectangleMock.FakeImmutableRectangleWithTopAndLeftSet;
            var endPoint = PointUtilities.GetEndingPoint(
                // using this as start point to get it click on mockRectangle: (300,300)
                // on this test multiplying by 2 gets the desired point on UI.
                new Point((int)((mockRectangle.Left * 2) + 50), (int)((mockRectangle.Top * 2) + 50)),
                200,
                200,
                horizontalDirection,
                verticalDirection);

            // act
            Window.InvokeButton(AutomationIds.AddImmutablePositionedRectangleButtonId);
            Mouse.Click(endPoint);
            var drawnContainer = RichItemsControl.Items[0];

            // assert
            using (new AssertionScope())
            {
                RichItemsControl.RichItemsControlData.CurrentState.Should().BeOfType<DrawingState>();
                drawnContainer.RichItemContainerData.Top.Should().Be(mockRectangle.Top);
                drawnContainer.RichItemContainerData.Left.Should().Be(mockRectangle.Left);
            }
        }

        [Test]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.BottomToTop)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.BottomToTop)]
        public void RichItemContainerAllowScaleToUpdatePositionTrue_AddPositionedItemAndMouseClick_ShouldModifyLeftAndTopIfScaleIsChanged(HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
        {
            // arrange
            var x = new Drawable();
            //var containerLocation = PointUtilities.GetEndingPoint(new Point(100d.ToInt(), 100d.ToInt()),
            //    50,
            //    50,
            //    horizontalDirection,
            //    verticalDirection);

            // act
            Window.InvokeButton(AutomationIds.AddPositionedRectangleButtonId);
            Mouse.Click(new Point(100, 100));
            var drawnContainer = RichItemsControl.Items[0];

            // assert
            //var expectedTop = verticalDirection switch
            //{
            //    VerticalDirection.BottomToTop => rectangleMock.Top - drawnContainer.ActualHeight,
            //    VerticalDirection.TopToBottom => rectangleMock.Top,
            //    _ => rectangleMock.Top,
            //};
            //var expectedLeft = horizontalDirection switch
            //{
            //    HorizontalDirection.LeftToRight => rectangleMock.Left,
            //    HorizontalDirection.RightToLeft => rectangleMock.Left - drawnContainer.ActualWidth,
            //    _ => rectangleMock.Left,
            //};
            //var expectedCanvasLocation = new System.Windows.Point(expectedLeft, expectedTop).ToCanvasPoint();
            //using (new AssertionScope())
            //{
            //    drawnContainer.RichItemContainerData.Top.Should().Be(expectedCanvasLocation.Y);
            //    drawnContainer.RichItemContainerData.Left.Should().Be(expectedCanvasLocation.X);
            //}
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
        public void AddDefinedRichItemContainer_ClickButtonAddDrawnRectangle_ContainerSizeAndPositionAreTheSameAsSpecified()
        {
            // act
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            var drawnContainer = RichItemsControl.Items[0];

            // assert
            using (new AssertionScope())
            {
                // TODO: use values pre-defined in RichCanvasDemo MainWindowViewModel stub
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