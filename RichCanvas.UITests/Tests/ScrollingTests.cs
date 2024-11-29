using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements.Scrolling;
using FlaUI.Core.Input;
using FluentAssertions;
using NUnit.Framework;
using RichCanvas.UITests.Helpers;
using RichCanvasUITests.App.Automation;
using System;
using System.Windows;

namespace RichCanvas.UITests.Tests
{
    // TODO: Investingate horizontal mouse wheel scrolling.
    [TestFixture]
    public class ScrollingTests : RichCanvasTestAppTest
    {
        public override void TearDown()
        {
            base.TearDown();
            Window.InvokeButton(AutomationIds.ResetViewportLocationButtonId);
        }

        [TestCase(ScrollingMethod.MouseWheel, Direction.Down)]
        [TestCase(ScrollingMethod.MouseWheel, Direction.Up)]
        [TestCase(ScrollingMethod.ThumbButton, Direction.Down)]
        [TestCase(ScrollingMethod.ThumbButton, Direction.Up)]
        [TestCase(ScrollingMethod.Page, Direction.Down)]
        [TestCase(ScrollingMethod.Page, Direction.Up)]
        [TestCase(ScrollingMethod.Scrollbar, Direction.Down)]
        [TestCase(ScrollingMethod.Scrollbar, Direction.Up)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Left)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Right)]
        [TestCase(ScrollingMethod.ThumbButton, Direction.Left)]
        [TestCase(ScrollingMethod.ThumbButton, Direction.Right)]
        [TestCase(ScrollingMethod.Page, Direction.Left)]
        [TestCase(ScrollingMethod.Page, Direction.Right)]
        [TestCase(ScrollingMethod.Scrollbar, Direction.Left)]
        [TestCase(ScrollingMethod.Scrollbar, Direction.Right)]
        [Test]
        public void ScrollingVerticallyAndHorizontally_WithAllScrollingMethods_ShouldTranslateTheCanvas(ScrollingMethod scrollingMethod, Direction scrollingMode)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            ArrangeUIHorizontallyToShowScrollbars(scrollingMode);
            var initialViewportLocation = ViewportLocation;

            // act
            if (scrollingMethod == ScrollingMethod.MouseWheel)
            {
                Input.MouseWheelScroll(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.Page)
            {
                RichItemsControl.ScrollByPage(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.Scrollbar)
            {
                // arrange the UI so scrollbars are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                initialViewportLocation = ViewportLocation;
                RichItemsControl.ScrollByScrollbarsDragging(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.ThumbButton)
            {
                // arrange the UI so arrow thumbs are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                initialViewportLocation = ViewportLocation;
                RichItemsControl.ScrollByArrowKeyOrButton(scrollingMode);
            }

            // assert
            AssertViewportLocationModified(scrollingMode, scrollingMethod, initialViewportLocation);
        }

        [TestCase(ScrollingMethod.MouseWheel, Direction.Down)]
        [TestCase(ScrollingMethod.ThumbButton, Direction.Down)]
        [TestCase(ScrollingMethod.Page, Direction.Down)]
        [TestCase(ScrollingMethod.Scrollbar, Direction.Down)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Left)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Right)]
        [TestCase(ScrollingMethod.ThumbButton, Direction.Left)]
        [TestCase(ScrollingMethod.Page, Direction.Left)]
        [TestCase(ScrollingMethod.Scrollbar, Direction.Left)]
        [Test]
        public void ScrollingDownAndLeft_WithAllScrollingMethodsAndItemsAdded_ShouldHaveOffsetEqualToDifferenceBetweenViewportLocationAndItemsExtentLocation(ScrollingMethod scrollingMethod, Direction scrollingMode)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            ArrangeUIHorizontallyToShowScrollbars(scrollingMode);

            // act
            if (scrollingMethod == ScrollingMethod.MouseWheel)
            {
                Input.MouseWheelScroll(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.Page)
            {
                RichItemsControl.ScrollByPage(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.Scrollbar)
            {
                // arrange the UI so scrollbars are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                RichItemsControl.ScrollByScrollbarsDragging(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.ThumbButton)
            {
                // arrange the UI so arrow thumbs are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                RichItemsControl.ScrollByArrowKeyOrButton(scrollingMode);
            }

            // assert
            var itemsExtent = RichItemsControl.RichItemsControlData.ItemsExtent;
            var scrollOffset = ViewportLocation - itemsExtent.Location;
            if (scrollingMode == Direction.Down)
            {
                RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(Math.Max(0, scrollOffset.Y));
            }
            else if (scrollingMode == Direction.Left)
            {
                RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(Math.Max(0, scrollOffset.X));
            }
        }

        [TestCase(ScrollingMethod.MouseWheel, Direction.Up)]
        [TestCase(ScrollingMethod.ThumbButton, Direction.Up)]
        [TestCase(ScrollingMethod.Page, Direction.Up)]
        [TestCase(ScrollingMethod.Scrollbar, Direction.Up)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Left)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Right)]
        [TestCase(ScrollingMethod.ThumbButton, Direction.Right)]
        [TestCase(ScrollingMethod.Page, Direction.Right)]
        [TestCase(ScrollingMethod.Scrollbar, Direction.Right)]
        [Test]
        public void ScrollingUpAndRight_WithAllScrollingMethodsAndItemsAddedInsideViewportSize_ShouldHaveOffsetEqualToZeroAndExtentEqualToUnionBetweenViewportAndItemsExtent(ScrollingMethod scrollingMethod, Direction scrollingMode)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            ArrangeUIHorizontallyToShowScrollbars(scrollingMode);

            // act
            if (scrollingMethod == ScrollingMethod.MouseWheel)
            {
                Input.MouseWheelScroll(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.Page)
            {
                RichItemsControl.ScrollByPage(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.Scrollbar)
            {
                // arrange the UI so scrollbars are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                RichItemsControl.ScrollByScrollbarsDragging(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.ThumbButton)
            {
                // arrange the UI so arrow thumbs are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                RichItemsControl.ScrollByArrowKeyOrButton(scrollingMode);
            }

            // assert
            var extent = RichItemsControl.RichItemsControlData.ItemsExtent;
            extent.Union(new Rect(ViewportLocation, ViewportSize));

            if (scrollingMode == Direction.Up)
            {
                if (scrollingMethod == ScrollingMethod.Scrollbar)
                {
                    RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(-GetScrollFactor(scrollingMode, scrollingMethod));
                }
                else
                {
                    RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                }
                RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().Be(extent.Height);
            }
            else if (scrollingMode == Direction.Right)
            {
                if (scrollingMethod == ScrollingMethod.Scrollbar)
                {
                    RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(-GetScrollFactor(scrollingMode, scrollingMethod));
                }
                else
                {
                    RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                }
                RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().Be(extent.Width);
            }
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
            VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(FlaUI.Core.Definitions.ControlType.ScrollBar)).AsVerticalScrollBar();
            var verticalScrollbarBoundingRectangle = verticalScrollBar.BoundingRectangle.Location;
            var verticalScrollbarLocation = new System.Drawing.Point(verticalScrollbarBoundingRectangle.X + 2, (int)ViewportSize.Height / 2);

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
                RichItemsControl.RichItemsControlData.ViewportLocation.Y.Should().Be(RichItemsControl.RichItemsControlData.ItemsExtent.Top);
            }
            else
            {
                RichItemsControl.RichItemsControlData.ViewportLocation.Y.Should().Be(-(ViewportSize.Height - RichItemsControl.RichItemsControlData.ItemsExtent.Bottom));
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
            ArrangeUIHorizontallyToShowScrollbars(scrollingMode);
            HorizontalScrollBar horizontalScrollbar = Window.FindFirstDescendant(x => x.ByControlType(FlaUI.Core.Definitions.ControlType.ScrollBar)).AsHorizontalScrollBar();
            var horizontalScrollbarBoundingRectangle = horizontalScrollbar.BoundingRectangle.Location;
            var horizontalScrollbarLocation = new System.Drawing.Point((int)ViewportSize.Width / 2, horizontalScrollbarBoundingRectangle.Y + 2);

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
                RichItemsControl.RichItemsControlData.ViewportLocation.X.Should().Be(RichItemsControl.RichItemsControlData.ItemsExtent.Left);
            }
            else
            {
                RichItemsControl.RichItemsControlData.ViewportLocation.X.Should().Be(-(ViewportSize.Width - RichItemsControl.RichItemsControlData.ItemsExtent.Right));
            }
            Mouse.Up();
            Mouse.Position = ViewportCenter;
        }

        // Left direction not excluded here as I can offset the starting point, because I am scrolling only once
        // scrolling multiple times would require offseting multiple times, meaning mouse up and down which will stop and restart dragging each time
        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [Test]
        public void SingleDraggingItemOutsideViewport_WithRealTimeDraggingDisable_ShouldUpdateScrollOnMouseUp(Direction draggingDirection)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            // distance to move the container outside
            const int OUTSIDE_DISTANCE = 1;

            // act
            RichItemsControl.DragContainerOutsideViewportWithOffset(RichItemsControl.Items[0], draggingDirection, OUTSIDE_DISTANCE);

            // assert
            if (draggingDirection == Direction.Up)
            {
                RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(OUTSIDE_DISTANCE);
                RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().Be(ViewportSize.Height + OUTSIDE_DISTANCE);
            }
            if (draggingDirection == Direction.Down)
            {
                RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().Be(ViewportSize.Height + OUTSIDE_DISTANCE);
            }
            if (draggingDirection == Direction.Left)
            {
                RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(OUTSIDE_DISTANCE);
                RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().Be(ViewportSize.Width + OUTSIDE_DISTANCE);
            }
            if (draggingDirection == Direction.Right)
            {
                RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().Be(ViewportSize.Width + OUTSIDE_DISTANCE);
            }
        }

        // exclude Left direction as full screen window is used and there's no space left
        // on the left side of the screen to drag
        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Right)]
        [Test]
        public void SingleDraggingItemOutsideViewport_WithRealTimeDraggingDisable_ShouldNotUpdateScrollOnMouseMove(Direction draggingDirection)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            // distance to move the container outside
            const int OUTSIDE_DISTANCE = 1;

            // act
            RichItemsControl.DefferedDragContainerOutsideViewportWithOffset(RichItemsControl.Items[0], draggingDirection, OUTSIDE_DISTANCE, AssertScrollModification);

            // assert
            void AssertScrollModification(System.Drawing.Point stepPoint, int offsetOnStep)
            {
                if (draggingDirection == Direction.Up)
                {
                    RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().Be(ViewportSize.Height);
                }
                if (draggingDirection == Direction.Down)
                {
                    RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().Be(ViewportSize.Height);
                }
                if (draggingDirection == Direction.Right)
                {
                    RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().Be(ViewportSize.Width);
                }
            }
        }


        // exclude Left direction as full screen window is used and there's no space left
        // on the left side of the screen to drag
        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Right)]
        [Test]
        public void SingleDraggingItemOutsideViewport_WithRealTimeDraggingEnable_ShouldUpdateScrollOnMouseMove(Direction draggingDirection)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddDrawnRectangleButtonId);
            Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
            // distance to move the container outside
            const int OUTSIDE_DISTANCE = 1;

            // act
            RichItemsControl.DefferedDragContainerOutsideViewportWithOffset(RichItemsControl.Items[0], draggingDirection, OUTSIDE_DISTANCE, AssertScrollModification);

            // assert
            void AssertScrollModification(System.Drawing.Point stepPoint, int offsetOnStep)
            {
                if (draggingDirection == Direction.Up)
                {
                    RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(offsetOnStep);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().Be(ViewportSize.Height + offsetOnStep);
                }
                if (draggingDirection == Direction.Down)
                {
                    RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().Be(ViewportSize.Height + offsetOnStep);
                }
                if (draggingDirection == Direction.Right)
                {
                    RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().Be(ViewportSize.Width + offsetOnStep);
                }
            }
            Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
        }

        private void ArrangeUIVerticallyToShowScrollbars(Direction scrollingMode)
        {
            while (scrollingMode == Direction.Down
                    ? RichItemsControl.RichItemsControlData.ItemsExtent.Top > ViewportLocation.Y
                    : scrollingMode == Direction.Up && RichItemsControl.RichItemsControlData.ItemsExtent.Height < ViewportSize.Height + ViewportLocation.Y)
            {
                Input.MouseWheelScroll(scrollingMode);
            }
        }

        private void ArrangeUIHorizontallyToShowScrollbars(Direction scrollingMode)
        {
            if (scrollingMode == Direction.Left)
            {
                RichItemsControl.SetScrollPercent(RichItemsControl.RichItemsControlData.ItemsExtent.Left + 10, 0);
            }
            else if (scrollingMode == Direction.Right)
            {
                RichItemsControl.SetScrollPercent(-(ViewportSize.Width - RichItemsControl.RichItemsControlData.ItemsExtent.Right) - 10, 0);
            }
        }

        private void AssertViewportLocationModified(Direction scrollingMode, ScrollingMethod scrollingMethod, Point initialViewportLocation)
        {
            var scrollFactor = GetScrollFactor(scrollingMode, scrollingMethod);
            if (scrollingMode == Direction.Up)
            {
                ViewportLocation.Y.Should().Be(initialViewportLocation.Y - scrollFactor);
            }
            else if (scrollingMode == Direction.Down)
            {
                ViewportLocation.Y.Should().Be(initialViewportLocation.Y + scrollFactor);
            }
            else if (scrollingMode == Direction.Left)
            {
                ViewportLocation.X.Should().Be(initialViewportLocation.X + scrollFactor);
            }
            else
            {
                ViewportLocation.X.Should().Be(initialViewportLocation.X - scrollFactor);
            }
        }

        private double GetScrollFactor(Direction scrollingMode, ScrollingMethod scrollingMethod)
        {
            var pageScrollFactor = scrollingMode == Direction.Up || scrollingMode == Direction.Down ? ViewportSize.Height : ViewportSize.Width;
            var scrollFactor = scrollingMethod == ScrollingMethod.Page
                ? pageScrollFactor
                : scrollingMethod == ScrollingMethod.Scrollbar
                ? 1
                : RichItemsControl.RichItemsControlData.ScrollFactor;
            return scrollFactor;
        }
    }
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    public enum ScrollingMethod
    {
        Page,
        MouseWheel,
        Scrollbar, // SetVerticalOffset method calls
        ThumbButton
    }
}
