using System;
using System.Linq;
using System.Windows;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements.Scrolling;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.UIAutomation.Tests.Helpers;

using RichCanvasUIA.Client.Automation;
using RichCanvasUIA.Client.TestMocks;

namespace RichCanvas.UIAutomation.Tests.Tests.Scrolling
{
    // TODO: Investingate horizontal mouse wheel scrolling.
    [TestFixture(false)]
    //[TestFixture(true, true)]
    //[TestFixture(true, false)]
    public class ScrollingTests : RichCanvasTestAppTest
    {
        private const double Tolerance = 1e-5;
        private readonly bool _shouldZoom;
        private readonly bool _zoomIn;

        public ScrollingTests(bool shouldZoom, bool zoomIn)
        {
            _shouldZoom = shouldZoom;
            _zoomIn = zoomIn;
        }

        public ScrollingTests(bool shouldZoom)
        {
            _shouldZoom = shouldZoom;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            if (_shouldZoom)
            {
                if (_zoomIn)
                {
                    RichCanvas.SetViewportZoom(0.3);
                }
                else
                {
                    RichCanvas.SetViewportZoom(1.5);
                }
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            RichCanvas.ResetZoom();
        }

        public override void TearDown()
        {
            base.TearDown();
            RichCanvas.ResetViewportLocation();
            Mouse.Position = VisualViewportCenter.ToCanvasDrawingPoint();
        }

        [TestCase(Direction.Down)]
        [TestCase(Direction.Up)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [Test]
        public void ScrollingVerticallyAndHorizontally_ShouldTranslateTheCanvas(Direction scrollingDirection)
        {
            // arrange
            var initialViewportLocation = RichCanvas.ViewportLocation;

            // act
            RichCanvas.ScrollByArrowKeyOrButton(scrollingDirection);

            // assert
            double scrollFactor = RichCanvas.RichCanvasData.ScrollFactor;
            if (scrollingDirection == Direction.Up)
            {
                ViewportLocation.Y.Should().BeApproximately(initialViewportLocation.Y - scrollFactor, Tolerance);
            }
            else if (scrollingDirection == Direction.Down)
            {
                ViewportLocation.Y.Should().BeApproximately(initialViewportLocation.Y + scrollFactor, Tolerance);
            }
            else if (scrollingDirection == Direction.Left)
            {
                ViewportLocation.X.Should().BeApproximately(initialViewportLocation.X + scrollFactor, Tolerance);
            }
            else
            {
                ViewportLocation.X.Should().BeApproximately(initialViewportLocation.X - scrollFactor, Tolerance);
            }
        }

        private void MouseWheelScroll(Direction scrollingDirection)
        {
            if (scrollingDirection == Direction.Up)
            {
                Mouse.Scroll(1);
            }
            else if (scrollingDirection == Direction.Down)
            {
                Mouse.Scroll(-1);
            }
            else if (scrollingDirection == Direction.Right)
            {
                Mouse.HorizontalScroll(1);
            }
            else
            {
                Mouse.HorizontalScroll(-1);
            }
        }

        [TestCase(Direction.Down)]
        [TestCase(Direction.Up)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [Test]
        public void ScrollingVerticallyAndHorizontally_WithItemDrawn_ShouldNotShowScrollbarsWhileItemIsInsideViewport(Direction scrollingDirection)
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddDrawnRectangle();

            // act
            RichCanvas.ScrollByArrowKeyOrButton(scrollingDirection);

            // assert
            if (scrollingDirection == Direction.Up || scrollingDirection == Direction.Down)
            {
                VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
                verticalScrollBar.Should().BeNull();
            }
            else
            {
                HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
                horizontalScrollBar.Should().BeNull();
            }
        }

        [TestCase(Direction.Down)]
        [TestCase(Direction.Up)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [Test]
        public void ScrollingVerticallyAndHorizontally_WithItemDrawn_ShouldShowScrollbarsWhenItemLeaveViewport(Direction scrollingDirection)
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddDrawnRectangle();
            RichCanvas.ScrollFactor = 100;

            // act
            var drawnContainer = RichCanvas.Items[0];
            do
            {
                RichCanvas.ScrollByArrowKeyOrButton(scrollingDirection);
            }
            while (ItemIsInsideViewport(drawnContainer, scrollingDirection));

            // assert
            if (scrollingDirection == Direction.Up || scrollingDirection == Direction.Down)
            {
                VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
                verticalScrollBar.Should().NotBeNull();
            }
            else
            {
                HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
                horizontalScrollBar.Should().NotBeNull();
            }
        }

        private bool ItemIsInsideViewport(RichCanvasContainerAutomation drawnContainer, Direction scrollingDirection)
        {
            return scrollingDirection switch
            {
                Direction.Down => RichCanvas.ViewportLocation.Y <= drawnContainer.Location.Y,
                Direction.Up => Math.Abs(RichCanvas.ViewportLocation.Y) <= RichCanvas.ViewportSizeInteger.Height - DrawingStateDataMocks.DrawnRectangleMock.BoundingBox.Bottom,
                Direction.Left => RichCanvas.ViewportLocation.X <= drawnContainer.Location.X,
                Direction.Right => Math.Abs(RichCanvas.ViewportLocation.X) <= RichCanvas.ViewportSizeInteger.Width - DrawingStateDataMocks.DrawnRectangleMock.BoundingBox.Right,
                _ => true,
            };
        }

        // scrollbar dragging here means actually putting the mouse over the scrollbar then dragging it (not using automation patterns)
        [TestCase(Direction.Down)]
        [TestCase(Direction.Up)]
        [Test]
        public void ScrollingVerticallyByScrollbarDraggingToMaximum_WithItemsInsideViewportSizeAndVisibleScrollbar_ShouldMoveAllItemsInViewport(Direction scrollingMode)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            ArrangeUIVerticallyToShowScrollbars(scrollingMode);
            VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
            System.Drawing.Point verticalScrollbarBoundingRectangle = verticalScrollBar.BoundingRectangle.Location;
            var verticalScrollbarLocation = new System.Drawing.Point(verticalScrollbarBoundingRectangle.X + 2, (int)VisualViewportSize.Height / 2);

            // act
            Mouse.Position = verticalScrollbarLocation;
            Mouse.Down();
            if (scrollingMode == Direction.Down)
            {
                Mouse.Position = new System.Drawing.Point(verticalScrollbarLocation.X, verticalScrollbarLocation.Y - 1000);
            }
            else
            {
                Mouse.Position = new System.Drawing.Point(verticalScrollbarLocation.X, verticalScrollbarLocation.Y + 1000);
            }

            // assert
            if (scrollingMode == Direction.Down)
            {
                RichCanvas.RichCanvasData.ViewportLocation.Y.Should().BeApproximately(RichCanvas.RichCanvasData.ItemsExtent.Top, Tolerance);
            }
            else
            {
                RichCanvas.RichCanvasData.ViewportLocation.Y.Should().BeApproximately(-(ViewportSize.Height - RichCanvas.RichCanvasData.ItemsExtent.Bottom), Tolerance);
            }
            Mouse.Up();
            Mouse.Position = ViewportCenter;
        }

        // scrollbar dragging here means actually putting the mouse over the scrollbar then dragging it (not using automation patterns)
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [Test]
        public void ScrollingHorizontallyByScrollbarDraggingToMaximum_WithItemsInsideViewportSizAndVisibleScrollbar_ShouldMoveAllItemsInViewport(Direction scrollingMode)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            RichCanvas.ResetViewportLocation();
            ArrangeUIHorizontallyToShowScrollbars(scrollingMode);
            HorizontalScrollBar horizontalScrollbar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
            System.Drawing.Point horizontalScrollbarBoundingRectangle = horizontalScrollbar.BoundingRectangle.Location;
            var horizontalScrollbarLocation = new System.Drawing.Point((int)VisualViewportSize.Width / 2, horizontalScrollbarBoundingRectangle.Y + 2);

            // act
            Mouse.Position = horizontalScrollbarLocation;
            Mouse.Down();
            if (scrollingMode == Direction.Left)
            {
                Mouse.Position = new System.Drawing.Point(horizontalScrollbarLocation.X - 1000, horizontalScrollbarLocation.Y);
            }
            else
            {
                Mouse.Position = new System.Drawing.Point(horizontalScrollbarLocation.X + 1000, horizontalScrollbarLocation.Y);
            }

            // assert
            if (scrollingMode == Direction.Left)
            {
                RichCanvas.RichCanvasData.ViewportLocation.X.Should().BeApproximately(RichCanvas.RichCanvasData.ItemsExtent.Left, Tolerance);
            }
            else
            {
                RichCanvas.RichCanvasData.ViewportLocation.X.Should().BeApproximately(-(ViewportSize.Width - RichCanvas.RichCanvasData.ItemsExtent.Right), Tolerance);
            }
            Mouse.Up();
            Mouse.Position = ViewportCenter;
        }

