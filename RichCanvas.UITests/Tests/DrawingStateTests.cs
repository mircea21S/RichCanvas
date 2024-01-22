using FlaUI.Core.Tools;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using RichCanvas.Gestures;
using RichCanvas.States;
using RichCanvas.UITests.Helpers;
using RichCanvas.UITests.Tests;
using RichCanvasUITests.App.Automation;
using RichCanvasUITests.App.TestMocks;
using System.Windows.Input;
using Mouse = FlaUI.Core.Input.Mouse;
using Point = System.Drawing.Point;

namespace RichCanvas.UITests
{
    [TestFixture]
    public class DrawingStateTests : RichCanvasTestAppTest
    {
        [Test]
        [TestCase(AutomationIds.AddEmptyRectangleButtonId)]
        [TestCase(AutomationIds.AddImmutablePositionedRectangleButtonId)]
        [TestCase(AutomationIds.AddPositionedRectangleButtonId)]
        public void RichItemsControl_DrawingGestureDoesNotMatchInputGesture_CurrentStateShouldBeNullOrNotDrawingState(string addDrawableContainerButtonId)
        {
            // arrange
            var wrongDrawingGesture = new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control);

            // act
            Window.InvokeButton(addDrawableContainerButtonId);
            Input.WithGesture(wrongDrawingGesture)
                .Click(ViewportCenter.AsFlaUIFixedPoint());

            // assert
            if (RichItemsControl.RichItemsControlData.CurrentState == null)
            {
                RichItemsControl.RichItemsControlData.CurrentState.Should().BeNull();
            }
            else
            {
                RichItemsControl.RichItemsControlData.CurrentState.Should().NotBeOfType<DrawingState>();
            }
        }


        [Test]
        [TestCase(AutomationIds.AddEmptyRectangleButtonId)]
        [TestCase(AutomationIds.AddImmutablePositionedRectangleButtonId)]
        [TestCase(AutomationIds.AddPositionedRectangleButtonId)]
        public void RichItemsControl_DrawingGestureDoesNotMatchInputGesture_AddedItemShouldNotBeDrawn(string addDrawableContainerButtonId)
        {
            // arrange
            var wrongDrawingGesture = new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control);

            // act
            Window.InvokeButton(addDrawableContainerButtonId);
            var startPoint = ViewportCenter.AsFlaUIFixedPoint();
            var endPoint = PointUtilities.GetEndingPoint(startPoint, 100, 100);
            Input.WithGesture(wrongDrawingGesture)
                .Drag(startPoint, endPoint);

            // assert
            using (new AssertionScope())
            {
                RichItemsControl.Items[0].ActualHeight.Should().Be(0);
                RichItemsControl.Items[0].ActualWidth.Should().Be(0);
            }
        }

        /// <summary>
        /// Drawable Container is a <see cref="RichItemContainer"/> where <see cref="RichItemContainer.IsValid()"/> is false.
        /// <br/>
        /// <paramref name="addDrawableContainerButtonId"/> specifies different buttons to be invoked, that are adding a Drawable container.
        /// </summary>
        [Test]
        [TestCase(AutomationIds.AddPositionedRectangleButtonId, false)]
        [TestCase(AutomationIds.AddEmptyRectangleButtonId, false)]
        [TestCase(AutomationIds.AddImmutablePositionedRectangleButtonId, true)]
        [TestCase(AutomationIds.AddEmptyRectangleButtonId, true)]
        public void RichItemsControl_DrawingGestureMatchesInputGesture_CurrentStateShouldAlwaysBeDrawingState(string addDrawableContainerButtonId, bool shouldClick)
        {
            // strange bug on the first test case drag if moved down in order -> probably a double click is happening and the item is not drawn
            // moved it first to solve the problem
            // might need more investigation

            // act
            Window.InvokeButton(addDrawableContainerButtonId);
            if (shouldClick)
            {
                Input.WithGesture(RichCanvasGestures.Drawing)
                    .Click(ViewportCenter.AsFlaUIFixedPoint());
            }
            else
            {
                var startPoint = ViewportCenter.AsFlaUIFixedPoint();
                var endPoint = PointUtilities.GetEndingPoint(startPoint, 100, 100);
                Input.WithGesture(RichCanvasGestures.Drawing)
                    .Drag(startPoint, endPoint);
            }

            // assert
            RichItemsControl.RichItemsControlData.CurrentState.Should().BeOfType<DrawingState>();
        }

        [Test]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.TopToBottom)]
        [TestCase(HorizontalDirection.LeftToRight, VerticalDirection.BottomToTop)]
        [TestCase(HorizontalDirection.RightToLeft, VerticalDirection.BottomToTop)]
        public void RichItemsControlAndDrawingState_AddItemWithAllowScaleToUpdatePositionFalse_ShouldNotModifyTopAndLeft(HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
        {
            // arrange
            var mockRectangle = RichItemContainerModelMocks.ImmutablePositionedRectangleMock;
            var endPoint = PointUtilities.GetEndingPoint(
                new Point(mockRectangle.Left.ToInt(), mockRectangle.Top.ToInt()),
                50,
                50,
                horizontalDirection,
                verticalDirection);

            // act
            Window.InvokeButton(AutomationIds.AddImmutablePositionedRectangleButtonId);
            Mouse.Click(endPoint);
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
        public void RichItemsControlAndDrawingState_AddItemWithAllowScaleToUpdatePositionTrue_ShouldModifyLeftAndTopIfScaleIsChanged(HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
        {
            // arrange
            var rectangleMock = RichItemContainerModelMocks.PositionedRectangleMock;
            var containerLocation = PointUtilities.GetEndingPoint(new Point(rectangleMock.Left.ToInt(), rectangleMock.Top.ToInt()),
                50,
                50,
                horizontalDirection,
                verticalDirection);

            // act
            Window.InvokeButton(AutomationIds.AddPositionedRectangleButtonId);
            Mouse.Click(containerLocation);
            var drawnContainer = RichItemsControl.Items[0];

            // assert
            var expectedTop = verticalDirection switch
            {
                // added +0.5 as it seems that ActualHeight is approximated to the upper value
                // could use the actual serialized Height in RichItemContainerData
                VerticalDirection.BottomToTop => rectangleMock.Top - drawnContainer.ActualHeight + 0.5,
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
        public void RichItemsControlAndDrawingState_AddEmptyContainer_ContainerSizeIsTheDraggedSizeAndPositionIsRelativeToScaleTransform(int rectanglesCount, HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
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
                Mouse.Drag(startPoint, endPoint);
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
        public void RichItemsControlAndDrawingState_AddItemWithBindedPositionAndSize_ShouldHavePositionAndSizeTheSameAsSpecified()
        {
            // arrange
            var mockRectangle = RichItemContainerModelMocks.DrawnRectangleMock;

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