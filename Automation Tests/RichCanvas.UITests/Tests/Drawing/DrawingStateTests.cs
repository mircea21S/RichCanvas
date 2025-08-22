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

using Point = System.Drawing.Point;

namespace RichCanvas.UITests.Tests.Drawing
{
    [TestFixture]
    public class DrawingStateTests : RichCanvasTestAppTest
    {
        [Test]
        public void DragMouseToDraw_WhenRemovingOneItem_ShouldDrawOnlyRemainingItem()
        {
            // arrange
            Point endingPointLine = PointUtilities.GetEndingPoint(ViewportCenter, 50, 50);
            Window.ToggleCheckbox(AutomationIds.ShouldExecuteDrawingEndedCommandCheckboxId);

            // act
            // add not drawn rectangle
            Window.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);
            // add not drawn line
            Window.InvokeButton(AutomationIds.AddEmptyLineButtonId);
            // remove first item
            Window.InvokeButton(AutomationIds.RemoveFirstItemButtonId);

            // draw
            Input.WithGesture(RichCanvasGestures.Drawing).Drag(ViewportCenter, endingPointLine);
            RichItemContainerAutomation itemDrawn = RichCanvas.Items[0];

            // assert
            RichCanvas.Items.Length.Should().Be(1);
            itemDrawn.RichCanvasContainerData.DataContextType.Should().Be(typeof(Line));
            Window.ToggleCheckbox(AutomationIds.ShouldExecuteDrawingEndedCommandCheckboxId);
        }

        [Test]
        public void DragMouseToDraw_WhenMovingItemsOrder_ShouldDrawItemsInOrder()
        {
            // arrange
            Point endingPointLine = PointUtilities.GetEndingPoint(ViewportCenter, 50, 50);
            Point endingPointRectangle = PointUtilities.GetEndingPoint(ViewportCenter.MoveX(100), 50, 50);

            // act
            // add not drawn rectangle
            Window.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);
            // add not drawn line
            Window.InvokeButton(AutomationIds.AddEmptyLineButtonId);
            // move rectangle on second position
            Window.InvokeButton(AutomationIds.MoveFirstItemToTheEndButtonId);

            // draw first item
            Input.WithGesture(RichCanvasGestures.Drawing).Drag(ViewportCenter, endingPointLine);
            RichItemContainerAutomation firstItemDrawn = RichCanvas.Items[0];
            // assert
            firstItemDrawn.RichCanvasContainerData.DataContextType.Should().Be(typeof(Line));

