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

        [TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Down)]
        [TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Up)]
        [TestCase(ScrollingMethod.ThumbButton, ScrollingMode.Down)]
        [TestCase(ScrollingMethod.ThumbButton, ScrollingMode.Up)]
        [TestCase(ScrollingMethod.Page, ScrollingMode.Down)]
        [TestCase(ScrollingMethod.Page, ScrollingMode.Up)]
        [TestCase(ScrollingMethod.Scrollbar, ScrollingMode.Down)]
        [TestCase(ScrollingMethod.Scrollbar, ScrollingMode.Up)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Left)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Right)]
        [TestCase(ScrollingMethod.ThumbButton, ScrollingMode.Left)]
        [TestCase(ScrollingMethod.ThumbButton, ScrollingMode.Right)]
        [TestCase(ScrollingMethod.Page, ScrollingMode.Left)]
        [TestCase(ScrollingMethod.Page, ScrollingMode.Right)]
        [TestCase(ScrollingMethod.Scrollbar, ScrollingMode.Left)]
        [TestCase(ScrollingMethod.Scrollbar, ScrollingMode.Right)]
        [Test]
        public void ScrollingVerticallyAndHorizontally_WithAllScrollingMethods_ShouldTranslateTheCanvas(ScrollingMethod scrollingMethod, ScrollingMode scrollingMode)
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

        [TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Down)]
        [TestCase(ScrollingMethod.ThumbButton, ScrollingMode.Down)]
        [TestCase(ScrollingMethod.Page, ScrollingMode.Down)]
        [TestCase(ScrollingMethod.Scrollbar, ScrollingMode.Down)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Left)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Right)]
        [TestCase(ScrollingMethod.ThumbButton, ScrollingMode.Left)]
        [TestCase(ScrollingMethod.Page, ScrollingMode.Left)]
        [TestCase(ScrollingMethod.Scrollbar, ScrollingMode.Left)]
        [Test]
        public void ScrollingDownAndLeft_WithAllScrollingMethodsAndItemsAdded_ShouldHaveOffsetEqualToDifferenceBetweenViewportLocationAndItemsExtentLocation(ScrollingMethod scrollingMethod, ScrollingMode scrollingMode)
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
            if (scrollingMode == ScrollingMode.Down)
            {
                RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(Math.Max(0, scrollOffset.Y));
            }
            else if (scrollingMode == ScrollingMode.Left)
            {
                RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(Math.Max(0, scrollOffset.X));
            }
        }

        [TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Up)]
        [TestCase(ScrollingMethod.ThumbButton, ScrollingMode.Up)]
        [TestCase(ScrollingMethod.Page, ScrollingMode.Up)]
        [TestCase(ScrollingMethod.Scrollbar, ScrollingMode.Up)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Left)]
        //[TestCase(ScrollingMethod.MouseWheel, ScrollingMode.Right)]
        [TestCase(ScrollingMethod.ThumbButton, ScrollingMode.Right)]
        [TestCase(ScrollingMethod.Page, ScrollingMode.Right)]
        [TestCase(ScrollingMethod.Scrollbar, ScrollingMode.Right)]
        [Test]
        public void ScrollingUpAndRight_WithAllScrollingMethodsAndItemsAddedInsideViewportSize_ShouldHaveOffsetEqualToZeroAndExtentEqualToUnionBetweenViewportAndItemsExtent(ScrollingMethod scrollingMethod, ScrollingMode scrollingMode)
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

            if (scrollingMode == ScrollingMode.Up)
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
            else if (scrollingMode == ScrollingMode.Right)
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
        [TestCase(ScrollingMode.Down)]
        [TestCase(ScrollingMode.Up)]
        [Test]
        public void ScrollingVerticallyByScrollbarDraggingToMaximum_WithItemsInsideViewportSizeAndVisibleScrollbar_ShouldMoveAllItemsInViewport(ScrollingMode scrollingMode)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            ArrangeUIHorizontallyToShowScrollbars(scrollingMode);
            ArrangeUIVerticallyToShowScrollbars(scrollingMode);
            VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(FlaUI.Core.Definitions.ControlType.ScrollBar)).AsVerticalScrollBar();
            var verticalScrollbarBoundingRectangle = verticalScrollBar.BoundingRectangle.Location;
            var verticalScrollbarLocation = new System.Drawing.Point(verticalScrollbarBoundingRectangle.X + 2, (int)ViewportSize.Height / 2);

            // act
            Mouse.Position = verticalScrollbarLocation;
            Mouse.Down();
            if (scrollingMode == ScrollingMode.Down)
            {
                Mouse.Position = new System.Drawing.Point(verticalScrollbarLocation.X, verticalScrollbarLocation.Y - 1000);
            }
            else
            {
                Mouse.Position = new System.Drawing.Point(verticalScrollbarLocation.X, verticalScrollbarLocation.Y + 1000);
            }

            // assert
            if (scrollingMode == ScrollingMode.Down)
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

        private void ArrangeUIVerticallyToShowScrollbars(ScrollingMode scrollingMode)
        {
            while (scrollingMode == ScrollingMode.Down
                    ? RichItemsControl.RichItemsControlData.ItemsExtent.Top > ViewportLocation.Y
                    : scrollingMode == ScrollingMode.Up && RichItemsControl.RichItemsControlData.ItemsExtent.Height < ViewportSize.Height + ViewportLocation.Y)
            {
                Input.MouseWheelScroll(scrollingMode);
            }
        }

        private void ArrangeUIHorizontallyToShowScrollbars(ScrollingMode scrollingMode)
        {
            if (scrollingMode == ScrollingMode.Left)
            {
                RichItemsControl.SetScrollPercent(RichItemsControl.RichItemsControlData.ItemsExtent.Left + 10, 0);
            }
            else if (scrollingMode == ScrollingMode.Right)
            {
                RichItemsControl.SetScrollPercent(-(ViewportSize.Width - RichItemsControl.RichItemsControlData.ItemsExtent.Right) - 10, 0);
            }
        }

        private void AssertViewportLocationModified(ScrollingMode scrollingMode, ScrollingMethod scrollingMethod, Point initialViewportLocation)
        {
            var scrollFactor = GetScrollFactor(scrollingMode, scrollingMethod);
            if (scrollingMode == ScrollingMode.Up)
            {
                ViewportLocation.Y.Should().Be(initialViewportLocation.Y - scrollFactor);
            }
            else if (scrollingMode == ScrollingMode.Down)
            {
                ViewportLocation.Y.Should().Be(initialViewportLocation.Y + scrollFactor);
            }
            else if (scrollingMode == ScrollingMode.Left)
            {
                ViewportLocation.X.Should().Be(initialViewportLocation.X + scrollFactor);
            }
            else
            {
                ViewportLocation.X.Should().Be(initialViewportLocation.X - scrollFactor);
            }
        }

        private double GetScrollFactor(ScrollingMode scrollingMode, ScrollingMethod scrollingMethod)
        {
            var pageScrollFactor = scrollingMode == ScrollingMode.Up || scrollingMode == ScrollingMode.Down ? ViewportSize.Height : ViewportSize.Width;
            var scrollFactor = scrollingMethod == ScrollingMethod.Page
                ? pageScrollFactor
                : scrollingMethod == ScrollingMethod.Scrollbar
                ? 1
                : RichItemsControl.RichItemsControlData.ScrollFactor;
            return scrollFactor;
        }
    }
    public enum ScrollingMode
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
