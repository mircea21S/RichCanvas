using System;
using System.Drawing;
using System.Linq;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements.Scrolling;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.UIAutomation.FlaUIClient;
using RichCanvas.UIAutomation.Tests.Extensions;
using RichCanvas.UIAutomation.Tests.Tests.Dragging;
using RichCanvas.UIAutomation.Tests.Tests.Drawing;
using RichCanvas.UIAutomation.Tests.Utilities;

using RichCanvasUIA.Client.UIA_Mode;

namespace RichCanvas.UIAutomation.Tests.Tests.Scrolling
{
    // TODO: Investingate horizontal mouse wheel scrolling.
    public class ScrollingTests : RichCanvasTestAppTest
    {
        public override void TearDown()
        {
            base.TearDown();
            RichCanvas.ViewportLocation = new Point(0, 0);
        }

        [TestCase(Direction.Down)]
        [TestCase(Direction.Up)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [Test]
        public void ScrollingVerticallyAndHorizontally_ShouldTranslateTheCanvas(Direction scrollingDirection)
        {
            // arrange
            Point initialViewportLocation = RichCanvas.ViewportLocation;

            // act
            RichCanvas.ScrollByArrowKeyOrButton(scrollingDirection);

            // assert
            double scrollFactor = RichCanvas.ScrollFactor;
            if (scrollingDirection == Direction.Up)
            {
                RichCanvas.ViewportLocation.Y.Should().Be((initialViewportLocation.Y - scrollFactor).ToInt());
            }
            else if (scrollingDirection == Direction.Down)
            {
                RichCanvas.ViewportLocation.Y.Should().Be((initialViewportLocation.Y + scrollFactor).ToInt());
            }
            else if (scrollingDirection == Direction.Left)
            {
                RichCanvas.ViewportLocation.X.Should().Be((initialViewportLocation.X + scrollFactor).ToInt());
            }
            else
            {
                RichCanvas.ViewportLocation.X.Should().Be((initialViewportLocation.X - scrollFactor).ToInt());
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
            ScrollbarShouldNotBeVisible(scrollingDirection);
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
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            do
            {
                RichCanvas.ScrollByArrowKeyOrButton(scrollingDirection);
                // assert
                if (ItemIsInsideViewport(drawnContainer, scrollingDirection))
                {
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
            }
            while (ItemIsInsideViewport(drawnContainer, scrollingDirection));

            // assert
            ScrollbarShouldBeVisible(scrollingDirection);
        }

        private bool ItemIsInsideViewport(RichCanvasContainerAutomation drawnContainer, Direction scrollingDirection)
        {
            return scrollingDirection switch
            {
                Direction.Down => RichCanvas.ViewportLocation.Y <= drawnContainer.Location.Y,
                Direction.Up => Math.Abs(RichCanvas.ViewportLocation.Y) <= RichCanvas.ViewportSize.Height - PreDefinedAutomationItemModels.FullyDrawnRectangle.BoundingBox.Bottom,
                Direction.Left => RichCanvas.ViewportLocation.X <= drawnContainer.Location.X,
                Direction.Right => Math.Abs(RichCanvas.ViewportLocation.X) <= RichCanvas.ViewportSize.Width - PreDefinedAutomationItemModels.FullyDrawnRectangle.BoundingBox.Right,
                _ => true,
            };
        }

        // scrollbar dragging here means actually putting the mouse over the scrollbar then dragging it (not using automation patterns)
        [TestCase(Direction.Down)]
        [TestCase(Direction.Up)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [Test]
        public void UsingScrollbars_WhenItemsOutsideViewport_ShouldBringAllItemsOnDirectionInsideViewport(Direction direction)
        {
            // arrange
            switch (direction)
            {
                case Direction.Down:
                    RichCanvasUIAClientCommunicator.AddItemTopOutsideViewport();
                    break;

                case Direction.Up:
                    RichCanvasUIAClientCommunicator.AddItemBottomOutsideViewport();
                    break;

                case Direction.Left:
                    RichCanvasUIAClientCommunicator.AddItemLeftOutsideViewport();
                    break;

                case Direction.Right:
                    RichCanvasUIAClientCommunicator.AddItemRightOutsideViewport();
                    break;
            }

            // act
            RichCanvas.ScrollByScrollbarsDragging(direction);

            // assert
            if (direction == Direction.Down)
            {
                RichCanvas.ViewportLocation.Y.Should().Be(RichCanvas.GetRichCanvasSettings().ItemsExtent.Top.ToInt());
            }
            else if (direction == Direction.Up)
            {
                RichCanvas.ViewportLocation.Y.Should().Be(-(RichCanvas.ViewportSize.Height - RichCanvas.GetRichCanvasSettings().ItemsExtent.Bottom).ToInt());
            }
            else if (direction == Direction.Left)
            {
                RichCanvas.ViewportLocation.X.Should().Be(RichCanvas.GetRichCanvasSettings().ItemsExtent.Left.ToInt());
            }
            else
            {
                RichCanvas.ViewportLocation.X.Should().Be(-(RichCanvas.ViewportSize.Width - RichCanvas.GetRichCanvasSettings().ItemsExtent.Right).ToInt());
            }
        }

        // Left direction not excluded here as I can offset the starting point, because I am scrolling only once
        // scrolling multiple times would require offseting multiple times, meaning mouse up and down which will stop and restart dragging each time
        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [RealTimeDragging(false)]
        [Test]
        public void DragItemOutsideViewport_WithRealTimeDraggingDisable_ShouldShowScrollBarsWhenMouseIsReleased(Direction draggingDirection)
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddDrawnRectangle();
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            int offset = draggingDirection switch
            {
                Direction.Left => RichCanvas.ViewportLocation.X - (drawnContainer.Location.X + drawnContainer.ActualWidth.ToInt()),
                Direction.Right => (RichCanvas.ViewportSize.Width - drawnContainer.Location.X).ToInt(),
                Direction.Up => RichCanvas.ViewportLocation.Y - (drawnContainer.Location.Y + drawnContainer.ActualHeight.ToInt()),
                Direction.Down => (RichCanvas.ViewportSize.Height - drawnContainer.Location.Y).ToInt(),
                _ => throw new NotImplementedException()
            };

            // act
            switch (draggingDirection)
            {
                case Direction.Left:
                    {
                        var startPoint = drawnContainer.GetLocationPoint(StartPosition.TopRight).RelativeToRichCanvas(RichCanvas);
                        var endPoint = startPoint.AddOffset(offset, DragDirection.OnlyX);
                        drawnContainer.Drag(startPoint, endPoint);
                        break;
                    }

                case Direction.Up:
                    {
                        var startPoint = drawnContainer.GetLocationPoint(StartPosition.BottomLeft).RelativeToRichCanvas(RichCanvas);
                        var endPoint = startPoint.AddOffset(offset, DragDirection.OnlyY);
                        drawnContainer.Drag(startPoint, endPoint);
                        break;
                    }

                case Direction.Down:
                    {
                        var startPoint = drawnContainer.GetLocationPoint().RelativeToRichCanvas(RichCanvas);
                        var endPoint = startPoint.AddOffset(offset, DragDirection.OnlyY);
                        drawnContainer.Drag(startPoint, endPoint);
                        break;
                    }

                case Direction.Right:
                    {
                        var startPoint = drawnContainer.GetLocationPoint().RelativeToRichCanvas(RichCanvas);
                        var endPoint = startPoint.AddOffset(offset, DragDirection.OnlyX);
                        drawnContainer.Drag(startPoint, endPoint);
                        break;
                    }
            }

            // assert
            ScrollbarShouldBeVisible(draggingDirection);
        }

        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [RealTimeDragging(false)]
        [Test]
        public void DragItemOutsideViewport_WithRealTimeDraggingDisable_ShouldNotShowScrollBarsWhileMouseIsMoving(Direction draggingDirection)
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddDrawnRectangle();
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            int offset = draggingDirection switch
            {
                Direction.Left => RichCanvas.ViewportLocation.X - (drawnContainer.Location.X + drawnContainer.ActualWidth.ToInt()),
                Direction.Right => (RichCanvas.ViewportSize.Width - drawnContainer.Location.X).ToInt(),
                Direction.Up => RichCanvas.ViewportLocation.Y - (drawnContainer.Location.Y + drawnContainer.ActualHeight.ToInt()),
                Direction.Down => (RichCanvas.ViewportSize.Height - drawnContainer.Location.Y).ToInt(),
                _ => throw new NotImplementedException()
            };

            // act & assert
            switch (draggingDirection)
            {
                case Direction.Left:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetLocationPoint(StartPosition.TopRight).RelativeToRichCanvas(RichCanvas));
                        drawnContainer.Move(CurrentMousePosition.AddOffset(offset, DragDirection.OnlyX));
                        HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
                        horizontalScrollBar.Should().BeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Up:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetLocationPoint(StartPosition.BottomLeft).RelativeToRichCanvas(RichCanvas));
                        drawnContainer.Move(CurrentMousePosition.AddOffset(offset, DragDirection.OnlyY));
                        VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
                        verticalScrollBar.Should().BeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Down:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetLocationPoint().RelativeToRichCanvas(RichCanvas));
                        drawnContainer.Move(CurrentMousePosition.AddOffset(offset, DragDirection.OnlyY));
                        VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
                        verticalScrollBar.Should().BeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Right:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetLocationPoint().RelativeToRichCanvas(RichCanvas));
                        drawnContainer.Move(CurrentMousePosition.AddOffset(offset, DragDirection.OnlyX));
                        HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
                        horizontalScrollBar.Should().BeNull();
                        drawnContainer.EndDragging();
                        break;
                    }
            }
        }

        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [RealTimeDragging(true)]
        [Test]
        public void DragItemOutsideViewport_WithRealTimeDraggingEnabled_ShouldShowScrollBarsWhileMouseIsMovig(Direction draggingDirection)
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddDrawnRectangle();
            RichCanvasContainerAutomation drawnContainer = RichCanvas.Items[0];
            int offset = draggingDirection switch
            {
                Direction.Left => RichCanvas.ViewportLocation.X - (drawnContainer.Location.X + drawnContainer.ActualWidth.ToInt()),
                Direction.Right => (RichCanvas.ViewportSize.Width - drawnContainer.Location.X).ToInt(),
                Direction.Up => RichCanvas.ViewportLocation.Y - (drawnContainer.Location.Y + drawnContainer.ActualHeight.ToInt()),
                Direction.Down => (RichCanvas.ViewportSize.Height - drawnContainer.Location.Y).ToInt(),
                _ => throw new NotImplementedException()
            };

            // act & assert
            switch (draggingDirection)
            {
                case Direction.Left:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetLocationPoint(StartPosition.TopRight).RelativeToRichCanvas(RichCanvas));
                        drawnContainer.Move(CurrentMousePosition.AddOffset(offset, DragDirection.OnlyX));
                        HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
                        horizontalScrollBar.Should().NotBeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Up:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetLocationPoint(StartPosition.BottomLeft).RelativeToRichCanvas(RichCanvas));
                        drawnContainer.Move(CurrentMousePosition.AddOffset(offset, DragDirection.OnlyY));
                        VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
                        verticalScrollBar.Should().NotBeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Down:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetLocationPoint().RelativeToRichCanvas(RichCanvas));
                        drawnContainer.Move(CurrentMousePosition.AddOffset(offset, DragDirection.OnlyY));
                        VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
                        verticalScrollBar.Should().NotBeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Right:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetLocationPoint().RelativeToRichCanvas(RichCanvas));
                        drawnContainer.Move(CurrentMousePosition.AddOffset(offset, DragDirection.OnlyX));
                        HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
                        horizontalScrollBar.Should().NotBeNull();
                        drawnContainer.EndDragging();
                        break;
                    }
            }
        }

        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [ShouldExecuteDrawingEndedCommand(false)]
        [Test]
        public void DrawContainerOutsideViewport_ShouldShowScrollBarsAfterDraw(Direction direction)
        {
            // arrange
            // resize so we can draw to left and right
            if (direction == Direction.Left || direction == Direction.Right)
            {
                Window.ResizeByTitleBarDragging();
                RichCanvas.ViewportLocation = new Point(0, 0);
            }

            RichCanvasUIAClientCommunicator.AddPositionedRectangle();

            // act
            RichCanvasContainerAutomation addedContainer = RichCanvas.Items[0];
            Size containerSize = direction switch
            {
                Direction.Left => new Size(-RichCanvas.BoundingRectangle.Left - addedContainer.Location.X, addedContainer.Location.Y + 50),
                Direction.Right => new Size(RichCanvas.BoundingRectangle.Right + 50, addedContainer.Location.Y),
                Direction.Up => new Size(addedContainer.Location.X + 50, -RichCanvas.BoundingRectangle.Top - addedContainer.Location.Y),
                Direction.Down => new Size(addedContainer.Location.X + 50, RichCanvas.BoundingRectangle.Bottom + 50),
                _ => throw new NotImplementedException(),
            };
            var drawingStartPoint = addedContainer.Location.RelativeToRichCanvas(RichCanvas);
            var drawingEndPoint = drawingStartPoint.GetEndPointByScale(containerSize);
            RichCanvas.Draw(drawingStartPoint, drawingEndPoint);

            // assert
            ScrollbarShouldBeVisible(direction);
            Window.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
        }

        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [ShouldExecuteDrawingEndedCommand(false)]
        [Test]
        public void AddDrawnContainerOutsideViewport_ShouldShowScrollBars(Direction direction)
        {
            // act
            switch (direction)
            {
                case Direction.Down:
                    RichCanvasUIAClientCommunicator.AddItemTopOutsideViewport();
                    break;

                case Direction.Up:
                    RichCanvasUIAClientCommunicator.AddItemBottomOutsideViewport();
                    break;

                case Direction.Left:
                    RichCanvasUIAClientCommunicator.AddItemLeftOutsideViewport();
                    break;

                case Direction.Right:
                    RichCanvasUIAClientCommunicator.AddItemRightOutsideViewport();
                    break;
            }

            // assert
            ScrollbarShouldBeVisible(direction);
        }

        [Test]
        public void ResizeViewportWhenItemsDrawn_ToHideOrIntersectWithThem_ShouldShowScrollBars()
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddSelectableItems();
            double lowestItemTop = RichCanvas.Items.Max(i => i.Location.Y);
            double lowestItemRight = RichCanvas.Items.Max(i => i.Location.X + i.ActualWidth);

            // act
            Window.ResizeByTitleBarDragging();
            Window.TryResize(lowestItemTop * RichCanvas.ViewportZoom, lowestItemRight * RichCanvas.ViewportZoom);

            // assert
            VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
            verticalScrollBar.Should().NotBeNull();
            HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
            horizontalScrollBar.Should().NotBeNull();

            Window.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
        }

        [TestCase(Direction.Left)]
        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Right)]
        [Test]
        public void PanningRichCanvas_WhenItemLeavesViewport_ShouldShowScrollBars(Direction direction)
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddDrawnRectangle();
            RichCanvas.Focus();

            RichCanvasContainerAutomation itemContainer = RichCanvas.Items[0];
            Point panningStartPoint = direction switch
            {
                Direction.Right => CreatePointRelativeToRichCanvasElement(itemContainer.BoundingRectangle.Left, itemContainer.BoundingRectangle.Top),
                Direction.Left => CreatePointRelativeToRichCanvasElement(itemContainer.BoundingRectangle.Right, itemContainer.BoundingRectangle.Top),
                Direction.Up => CreatePointRelativeToRichCanvasElement(itemContainer.BoundingRectangle.Left, itemContainer.BoundingRectangle.Top),
                Direction.Down => CreatePointRelativeToRichCanvasElement(itemContainer.BoundingRectangle.Left, itemContainer.BoundingRectangle.Bottom),
                _ => throw new NotImplementedException()
            };
            Point outsideViewportPoint = direction switch
            {
                Direction.Right => CreatePointRelativeToRichCanvasElement(RichCanvas.ViewportSize.Width.ToInt() + 50, itemContainer.BoundingRectangle.Top),
                Direction.Left => CreatePointRelativeToRichCanvasElement(itemContainer.BoundingRectangle.Left - 50, itemContainer.BoundingRectangle.Top),
                Direction.Up => CreatePointRelativeToRichCanvasElement(itemContainer.BoundingRectangle.Left, -50),
                Direction.Down => CreatePointRelativeToRichCanvasElement(itemContainer.BoundingRectangle.Left, RichCanvas.ViewportSize.Height.ToInt() + 50),
                _ => throw new NotImplementedException()
            };

            // act
            RichCanvas.Pan(panningStartPoint, outsideViewportPoint);

            // assert
            ScrollbarShouldBeVisible(direction);
        }

        private void ScrollbarShouldBeVisible(Direction direction)
        {
            if (direction == Direction.Up || direction == Direction.Down)
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

        private void ScrollbarShouldNotBeVisible(Direction direction)
        {
            if (direction == Direction.Up || direction == Direction.Down)
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
    }
}