            // draw second item
            Input.WithGesture(RichCanvasGestures.Drawing).Drag(ViewportCenter.MoveX(100, HorizontalDirection.LeftToRight), endingPointRectangle);
            RichItemContainerAutomation secondItemDrawn = RichCanvas.Items[1];
            // assert
            secondItemDrawn.RichCanvasContainerData.DataContextType.Should().Be(typeof(RichItemContainerModel));
        }

        [Test]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.BottomToTop)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.BottomToTop)]
        public void DragMouseToDraw_WhenAddingItemWithAllowScaleToUpdatePositionFalse_ShouldNotModifyTopAndLeft(HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
        {
            // arrange
            RichItemContainerModel mockRectangle = DrawingStateDataMocks.ImmutablePositionedRectangleMockWithoutSize;
            Point endPoint = PointUtilities.GetEndingPoint(
                new Point(mockRectangle.Left.ToInt(), mockRectangle.Top.ToInt()),
                50,
                50,
                horizontalDirection,
                verticalDirection);

            // act
            Window.InvokeButton(AutomationIds.AddImmutablePositionedRectangleButtonId);
            Input.WithGesture(RichCanvasGestures.Drawing).Click(endPoint);
            RichItemContainerAutomation drawnContainer = RichCanvas.Items[0];

            // assert
            using (new AssertionScope())
            {
                drawnContainer.RichCanvasContainerData.Top.Should().Be(mockRectangle.Top);
                drawnContainer.RichCanvasContainerData.Left.Should().Be(mockRectangle.Left);
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
            RichItemContainerModel rectangleMock = DrawingStateDataMocks.PositionedRectangleMockWithoutSize;
            Point containerLocation = PointUtilities.GetEndingPoint(new Point(rectangleMock.Left.ToInt(), rectangleMock.Top.ToInt()),
                50,
                50,
                horizontalDirection,
                verticalDirection);

            // act
            Window.InvokeButton(AutomationIds.AddPositionedRectangleButtonId);
            Input.WithGesture(RichCanvasGestures.Drawing).Click(containerLocation);
            RichItemContainerAutomation drawnContainer = RichCanvas.Items[0];

            // assert
            double expectedTop = verticalDirection switch
            {
                VerticalDirection.BottomToTop => rectangleMock.Top - drawnContainer.ActualHeight,
                VerticalDirection.TopToBottom => rectangleMock.Top,
                _ => rectangleMock.Top,
            };
            double expectedLeft = horizontalDirection switch
            {
                HorizontalDirection.LeftToRight => rectangleMock.Left,
                HorizontalDirection.RightToLeft => rectangleMock.Left - drawnContainer.ActualWidth,
                _ => rectangleMock.Left,
            };
            using (new AssertionScope())
            {
                drawnContainer.RichCanvasContainerData.Top.Should().Be(expectedTop);
                drawnContainer.RichCanvasContainerData.Left.Should().Be(expectedLeft);
            }
        }

        /// <summary>
        /// Test to draw multiple rectangles in specified <paramref name="horizontalDirection"/> and <paramref name="verticalDirection"/> and verify their ActualSize.
        /// <br></br>
        /// <i>Note: <paramref name="horizontalDirection"/> modifies <see cref="RichCanvas.ScaleTransform"/>.ScaleX
        /// <br></br>
        /// <paramref name="verticalDirection"/> modifies <see cref="RichCanvas.ScaleTransform"/>.ScaleY
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
            Window.ToggleCheckbox(AutomationIds.ShouldExecuteDrawingEndedCommandCheckboxId);
            int rectangleWidth = 50;
            int rectangleHeight = 50;

            Point startPoint = ViewportCenter;
            Point endPoint = PointUtilities.GetEndingPoint(startPoint, rectangleWidth, rectangleHeight, horizontalDirection, verticalDirection);

            // act
            var itemMouseDownPositions = new System.Windows.Point[rectanglesCount];
            for (int i = 0; i < rectanglesCount; i++)
            {
                // add rectangle to ItemsSource
                Window.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);

                // draw the rectanlge
                Input.WithGesture(RichCanvasGestures.Drawing).Drag(startPoint, endPoint);
                // save rectangle position (on click)
                System.Windows.Point startPointRelativeToCanvas = startPoint.ToCanvasPoint();
                itemMouseDownPositions[i] = startPointRelativeToCanvas;

                // move points for the next rectangle
                startPoint = startPoint.MoveX(rectangleWidth + 1, horizontalDirection);
                endPoint = endPoint.MoveX(rectangleWidth + 1, horizontalDirection);
            }

            // assert
            using (new AssertionScope())
            {
                RichCanvas.Items.Length.Should().Be(rectanglesCount);
                for (int i = 0; i < RichCanvas.Items.Length; i++)
                {
                    RichItemContainerAutomation item = RichCanvas.Items[i];
                    System.Windows.Point initialMouseDownPosition = itemMouseDownPositions[i];

                    double expectedTopPosition = item.RichCanvasContainerData.ScaleY == 1 ? initialMouseDownPosition.Y :
                        initialMouseDownPosition.Y - item.ActualHeight;
                    double expectedLeftPosition = item.RichCanvasContainerData.ScaleX == 1 ? initialMouseDownPosition.X :
                        initialMouseDownPosition.X - item.ActualWidth;

                    item.RichCanvasContainerData.Top.Should().Be(expectedTopPosition);
                    item.RichCanvasContainerData.Left.Should().Be(expectedLeftPosition);
                    item.ActualWidth.Should().Be(rectangleWidth);
                    item.ActualHeight.Should().Be(rectangleHeight);
                    item.RichCanvasContainerData.ScaleX.Should().Be(horizontalDirection == HorizontalDirection.LeftToRight ? 1 : -1);
                    item.RichCanvasContainerData.ScaleY.Should().Be(verticalDirection == VerticalDirection.TopToBottom ? 1 : -1);
                }
            }
            Window.ToggleCheckbox(AutomationIds.ShouldExecuteDrawingEndedCommandCheckboxId);
        }

        [Test]
        public void AddContainerWithBindedPositionAndSize_ShouldDrawContainerWithPositionAndSizeTheSameAsSpecified()
        {
            // arrange
            RichItemContainerModel mockRectangle = DrawingStateDataMocks.DrawnRectangleMock;

            // act
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            RichItemContainerAutomation drawnContainer = RichCanvas.Items[0];

            // assert
            using (new AssertionScope())
            {
                drawnContainer.RichCanvasContainerData.Top.Should().Be(mockRectangle.Top);
                drawnContainer.RichCanvasContainerData.Left.Should().Be(mockRectangle.Left);
                drawnContainer.ActualHeight.Should().Be(mockRectangle.Height);
                drawnContainer.ActualWidth.Should().Be(mockRectangle.Width);
            }
        }

        [Test]
        public void DragMouseToDraw_WhenDrawingIsFinished_ShouldInvokeDrawEndedCommand()
        {
            // arrange
            Point drawingStartPoint = ViewportCenter;
            var drawingEndPoint = new Point(drawingStartPoint.X + 50, drawingStartPoint.Y + 50);

            // act
            Window.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);
            Input.WithGesture(RichCanvasGestures.Drawing).Drag(drawingStartPoint, drawingEndPoint);
            Wait.UntilInputIsProcessed();

            // assert
            TextBox drawingEndedTextBox = RichCanvas.FindFirstDescendant(x => x.ByAutomationId(AutomationIds.DrawingEndedTextBoxId)).AsTextBox();
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