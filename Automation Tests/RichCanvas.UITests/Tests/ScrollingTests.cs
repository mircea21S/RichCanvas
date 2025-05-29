using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements.Scrolling;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FluentAssertions;
using NUnit.Framework;
using RichCanvas.UITests.Helpers;
using RichCanvasUITests.App.Automation;
using System;
using System.Linq;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RichCanvas.UITests.Tests
{
    // TODO: Investingate horizontal mouse wheel scrolling.
    [TestFixture(false)]
    [TestFixture(true, true)]
    [TestFixture(true, false)]
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
                    RichItemsControl.SetViewportZoom(0.3);
                }
                else
                {
                    RichItemsControl.SetViewportZoom(1.5);
                }
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            RichItemsControl.ResetZoom();
        }

        public override void TearDown()
        {
            base.TearDown();
            RichItemsControl.ResetViewportLocation();
            Mouse.Position = VisualViewportCenter.ToCanvasDrawingPoint();
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
                RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(Math.Max(0, scrollOffset.Y), Tolerance);
            }
            else if (scrollingMode == Direction.Left)
            {
                RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(Math.Max(0, scrollOffset.X), Tolerance);
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
                RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(extent.Height, Tolerance);
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
                RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().BeApproximately(extent.Width, Tolerance);
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
            VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
            var verticalScrollbarBoundingRectangle = verticalScrollBar.BoundingRectangle.Location;
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
                RichItemsControl.RichItemsControlData.ViewportLocation.Y.Should().BeApproximately(RichItemsControl.RichItemsControlData.ItemsExtent.Top, Tolerance);
            }
            else
            {
                RichItemsControl.RichItemsControlData.ViewportLocation.Y.Should().BeApproximately(-(ViewportSize.Height - RichItemsControl.RichItemsControlData.ItemsExtent.Bottom), Tolerance);
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
            RichItemsControl.ResetViewportLocation();
            ArrangeUIHorizontallyToShowScrollbars(scrollingMode);
            HorizontalScrollBar horizontalScrollbar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
            var horizontalScrollbarBoundingRectangle = horizontalScrollbar.BoundingRectangle.Location;
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
                RichItemsControl.RichItemsControlData.ViewportLocation.X.Should().BeApproximately(RichItemsControl.RichItemsControlData.ItemsExtent.Left, Tolerance);
            }
            else
            {
                RichItemsControl.RichItemsControlData.ViewportLocation.X.Should().BeApproximately(-(ViewportSize.Width - RichItemsControl.RichItemsControlData.ItemsExtent.Right), Tolerance);
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
            RichItemsControl.DragContainerOutsideViewportWithOffset(RichItemsControl.Items[0], draggingDirection, outsideDistance, VisualViewportSize);

            // assert
            var scrollOffset = ViewportLocation - RichItemsControl.RichItemsControlData.ItemsExtent.Location;
            var extent = RichItemsControl.RichItemsControlData.ItemsExtent;
            extent.Union(new Rect(ViewportLocation, ViewportSize));

            if (draggingDirection == Direction.Up)
            {
                RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(scrollOffset.Y, Tolerance);
                RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(extent.Height, Tolerance);
            }
            if (draggingDirection == Direction.Down)
            {
                RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(extent.Height, Tolerance);
            }
            if (draggingDirection == Direction.Left)
            {
                RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(scrollOffset.X, Tolerance);
                RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().BeApproximately(extent.Width, Tolerance);
            }
            if (draggingDirection == Direction.Right)
            {
                RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().BeApproximately(extent.Width, Tolerance);
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
            RichItemsControl.DefferedDragContainerOutsideViewportWithOffset(RichItemsControl.Items[0], draggingDirection, offsetDistance, AssertScrollModification, VisualViewportSize);

            // assert
            void AssertScrollModification(System.Drawing.Point _, int offsetOnStep)
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
            RichItemsControl.DefferedDragContainerOutsideViewportWithOffset(RichItemsControl.Items[0], draggingDirection, offsetDistance, AssertScrollModification, VisualViewportSize);

            // assert
            void AssertScrollModification(System.Drawing.Point _, int offsetOnStep)
            {
                var scrollOffset = ViewportLocation - RichItemsControl.RichItemsControlData.ItemsExtent.Location;
                var extent = RichItemsControl.RichItemsControlData.ItemsExtent;
                extent.Union(new Rect(ViewportLocation, ViewportSize));

                if (draggingDirection == Direction.Up)
                {
                    RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(scrollOffset.Y, Tolerance);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(extent.Height, Tolerance);
                }
                if (draggingDirection == Direction.Down)
                {
                    RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(extent.Height, Tolerance);
                }
                if (draggingDirection == Direction.Right)
                {
                    RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().BeApproximately(extent.Width, Tolerance);
                }
            }
            Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);
        }

        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [Test]
        public void MultipleDraggingItemsOutsideViewport_WithRealTimeDraggingDisabled_ShouldUpdateScrollOnMouseUp(Direction draggingDirection)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);

            // act
            Window.InvokeButton(AutomationIds.SelectAllItemsButtonId);
            RichItemsControl.DragCurrentSelectionOutsideViewport(RichItemsControl.Items[1], draggingDirection, VisualViewportSize);

            // assert
            var expectedScrollOffset = ViewportLocation - RichItemsControl.RichItemsControlData.ItemsExtent.Location;
            var expectedExtent = RichItemsControl.RichItemsControlData.ItemsExtent;
            expectedExtent.Union(new Rect(ViewportLocation, ViewportSize));

            RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(expectedScrollOffset.Y < 0 ? 0 : expectedScrollOffset.Y);
            RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(expectedScrollOffset.X < 0 ? 0 : expectedScrollOffset.X);
            RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().Be(expectedExtent.Height);
            RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().Be(expectedExtent.Width);

            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
        }

        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [Test]
        public void MultipleDraggingItemsOutsideViewport_WithRealTimeDraggingDisabled_ShouldNotUpdateScrollOnMouseMove(Direction draggingDirection)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);

            // act
            Window.InvokeButton(AutomationIds.SelectAllItemsButtonId);
            RichItemsControl.DefferedDragCurrentSelectionOutsideViewport(RichItemsControl.Items[1], draggingDirection, AssertScrollModification, VisualViewportSize);

            // assert
            void AssertScrollModification(System.Drawing.Point _, int offsetOnStep)
            {
                RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().Be(ViewportSize.Height);
                RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().Be(ViewportSize.Width);
            }

            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
        }

        [TestCase(Direction.Up, 1)]
        [TestCase(Direction.Down, 1)]
        [TestCase(Direction.Right, 1)]
        [TestCase(Direction.Up, 5)]
        [TestCase(Direction.Down, 5)]
        [TestCase(Direction.Right, 5)]
        [TestCase(Direction.Up, 7)]
        [TestCase(Direction.Down, 7)]
        [TestCase(Direction.Right, 7)]
        [Test]
        public void MultipleDraggingItemsOutsideViewport_WithRealTimeDraggingEnabled_ShouldUpdateScrollOnMouseMove(Direction draggingDirection, int offsetBetweenSteps)
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
            Window.ToggleButton(AutomationIds.RealTimeDraggingToggleButtonId);

            // act
            Window.InvokeButton(AutomationIds.SelectAllItemsButtonId);
            RichItemsControl.DefferedDragCurrentSelectionOutsideViewport(RichItemsControl.Items[1], draggingDirection, AssertScrollModification, VisualViewportSize, offsetBetweenSteps);

            // assert
            void AssertScrollModification(System.Drawing.Point _, int offsetOnStep)
            {
                var expectedScrollOffset = ViewportLocation - RichItemsControl.RichItemsControlData.ItemsExtent.Location;
                var expectedExtent = RichItemsControl.RichItemsControlData.ItemsExtent;
                expectedExtent.Union(new Rect(ViewportLocation, ViewportSize));

                RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.Y < 0 ? 0 : expectedScrollOffset.Y, Tolerance);
                RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.X < 0 ? 0 : expectedScrollOffset.X, Tolerance);
                RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
                RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, Tolerance);
            }

            Window.ToggleButton(AutomationIds.CanSelectMultipleItemsToggleButtonId);
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
            RichItemsControl.DrawEmptyContainer(VisualViewportSize, direction, offset, AssertOnMouseMove);

            // assert
            Window.ToggleCheckbox(AutomationIds.ShouldExecuteDrawingEndedCommandCheckboxId);
            void AssertOnMouseMove()
            {
                var expectedExtent = RichItemsControl.RichItemsControlData.ItemsExtent;
                expectedExtent.Union(new Rect(ViewportLocation, RichItemsControl.RichItemsControlData.ViewportSize));
                var scrollOffset = ViewportLocation - RichItemsControl.RichItemsControlData.ItemsExtent.Location;

                if (direction == Direction.Up)
                {
                    RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(scrollOffset.Y, Tolerance);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
                }
                if (direction == Direction.Down)
                {
                    RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
                }
                if (direction == Direction.Right)
                {
                    RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                    RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, Tolerance);
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
            var addedItemLocation = new Point(RichItemsControl.Items[0].RichItemContainerData.Left, RichItemsControl.Items[0].RichItemContainerData.Top);
            var offset = ViewportLocation - addedItemLocation;
            var expectedExtent = RichItemsControl.RichItemsControlData.ItemsExtent;
            expectedExtent.Union(new Rect(ViewportLocation, ViewportSize));
            RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(Math.Max(0, offset.Y));
            RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(Math.Max(0, offset.X));
            RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().Be(expectedExtent.Height);
            RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().Be(expectedExtent.Width);

            Window.ToggleCheckbox(AutomationIds.ShouldExecuteDrawingEndedCommandCheckboxId);
        }

        [Test]
        public void ResizeViewportWithItems_ToHideOrIntersectWithThem_ShouldUpdateScroll()
        {
            // arrange
            Window.InvokeButton(AutomationIds.AddSelectableItemsButtonId2);
            var lowestItemTop = RichItemsControl.Items.Max(i => i.RichItemContainerData.Top);
            var lowestItemRight = RichItemsControl.Items.Max(i => i.RichItemContainerData.Left + i.ActualWidth);

            // act
            // resize by dragging the title bar
            Mouse.Click(new System.Drawing.Point((int)VisualViewportSize.Width / 2, -5));
            Mouse.Drag(new System.Drawing.Point((int)VisualViewportSize.Width / 2, -5), new System.Drawing.Point((int)VisualViewportSize.Width / 2, 0));
            Wait.UntilInputIsProcessed();

            if (Window.Patterns.Transform.TryGetPattern(out var transformPattern))
            {
                transformPattern.Resize(lowestItemTop * RichItemsControl.ViewportZoom, lowestItemRight * RichItemsControl.ViewportZoom);
            }

            // assert
            var offset = ViewportLocation - RichItemsControl.RichItemsControlData.ItemsExtent.Location;
            var expectedExtent = RichItemsControl.RichItemsControlData.ItemsExtent;
            expectedExtent.Union(new Rect(ViewportLocation, ViewportSize));

            RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(Math.Max(0, offset.Y), Tolerance);
            RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(Math.Max(0, offset.X), Tolerance);
            RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
            RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, Tolerance);

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
            RichItemsControl.Focus();

            // act
            RichItemsControl.PanItemOutsideViewport(RichItemsControl.Items[0], direction, outsideDistance, VisualViewportSize);

            // assert
            var offset = ViewportLocation - RichItemsControl.RichItemsControlData.ItemsExtent.Location;
            var expectedExtent = RichItemsControl.RichItemsControlData.ItemsExtent;
            expectedExtent.Union(new Rect(ViewportLocation, ViewportSize));

            if (direction == Direction.Up)
            {
                RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(offset.Y, Tolerance);
                RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
            }
            if (direction == Direction.Down)
            {
                RichItemsControl.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                RichItemsControl.RichItemsControlData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
            }
            if (direction == Direction.Right)
            {
                RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, Tolerance);
            }
            if (direction == Direction.Left)
            {
                RichItemsControl.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(offset.X, Tolerance);
                RichItemsControl.RichItemsControlData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, Tolerance);
            }
            RichItemsControl.ResetViewportLocation();
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
                ViewportLocation.Y.Should().BeApproximately(initialViewportLocation.Y - scrollFactor, Tolerance);
            }
            else if (scrollingMode == Direction.Down)
            {
                ViewportLocation.Y.Should().BeApproximately(initialViewportLocation.Y + scrollFactor, Tolerance);
            }
            else if (scrollingMode == Direction.Left)
            {
                ViewportLocation.X.Should().BeApproximately(initialViewportLocation.X + scrollFactor, Tolerance);
            }
            else
            {
                ViewportLocation.X.Should().BeApproximately(initialViewportLocation.X - scrollFactor, Tolerance);
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
