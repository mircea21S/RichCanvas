using FluentAssertions;
using NUnit.Framework;
using RichCanvas.UITests.Helpers;
using RichCanvasUITests.App.Automation;
using System.Windows;

namespace RichCanvas.UITests.Tests
{
    // TODO: Investingate horizontal mouse wheel scrolling.
    [TestFixture]
    public class ScrollingTests : RichCanvasTestAppTest
    {
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
            var initialViewportLocation = RichItemsControl.RichItemsControlData.ViewportLocation;

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
                ArrangeUIVerticallyIToShowScrollbars(scrollingMode);
                initialViewportLocation = RichItemsControl.RichItemsControlData.ViewportLocation;
                RichItemsControl.ScrollByScrollbarsDragging(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.ThumbButton)
            {
                // arrange the UI so arrow thumbs are visible and can be used for scrolling
                ArrangeUIVerticallyIToShowScrollbars(scrollingMode);
                initialViewportLocation = RichItemsControl.RichItemsControlData.ViewportLocation;
                RichItemsControl.ScrollByArrowKeyOrButton(scrollingMode);
            }

            // assert
            AssertViewportLocationModified(scrollingMode, scrollingMethod, initialViewportLocation);
            Window.InvokeButton(AutomationIds.ResetViewportLocationButtonId);
        }

        [TestCase(ScrollingMode.Up)]
        [Test]
        public void ScrollUpByMethod_WIthItemsAdded_ShouldStartScrollingWhenItemsExtentTopIsGreaterThanViewportHieght(ScrollingMode scrollingMode)
        {

        }

        private void ArrangeUIVerticallyIToShowScrollbars(ScrollingMode scrollingMode)
        {
            while (scrollingMode == ScrollingMode.Down
                    ? RichItemsControl.RichItemsControlData.ItemsExtent.Top > RichItemsControl.RichItemsControlData.ViewportLocation.Y
                    : scrollingMode == ScrollingMode.Up && RichItemsControl.RichItemsControlData.ItemsExtent.Height < RichItemsControl.RichItemsControlData.ViewportSize.Height + RichItemsControl.RichItemsControlData.ViewportLocation.Y)
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
                RichItemsControl.SetScrollPercent(-(RichItemsControl.RichItemsControlData.ViewportSize.Width - RichItemsControl.RichItemsControlData.ItemsExtent.Right) - 10, 0);
            }
        }

        private void AssertViewportLocationModified(ScrollingMode scrollingMode, ScrollingMethod scrollingMethod, Point initialViewportLocation)
        {
            var viewportLocation = RichItemsControl.RichItemsControlData.ViewportLocation;
            var pageScrollFactor = scrollingMode == ScrollingMode.Up || scrollingMode == ScrollingMode.Down ? RichItemsControl.RichItemsControlData.ViewportSize.Height : RichItemsControl.RichItemsControlData.ViewportSize.Width;
            var scrollFactor = scrollingMethod == ScrollingMethod.Page
                ? pageScrollFactor
                : scrollingMethod == ScrollingMethod.Scrollbar
                ? 1
                : RichItemsControl.RichItemsControlData.ScrollFactor;
            if (scrollingMode == ScrollingMode.Up)
            {
                viewportLocation.Y.Should().Be(initialViewportLocation.Y - scrollFactor);
            }
            else if (scrollingMode == ScrollingMode.Down)
            {
                viewportLocation.Y.Should().Be(initialViewportLocation.Y + scrollFactor);
            }
            else if (scrollingMode == ScrollingMode.Left)
            {
                viewportLocation.X.Should().Be(initialViewportLocation.X + scrollFactor);
            }
            else
            {
                viewportLocation.X.Should().Be(initialViewportLocation.X - scrollFactor);
            }
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
