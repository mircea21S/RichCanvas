using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using RichCanvas.Gestures;
using RichCanvas.UITests.Helpers;
using RichCanvasUITests.App;
using RichCanvasUITests.App.Automation;
using RichCanvasUITests.App.Models;
using RichCanvasUITests.App.TestMocks;
using System.Linq;
using Point = System.Drawing.Point;

namespace RichCanvas.UITests.Tests
{
    [TestFixture]
    public class DrawingStateTests : RichCanvasTestAppTest
    {
        [Test]
        public void DragMouseToDraw_WhenRemovingOneItem_ShouldDrawOnlyRemainingItem()
        {
            // arrange
            var endingPointLine = PointUtilities.GetEndingPoint(ViewportCenter, 50, 50);

            // act
            // add not drawn rectangle
            Window.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);
            // add not drawn line
            Window.InvokeButton(AutomationIds.AddEmptyLineButtonId);
            // remove first item
            Window.InvokeButton(AutomationIds.RemoveFirstItemButtonId);

            // draw
            Input.WithGesture(RichCanvasGestures.Drawing).Drag(ViewportCenter, endingPointLine);
            var itemDrawn = RichItemsControl.Items[0];

            // assert
            RichItemsControl.Items.Length.Should().Be(1);
            itemDrawn.RichItemContainerData.DataContextType.Should().Be(typeof(Line));
        }

        [Test]
        public void DragMouseToDraw_WhenMovingItemsOrder_ShouldDrawItemsInOrder()
        {
            // arrange
            var endingPointLine = PointUtilities.GetEndingPoint(ViewportCenter, 50, 50);
            var endingPointRectangle = PointUtilities.GetEndingPoint(ViewportCenter.MoveX(100), 50, 50);

            // act
            // add not drawn rectangle
            Window.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);
            // add not drawn line
            Window.InvokeButton(AutomationIds.AddEmptyLineButtonId);
            // move rectangle on second position
            Window.InvokeButton(AutomationIds.MoveFirstItemToTheEndButtonId);

            // draw first item
            Input.WithGesture(RichCanvasGestures.Drawing).Drag(ViewportCenter, endingPointLine);
            var firstItemDrawn = RichItemsControl.Items[0];
            // assert
            firstItemDrawn.RichItemContainerData.DataContextType.Should().Be(typeof(Line));

