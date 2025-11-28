using System;
using System.Linq;
using System.Windows;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements.Scrolling;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;

using FluentAssertions;

using NUnit.Framework;

using RichCanvas.UIAutomation.Tests.Extensions;
using RichCanvas.UIAutomation.Tests.Tests.Dragging;
using RichCanvas.UIAutomation.Tests.Tests.Drawing;

using RichCanvasUIA.Client.TestMocks;

namespace RichCanvas.UIAutomation.Tests.Tests.Scrolling
{
    // TODO: Investingate horizontal mouse wheel scrolling.
    public class ScrollingTests : RichCanvasTestAppTest
    {
        public override void TearDown()
        {
            base.TearDown();
            RichCanvas.ViewportLocation = new System.Drawing.Point(0, 0);
        }

        [TestCase(Direction.Down)]
        [TestCase(Direction.Up)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [Test]
        public void ScrollingVerticallyAndHorizontally_ShouldTranslateTheCanvas(Direction scrollingDirection)
        {
            // arrange
            System.Drawing.Point initialViewportLocation = RichCanvas.ViewportLocation;

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
                Direction.Up => Math.Abs(RichCanvas.ViewportLocation.Y) <= RichCanvas.ViewportSize.Height - DrawingStateDataMocks.DrawnRectangleMock.BoundingBox.Bottom,
                Direction.Left => RichCanvas.ViewportLocation.X <= drawnContainer.Location.X,
                Direction.Right => Math.Abs(RichCanvas.ViewportLocation.X) <= RichCanvas.ViewportSize.Width - DrawingStateDataMocks.DrawnRectangleMock.BoundingBox.Right,
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
                RichCanvas.ViewportLocation.Y.Should().Be(RichCanvas.RichCanvasSettings.ItemsExtent.Top.ToInt());
            }
            else if (direction == Direction.Up)
            {
                RichCanvas.ViewportLocation.Y.Should().Be(-(RichCanvas.ViewportSize.Height - RichCanvas.RichCanvasSettings.ItemsExtent.Bottom).ToInt());
            }
            else if (direction == Direction.Left)
            {
                RichCanvas.ViewportLocation.X.Should().Be(RichCanvas.RichCanvasSettings.ItemsExtent.Left.ToInt());
            }
            else
            {
                RichCanvas.ViewportLocation.X.Should().Be(-(RichCanvas.ViewportSize.Width - RichCanvas.RichCanvasSettings.ItemsExtent.Right).ToInt());
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
                    drawnContainer.Drag(drawnContainer.GetContainerLocation(StartPosition.TopRight), CurrentPoint.AddOffset(offset, DragDirection.OnlyX));
                    break;

                case Direction.Up:
                    drawnContainer.Drag(drawnContainer.GetContainerLocation(StartPosition.BottomLeft), CurrentPoint.AddOffset(offset, DragDirection.OnlyY));
                    break;

                case Direction.Down:
                    drawnContainer.Drag(drawnContainer.GetContainerLocation(), CurrentPoint.AddOffset(offset, DragDirection.OnlyY));
                    break;

                case Direction.Right:
                    drawnContainer.Drag(drawnContainer.GetContainerLocation(), CurrentPoint.AddOffset(offset, DragDirection.OnlyX));
                    break;
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
                        drawnContainer.StartDragging(drawnContainer.GetContainerLocation(StartPosition.TopRight));
                        drawnContainer.Move(CurrentPoint.AddOffset(offset, DragDirection.OnlyX));
                        HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
                        horizontalScrollBar.Should().BeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Up:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetContainerLocation(StartPosition.BottomLeft));
                        drawnContainer.Move(CurrentPoint.AddOffset(offset, DragDirection.OnlyY));
                        VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
                        verticalScrollBar.Should().BeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Down:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetContainerLocation());
                        drawnContainer.Move(CurrentPoint.AddOffset(offset, DragDirection.OnlyY));
                        VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
                        verticalScrollBar.Should().BeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Right:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetContainerLocation());
                        drawnContainer.Move(CurrentPoint.AddOffset(offset, DragDirection.OnlyX));
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
                        drawnContainer.StartDragging(drawnContainer.GetContainerLocation(StartPosition.TopRight));
                        drawnContainer.Move(CurrentPoint.AddOffset(offset, DragDirection.OnlyX));
                        HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
                        horizontalScrollBar.Should().NotBeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Up:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetContainerLocation(StartPosition.BottomLeft));
                        drawnContainer.Move(CurrentPoint.AddOffset(offset, DragDirection.OnlyY));
                        VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
                        verticalScrollBar.Should().NotBeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Down:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetContainerLocation());
                        drawnContainer.Move(CurrentPoint.AddOffset(offset, DragDirection.OnlyY));
                        VerticalScrollBar verticalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
                        verticalScrollBar.Should().NotBeNull();
                        drawnContainer.EndDragging();
                        break;
                    }

                case Direction.Right:
                    {
                        drawnContainer.StartDragging(drawnContainer.GetContainerLocation());
                        drawnContainer.Move(CurrentPoint.AddOffset(offset, DragDirection.OnlyX));
                        HorizontalScrollBar horizontalScrollBar = Window.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
                        horizontalScrollBar.Should().NotBeNull();
                        drawnContainer.EndDragging();
                        break;
                    }
            }
        }

        // left excluded due to maximized window
        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Right)]
        [ShouldExecuteDrawingEndedCommand(false)]
        [Test]
        public void DrawContainerOutsideViewport_ShouldShowScrollBarsAfterDraw(Direction direction)
        {
            // arrange
            RichCanvasUIAClientCommunicator.AddPositionedRectangle();

            // act
            RichCanvasContainerAutomation addedContainer = RichCanvas.Items[0];
            System.Drawing.Size containerSize = direction switch
            {
                Direction.Right => new System.Drawing.Size(RichCanvas.ViewportSize.Width.ToInt() + 50, addedContainer.Location.Y + 50),
                Direction.Up => new System.Drawing.Size(addedContainer.Location.X + 50, -RichCanvasDemoTitleBarHeight - addedContainer.Location.Y),
                Direction.Down => new System.Drawing.Size(addedContainer.Location.X + 50, RichCanvas.ViewportSize.Height.ToInt() + 50),
                _ => throw new NotImplementedException(),
            };
            RichCanvas.DrawPositionedContainer(addedContainer, containerSize);

            // assert
            ScrollbarShouldBeVisible(direction);
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
            // resize by dragging the title bar
            Mouse.Click(new System.Drawing.Point((int)VisualViewportSize.Width / 2, -5));
            Mouse.Drag(new System.Drawing.Point((int)VisualViewportSize.Width / 2, -5), new System.Drawing.Point((int)VisualViewportSize.Width / 2, 0));
            Wait.UntilInputIsProcessed();

            if (Window.Patterns.Transform.TryGetPattern(out FlaUI.Core.Patterns.ITransformPattern transformPattern))
            {
                transformPattern.Resize(lowestItemTop * RichCanvas.ViewportZoom, lowestItemRight * RichCanvas.ViewportZoom);
            }

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
                Direction.Right => new Point(itemContainer.BoundingRectangle.Right, itemContainer.BoundingRectangle.Top),
                Direction.Left => new Point(itemContainer.BoundingRectangle.Right, itemContainer.BoundingRectangle.Top),
                Direction.Up => new Point(itemContainer.BoundingRectangle.Left, itemContainer.BoundingRectangle.Top),
                Direction.Down => new Point(itemContainer.BoundingRectangle.Left, itemContainer.BoundingRectangle.Bottom),
                _ => throw new NotImplementedException()
            };
            Point outsideViewportPoint = direction switch
            {
                Direction.Right => new Point(RichCanvas.ViewportSize.Width.ToInt() + 50, itemContainer.BoundingRectangle.Top),
                Direction.Left => new Point(itemContainer.BoundingRectangle.Left - 50, itemContainer.BoundingRectangle.Top),
                Direction.Up => new Point(itemContainer.BoundingRectangle.Left, -50),
                Direction.Down => new Point(itemContainer.BoundingRectangle.Left, RichCanvas.ViewportSize.Height.ToInt() + 50),
                _ => throw new NotImplementedException()
            };

            // act
            RichCanvas.Pan(panningStartPoint.AsDrawingPoint(), outsideViewportPoint.AsDrawingPoint());

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

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