        // Left direction not excluded here as I can offset the starting point, because I am scrolling only once
        // scrolling multiple times would require offseting multiple times, meaning mouse up and down which will stop and restart dragging each time
        [TestCase(Direction.Up, 1)]
        [TestCase(Direction.Down, 1)]
        [TestCase(Direction.Left, 1)]
        [TestCase(Direction.Right, 1)]
        [TestCase(Direction.Up, 5)]
        [TestCase(Direction.Down, 5)]
        [TestCase(Direction.Left, 5)]
        [TestCase(Direction.Right, 5)]
        [TestCase(Direction.Up, 10)]
        [TestCase(Direction.Down, 10)]
        [TestCase(Direction.Left, 10)]
        [TestCase(Direction.Right, 10)]
        [Test]
        public void SingleDraggingItemOutsideViewport_WithRealTimeDraggingDisable_ShouldUpdateScrollOnMouseUp(Direction draggingDirection, int outsideDistance)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);

            // act
            RichCanvas.DragContainerOutsideViewportWithOffset(RichCanvas.Items[0], draggingDirection, outsideDistance, VisualViewportSize);

            // assert
            Vector scrollOffset = ViewportLocation - RichCanvas.RichCanvasData.ItemsExtent.Location;
            Rect extent = RichCanvas.RichCanvasData.ItemsExtent;
            extent.Union(new Rect(ViewportLocation, ViewportSize));

