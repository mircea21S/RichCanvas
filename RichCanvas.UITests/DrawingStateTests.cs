using FlaUI.Core.Input;
using FlaUI.UIA3;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using RichCanvas.States;
using RichCanvas.UITests.Extensions;
using RichCanvas.UITests.Helpers;
using System.Drawing;

namespace RichCanvas.UITests
{
    [TestFixture]
    public class DrawingStateTests
    {
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
            using (var automation = new UIA3Automation())
            {
                using (var app = ApplicationHelper.AttachOrLaunchRichCanvasDemo())
                {
                    app.WaitWhileMainHandleIsMissing();

                    // arrange
                    var window = app.GetMainWindow(automation);
                    var richItemsControl = window.FindFirstDescendant(d => d.ByAutomationId("source")).AsRichItemsControlAutomation();
                    var startPoint = richItemsControl.ViewportCenter;
                    var rectangleWidth = 50;
                    var rectangleHeight = 50;
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

                    // act
                    for (int i = 0; i < rectanglesCount; i++)
                    {
                        // add rectangle to ItemsSource
                        window.AddEmptyRectangle();

                        // draw the rectanlge
                        Mouse.Drag(startPoint, endPoint);

                        // move points
                        if (horizontalDirection == HorizontalDirection.LeftToRight)
                        {
                            startPoint.X = endPoint.X + 1;
                            endPoint.X = startPoint.X + rectangleWidth;
                        }

                        if (horizontalDirection == HorizontalDirection.RightToLeft)
                        {
                            startPoint.X = endPoint.X - 1;
                            endPoint.X = startPoint.X - rectangleWidth;
                        }
                    }

                    // assert
                    using (new AssertionScope())
                    {
                        richItemsControl.RichItemsControlData.CurrentState.Should().BeOfType<DrawingState>();
                        richItemsControl.Items.Length.Should().Be(rectanglesCount);
                        foreach (var item in richItemsControl.Items)
                        {
                            item.RichItemContainerData.ScaleX.Should().Be(horizontalDirection == HorizontalDirection.LeftToRight ? 1 : -1);
                            item.RichItemContainerData.ScaleY.Should().Be(verticalDirection == VerticalDirection.TopToBottom ? 1 : -1);
                            item.ActualWidth.Should().Be(rectangleWidth);
                            item.ActualHeight.Should().Be(rectangleHeight);
                        }
                        window.ClearAllItems();
                    }
                }
            }
        }

        [Test]
        public void AddDrawnRectangle_ButtonClick_ContainerSizeIsTheSameAsSpecified()
        {
            using (var automation = new UIA3Automation())
            {
                using (var app = ApplicationHelper.AttachOrLaunchRichCanvasDemo())
                {
                    app.WaitWhileMainHandleIsMissing();

                    // arrange
                    var window = app.GetMainWindow(automation);
                    var richItemsControl = window.FindFirstDescendant(d => d.ByAutomationId("source")).AsRichItemsControlAutomation();

                    // act
                    window.AddDrawnRectangle();
                    var drawnContainer = richItemsControl.Items[0].AsRichItemContainerAutomation();

                    // assert
                    using (new AssertionScope())
                    {
                        // values pre-defined in RichCanvasDemo MainWindowViewModel
                        drawnContainer.ActualHeight.Should().Be(100);
                        drawnContainer.ActualWidth.Should().Be(100);
                        richItemsControl.RichItemsControlData.CurrentState.Should().BeNull();
                        window.ClearAllItems();
                    }
                }
            }
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