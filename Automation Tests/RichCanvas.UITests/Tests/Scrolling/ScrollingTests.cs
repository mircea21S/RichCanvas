using System;
using System.Linq;
using System.Windows;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements.Scrolling;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.UITests.Helpers;

using RichCanvasUITests.App.Automation;

namespace RichCanvas.UITests.Tests.Scrolling
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
            Point initialViewportLocation = ViewportLocation;

            // act
            if (scrollingMethod == ScrollingMethod.MouseWheel)
            {
                Input.MouseWheelScroll(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.Page)
            {
                RichCanvas.ScrollByPage(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.Scrollbar)
            {
                // arrange the UI so scrollbars are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                initialViewportLocation = ViewportLocation;
                RichCanvas.ScrollByScrollbarsDragging(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.ThumbButton)
            {
                // arrange the UI so arrow thumbs are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                initialViewportLocation = ViewportLocation;
                RichCanvas.ScrollByArrowKeyOrButton(scrollingMode);
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
                RichCanvas.ScrollByPage(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.Scrollbar)
            {
                // arrange the UI so scrollbars are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                RichCanvas.ScrollByScrollbarsDragging(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.ThumbButton)
            {
                // arrange the UI so arrow thumbs are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                RichCanvas.ScrollByArrowKeyOrButton(scrollingMode);
            }

            // assert
            Rect itemsExtent = RichCanvas.RichCanvasData.ItemsExtent;
            Vector scrollOffset = ViewportLocation - itemsExtent.Location;
            if (scrollingMode == Direction.Down)
            {
                RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(Math.Max(0, scrollOffset.Y), Tolerance);
            }
            else if (scrollingMode == Direction.Left)
            {
                RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(Math.Max(0, scrollOffset.X), Tolerance);
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
                RichCanvas.ScrollByPage(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.Scrollbar)
            {
                // arrange the UI so scrollbars are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                RichCanvas.ScrollByScrollbarsDragging(scrollingMode);
            }
            else if (scrollingMethod == ScrollingMethod.ThumbButton)
            {
                // arrange the UI so arrow thumbs are visible and can be used for scrolling
                ArrangeUIVerticallyToShowScrollbars(scrollingMode);
                RichCanvas.ScrollByArrowKeyOrButton(scrollingMode);
            }

            // assert
            Rect extent = RichCanvas.RichCanvasData.ItemsExtent;
            extent.Union(new Rect(ViewportLocation, ViewportSize));

            if (scrollingMode == Direction.Up)
            {
                if (scrollingMethod == ScrollingMethod.Scrollbar)
                {
                    RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().Be(-GetScrollFactor(scrollingMode, scrollingMethod));
                }
                else
                {
                    RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                }
                RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(extent.Height, Tolerance);
            }
            else if (scrollingMode == Direction.Right)
            {
                if (scrollingMethod == ScrollingMethod.Scrollbar)
                {
                    RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(-GetScrollFactor(scrollingMode, scrollingMethod));
                }
                else
                {
                    RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                }
                RichCanvas.RichCanvasData.ViewportExtent.Width.Should().BeApproximately(extent.Width, Tolerance);
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
            RichCanvas.DragCurrentSelectionOutsideViewport(RichCanvas.Items[1], draggingDirection, VisualViewportSize);

            // assert
            Vector expectedScrollOffset = ViewportLocation - RichCanvas.RichCanvasData.ItemsExtent.Location;
            Rect expectedExtent = RichCanvas.RichCanvasData.ItemsExtent;
            expectedExtent.Union(new Rect(ViewportLocation, ViewportSize));

            RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().Be(expectedScrollOffset.Y < 0 ? 0 : expectedScrollOffset.Y);
            RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(expectedScrollOffset.X < 0 ? 0 : expectedScrollOffset.X);
            RichCanvas.RichCanvasData.ViewportExtent.Height.Should().Be(expectedExtent.Height);
            RichCanvas.RichCanvasData.ViewportExtent.Width.Should().Be(expectedExtent.Width);

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
            RichCanvas.DefferedDragCurrentSelectionOutsideViewport(RichCanvas.Items[1], draggingDirection, AssertScrollModification, VisualViewportSize);

            // assert
            void AssertScrollModification(System.Drawing.Point _, int offsetOnStep)
            {
                RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().Be(0);
                RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().Be(0);
                RichCanvas.RichCanvasData.ViewportExtent.Height.Should().Be(ViewportSize.Height);
                RichCanvas.RichCanvasData.ViewportExtent.Width.Should().Be(ViewportSize.Width);
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
            RichCanvas.DefferedDragCurrentSelectionOutsideViewport(RichCanvas.Items[1], draggingDirection, AssertScrollModification, VisualViewportSize, offsetBetweenSteps);

            // assert
            void AssertScrollModification(System.Drawing.Point _, int offsetOnStep)
            {
                Vector expectedScrollOffset = ViewportLocation - RichCanvas.RichCanvasData.ItemsExtent.Location;
                Rect expectedExtent = RichCanvas.RichCanvasData.ItemsExtent;
                expectedExtent.Union(new Rect(ViewportLocation, ViewportSize));

                RichCanvas.ScrollInfo.VerticalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.Y < 0 ? 0 : expectedScrollOffset.Y, Tolerance);
                RichCanvas.ScrollInfo.HorizontalScrollPercent.Value.Should().BeApproximately(expectedScrollOffset.X < 0 ? 0 : expectedScrollOffset.X, Tolerance);
                RichCanvas.RichCanvasData.ViewportExtent.Height.Should().BeApproximately(expectedExtent.Height, Tolerance);
                RichCanvas.RichCanvasData.ViewportExtent.Width.Should().BeApproximately(expectedExtent.Width, Tolerance);
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

        private void AssertViewportLocationModified(Direction scrollingMode, ScrollingMethod scrollingMethod, Point initialViewportLocation)
        {
            double scrollFactor = GetScrollFactor(scrollingMode, scrollingMethod);
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
            double pageScrollFactor = scrollingMode == Direction.Up || scrollingMode == Direction.Down ? ViewportSize.Height : ViewportSize.Width;
            double scrollFactor = scrollingMethod == ScrollingMethod.Page
                ? pageScrollFactor
                : scrollingMethod == ScrollingMethod.Scrollbar
                ? 1
                : RichCanvas.RichCanvasData.ScrollFactor;
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