            if (draggingDirection == Direction.Up)
            {
                RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(scrollOffset.Y, Tolerance);
                RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(extent.Height, Tolerance);
            }
            if (draggingDirection == Direction.Down)
            {
                RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(extent.Height, Tolerance);
            }
            if (draggingDirection == Direction.Left)
            {
                RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(scrollOffset.X, Tolerance);
                RichCanvas.RichCanvasData.ViewportExtent.Width.Should().BeApproximately(extent.Width, Tolerance);
            }
            if (draggingDirection == Direction.Right)
            {
                RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                RichCanvas.RichCanvasData.ViewportExtent.Width.Should().BeApproximately(extent.Width, Tolerance);
            }
        }

        // exclude Left direction as full screen window is used and there's no space left
        // on the left side of the screen to drag the mouse
        [TestCase(Direction.Up, 1)]
        [TestCase(Direction.Down, 1)]
        [TestCase(Direction.Right, 1)]
        [TestCase(Direction.Up, 5)]
        [TestCase(Direction.Down, 5)]
        [TestCase(Direction.Right, 5)]
        [TestCase(Direction.Up, 10)]
        [TestCase(Direction.Down, 10)]
        [TestCase(Direction.Right, 10)]
        [Test]
        public void SingleDraggingItemOutsideViewport_WithRealTimeDraggingDisable_ShouldNotUpdateScrollOnMouseMove(Direction draggingDirection, int offsetDistance)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);

            // act
            RichCanvas.DefferedDragContainerOutsideViewportWithOffset(RichCanvas.Items[0], draggingDirection, offsetDistance, AssertScrollModification, VisualViewportSize);

            // assert
            void AssertScrollModification(System.Drawing.Point _, int offsetOnStep)
            {
                if (draggingDirection == Direction.Up)
                {
                    RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                    RichCanvas.RichCanvasData.ViewportExtent.Height.Should().Be(ViewportSize.Height);
                }
                if (draggingDirection == Direction.Down)
                {
                    RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                    RichCanvas.RichCanvasData.ViewportExtent.Height.Should().Be(ViewportSize.Height);
                }
                if (draggingDirection == Direction.Right)
                {
                    RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                    RichCanvas.RichCanvasData.ViewportExtent.Width.Should().Be(ViewportSize.Width);
                }
            }
        }

        // exclude Left direction as full screen window is used and there's no space left
        // on the left side of the screen to drag the mouse
        [TestCase(Direction.Up, 1)]
        [TestCase(Direction.Down, 1)]
        [TestCase(Direction.Right, 1)]
        [TestCase(Direction.Up, 5)]
        [TestCase(Direction.Down, 5)]
        [TestCase(Direction.Right, 5)]
        [TestCase(Direction.Up, 7)] // dragging up has the same problem and can't go on a higher value due to full screen
        [TestCase(Direction.Down, 7)]
        [TestCase(Direction.Right, 7)]
        [Test]
        public void SingleDraggingItemOutsideViewport_WithRealTimeDraggingEnable_ShouldUpdateScrollOnMouseMove(Direction draggingDirection, int offsetDistance)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);

            // act
            RichCanvas.DefferedDragContainerOutsideViewportWithOffset(RichCanvas.Items[0], draggingDirection, offsetDistance, AssertScrollModification, VisualViewportSize);

            // assert
            void AssertScrollModification(System.Drawing.Point _, int offsetOnStep)
            {
                Vector scrollOffset = ViewportLocation - RichCanvas.RichCanvasData.ItemsExtent.Location;
                Rect extent = RichCanvas.RichCanvasData.ItemsExtent;
                extent.Union(new Rect(ViewportLocation, ViewportSize));

                if (draggingDirection == Direction.Up)
                {
                    RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(scrollOffset.Y, Tolerance);
                    RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(extent.Height, Tolerance);
                }
                if (draggingDirection == Direction.Down)
                {
                    RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                    RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(extent.Height, Tolerance);
                }
                if (draggingDirection == Direction.Right)
                {
                    RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                    RichCanvas.RichCanvasData.ViewportExtent.Width.Should().BeApproximately(extent.Width, Tolerance);
                }
            }
            Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
        }

        [TestCase(Direction.Up, 1)]
        [TestCase(Direction.Down, 1)]
        [TestCase(Direction.Right, 1)]
        [TestCase(Direction.Up, 7)]
        [TestCase(Direction.Down, 7)]
        [TestCase(Direction.Right, 7)]
        [Test]
        public void DrawingContainerOutsideViewport_ShouldUpdateScrollOnMouseMove(Direction direction, int offset)
        {
            // arrange
            Window.ToggleCheckbox(AutomationIds.ShouldExecuteDrawingEndedCommandCheckboxId);

            // act
            RichCanvas.DrawEmptyContainer(VisualViewportSize, direction, offset, AssertOnMouseMove);

            // assert
            Window.ToggleCheckbox(AutomationIds.ShouldExecuteDrawingEndedCommandCheckboxId);
            void AssertOnMouseMove()
            {
                Rect expectedExtent = RichCanvas.RichCanvasData.ItemsExtent;
                expectedExtent.Union(new Rect(ViewportLocation, RichCanvas.RichCanvasData.ViewportSize));
                Vector scrollOffset = ViewportLocation - RichCanvas.RichCanvasData.ItemsExtent.Location;

                if (direction == Direction.Up)
                {
                    RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(scrollOffset.Y, Tolerance);
                    RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
                }
                if (direction == Direction.Down)
                {
                    RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                    RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
                }
                if (direction == Direction.Right)
                {
                    RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                    RichCanvas.RichCanvasData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, Tolerance);
                }
            }
        }

        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [Test]
        public void AddDrawnContainerOutsideViewport_ShouldUpdateScroll(Direction direction)
        {
            // arrange
            Window.ToggleCheckbox(AutomationIds.ShouldExecuteDrawingEndedCommandCheckboxId);

            // act
            switch (direction)
            {
                case Direction.Up:
                    Window.InvokeButton(AutomationIds.AddItemTopOutsideViewportButtonId);
                    break;

                case Direction.Down:
                    Window.InvokeButton(AutomationIds.AddItemBottomOutsideViewportButtonId);
                    break;

                case Direction.Left:
                    Window.InvokeButton(AutomationIds.AddItemLeftOutsideViewportButtonId);
                    break;

                case Direction.Right:
                    Window.InvokeButton(AutomationIds.AddItemRightOutsideViewportButtonId);
                    break;
            }

            // assert
            var addedItemLocation = new Point(RichCanvas.Items[0].RichCanvasContainerData.Left, RichCanvas.Items[0].RichCanvasContainerData.Top);
            Vector offset = ViewportLocation - addedItemLocation;
            Rect expectedExtent = RichCanvas.RichCanvasData.ItemsExtent;
            expectedExtent.Union(new Rect(ViewportLocation, ViewportSize));
            RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().Be(Math.Max(0, offset.Y));
            RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(Math.Max(0, offset.X));
            RichCanvas.RichCanvasData.ViewportExtent.Height.Should().Be(expectedExtent.Height);
            RichCanvas.RichCanvasData.ViewportExtent.Width.Should().Be(expectedExtent.Width);

            Window.ToggleCheckbox(AutomationIds.ShouldExecuteDrawingEndedCommandCheckboxId);
        }

        [Test]
        public void ResizeViewportWithItems_ToHideOrIntersectWithThem_ShouldUpdateScroll()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            double lowestItemTop = RichCanvas.Items.Max(i => i.RichCanvasContainerData.Top);
            double lowestItemRight = RichCanvas.Items.Max(i => i.RichCanvasContainerData.Left + i.ActualWidth);

            // act
            // resize by dragging the title bar
            Mouse.Click(new System.Drawing.Point((int)VisualViewportSize.Width / 2, -5));
            Mouse.Drag(new System.Drawing.Point((int)VisualViewportSize.Width / 2, -5), new System.Drawing.Point((int)VisualViewportSize.Width / 2, 0));
            Wait.UntilInputIsProcessed();

            if (Window.Patterns.Transform.TryGetPattern(out FlaUI.Core.Patterns.ITransformPattern transformPattern))
            {
                transformPattern.Resize(lowestItemTop * RichCanvas.ViewportZoom, lowestItemRight * RichCanvas.ViewportZoom);
            }

            // assert
            Vector offset = ViewportLocation - RichCanvas.RichCanvasData.ItemsExtent.Location;
            Rect expectedExtent = RichCanvas.RichCanvasData.ItemsExtent;
            expectedExtent.Union(new Rect(ViewportLocation, ViewportSize));

            RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(Math.Max(0, offset.Y), Tolerance);
            RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(Math.Max(0, offset.X), Tolerance);
            RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
            RichCanvas.RichCanvasData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, Tolerance);

            Window.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
        }

        [TestCase(Direction.Left, 5)]
        [TestCase(Direction.Up, 5)]
        [TestCase(Direction.Down, 5)]
        [TestCase(Direction.Right, 5)]
        [TestCase(Direction.Left, 7)]
        [TestCase(Direction.Up, 7)]
        [TestCase(Direction.Down, 7)]
        [TestCase(Direction.Right, 7)]
        [TestCase(Direction.Left, 1)]
        [TestCase(Direction.Up, 1)]
        [TestCase(Direction.Down, 1)]
        [TestCase(Direction.Right, 1)]
        [Test]
        public void PanningRichCanvasWithElementInDirection_ToMoveElementOutsideViewport_ShouldUpdateScroll(Direction direction, int outsideDistance)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            RichCanvas.Focus();

            // act
            RichCanvas.PanItemOutsideViewport(RichCanvas.Items[0], direction, outsideDistance, VisualViewportSize);

            // assert
            Vector offset = ViewportLocation - RichCanvas.RichCanvasData.ItemsExtent.Location;
            Rect expectedExtent = RichCanvas.RichCanvasData.ItemsExtent;
            expectedExtent.Union(new Rect(ViewportLocation, ViewportSize));

            if (direction == Direction.Up)
            {
                RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(offset.Y, Tolerance);
                RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
            }
            if (direction == Direction.Down)
            {
                RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
            }
            if (direction == Direction.Right)
            {
                RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                RichCanvas.RichCanvasData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, Tolerance);
            }
            if (direction == Direction.Left)
            {
                RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(offset.X, Tolerance);
                RichCanvas.RichCanvasData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, Tolerance);
            }
            RichCanvas.ResetViewportLocation();
        }

        private void ArrangeUIVerticallyToShowScrollbars(Direction scrollingMode)
        {
            while (scrollingMode == Direction.Down
                    ? RichCanvas.RichCanvasData.ItemsExtent.Top > ViewportLocation.Y
                    : scrollingMode == Direction.Up && RichCanvas.RichCanvasData.ItemsExtent.Height < ViewportSize.Height + ViewportLocation.Y)
            {
                Input.MouseWheelScroll(scrollingMode);
            }
        }

        private void ArrangeUIHorizontallyToShowScrollbars(Direction scrollingMode)
        {
            if (scrollingMode == Direction.Left)
            {
                RichCanvas.SetScrollPercent(RichCanvas.RichCanvasData.ItemsExtent.Left + 10, 0);
            }
            else if (scrollingMode == Direction.Right)
            {
                RichCanvas.SetScrollPercent(-(ViewportSize.Width - RichCanvas.RichCanvasData.ItemsExtent.Right) - 10, 0);
            }
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