            // draw second item
            Input.WithGesture(RichCanvasGestures.Drawing).Drag(ViewportCenter.MoveX(100, HorizontalDirection.LeftToRight), endingPointRectangle);
            var secondItemDrawn = RichItemsControl.Items[1];
            // assert
            secondItemDrawn.RichItemContainerData.DataContextType.Should().Be(typeof(RichItemContainerModel));
        }

        [Test]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.BottomToTop)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.BottomToTop)]
        public void DragMouseToDraw_WhenAddingItemWithAllowScaleToUpdatePositionFalse_ShouldNotModifyTopAndLeft(HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
        {
            // arrange
            var mockRectangle = DrawingStateDataMocks.ImmutablePositionedRectangleMockWithoutSize;
            var endPoint = PointUtilities.GetEndingPoint(
                new Point(mockRectangle.Left.ToInt(), mockRectangle.Top.ToInt()),
                50,
                50,
                horizontalDirection,
                verticalDirection);

            // act
            Window.InvokeButton(AutomationIds.AddImmutablePositionedRectangleButtonId);
            Input.WithGesture(RichCanvasGestures.Drawing).Click(endPoint);
            var drawnContainer = RichItemsControl.Items[0];

            // assert
            using (new AssertionScope())
            {
                drawnContainer.RichItemContainerData.Top.Should().Be(mockRectangle.Top);
                drawnContainer.RichItemContainerData.Left.Should().Be(mockRectangle.Left);
            }
        }

        [Test]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.BottomToTop)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.BottomToTop)]
        public void DragMouseToDraw_WhenAddingItemWithAllowScaleToUpdatePositionTrue_ShouldModifyLeftAndTopIfScaleIsChanged(HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
        {
            // arrange
            var rectangleMock = DrawingStateDataMocks.PositionedRectangleMockWithoutSize;
            var containerLocation = PointUtilities.GetEndingPoint(new Point(rectangleMock.Left.ToInt(), rectangleMock.Top.ToInt()),
                50,
                50,
                horizontalDirection,
                verticalDirection);

            // act
            Window.InvokeButton(AutomationIds.AddPositionedRectangleButtonId);
            Input.WithGesture(RichCanvasGestures.Drawing).Click(containerLocation);
            var drawnContainer = RichItemsControl.Items[0];

            // assert
            var expectedTop = verticalDirection switch
            {
                VerticalDirection.BottomToTop => rectangleMock.Top - drawnContainer.ActualHeight,
                VerticalDirection.TopToBottom => rectangleMock.Top,
                _ => rectangleMock.Top,
            };
            var expectedLeft = horizontalDirection switch
            {
                HorizontalDirection.LeftToRight => rectangleMock.Left,
                HorizontalDirection.RightToLeft => rectangleMock.Left - drawnContainer.ActualWidth,
                _ => rectangleMock.Left,
            };
            using (new AssertionScope())
            {
                drawnContainer.RichItemContainerData.Top.Should().Be(expectedTop);
                drawnContainer.RichItemContainerData.Left.Should().Be(expectedLeft);
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
        public void DragMouseToDraw_WhenAddingEmptyContainer_ContainerSizeIsTheDraggedSizeAndPositionIsRelativeToScaleTransform(int rectanglesCount, HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
        {
            // arrange
            var rectangleWidth = 50;
            var rectangleHeight = 50;

            var startPoint = ViewportCenter;
            var endPoint = PointUtilities.GetEndingPoint(startPoint, rectangleWidth, rectangleHeight, horizontalDirection, verticalDirection);

            // act
            var itemMouseDownPositions = new System.Windows.Point[rectanglesCount];
            for (int i = 0; i < rectanglesCount; i++)
            {
                // add rectangle to ItemsSource
                Window.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);

                // draw the rectanlge
                Input.WithGesture(RichCanvasGestures.Drawing).Drag(startPoint, endPoint);
                // save rectangle position (on click)
                var startPointRelativeToCanvas = startPoint.ToCanvasPoint();
                itemMouseDownPositions[i] = startPointRelativeToCanvas;

                // move points for the next rectangle
                startPoint = startPoint.MoveX(rectangleWidth + 1, horizontalDirection);
                endPoint = endPoint.MoveX(rectangleWidth + 1, horizontalDirection);
            }

            // assert
            using (new AssertionScope())
            {
                RichItemsControl.Items.Length.Should().Be(rectanglesCount);
                for (int i = 0; i < RichItemsControl.Items.Length; i++)
                {
                    var item = RichItemsControl.Items[i];
                    var initialMouseDownPosition = itemMouseDownPositions[i];

                    var expectedTopPosition = item.RichItemContainerData.ScaleY == 1 ? initialMouseDownPosition.Y :
                        initialMouseDownPosition.Y - item.ActualHeight;
                    var expectedLeftPosition = item.RichItemContainerData.ScaleX == 1 ? initialMouseDownPosition.X :
                        initialMouseDownPosition.X - item.ActualWidth;

                    item.RichItemContainerData.Top.Should().Be(expectedTopPosition);
                    item.RichItemContainerData.Left.Should().Be(expectedLeftPosition);
                    item.ActualWidth.Should().Be(rectangleWidth);
                    item.ActualHeight.Should().Be(rectangleHeight);
                    item.RichItemContainerData.ScaleX.Should().Be(horizontalDirection == HorizontalDirection.LeftToRight ? 1 : -1);
                    item.RichItemContainerData.ScaleY.Should().Be(verticalDirection == VerticalDirection.TopToBottom ? 1 : -1);
                }
            }
        }

        [Test]
        public void AddContainerWithBindedPositionAndSize_ShouldDrawContainerWithPositionAndSizeTheSameAsSpecified()
        {
            // arrange
            var mockRectangle = DrawingStateDataMocks.DrawnRectangleMock;

            // act
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            var drawnContainer = RichItemsControl.Items[0];

            // assert
            using (new AssertionScope())
            {
                drawnContainer.RichItemContainerData.Top.Should().Be(mockRectangle.Top);
                drawnContainer.RichItemContainerData.Left.Should().Be(mockRectangle.Left);
                drawnContainer.ActualHeight.Should().Be(mockRectangle.Height);
                drawnContainer.ActualWidth.Should().Be(mockRectangle.Width);
            }
        }

        [Test]
        public void DragMouseToDraw_WhenDrawingIsFinished_ShouldInvokeDrawEndedCommand()
        {
            // arrange
            var drawingStartPoint = ViewportCenter;
            var drawingEndPoint = new Point(drawingStartPoint.X + 50, drawingStartPoint.Y + 50);

            // act
            Window.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);
            Input.WithGesture(RichCanvasGestures.Drawing).Drag(drawingStartPoint, drawingEndPoint);
            Wait.UntilInputIsProcessed();

            // assert
            var drawingEndedTextBox = RichItemsControl.FindFirstDescendant(x => x.ByAutomationId(AutomationIds.DrawingEndedTextBoxId)).AsTextBox();
            drawingEndedTextBox.Name.Should().Be("DRAWING ENDED");
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