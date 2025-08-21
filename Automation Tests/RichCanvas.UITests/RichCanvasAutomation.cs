using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Patterns;

using RichCanvas.Automation.ControlInformations;
using RichCanvas.Gestures;
using RichCanvas.UITests.Helpers;
using RichCanvas.UITests.Tests;

using RichCanvasUITests.App.Automation;

namespace RichCanvas.UITests
{
    public partial class RichCanvasAutomation : AutomationElement
    {
        public Point ViewportLocation => RichCanvasData.ViewportLocation.AsDrawingPoint();

        public Size ViewportSizeInteger => new Size((int)RichCanvasData.ViewportSize.Width, (int)RichCanvasData.ViewportSize.Height);

        public RichItemContainerAutomation[] Items
        {
            get
            {
                if (Patterns.ItemContainer.TryGetPattern(out IItemContainerPattern itemContainerPattern))
                {
                    var allItems = new List<RichItemContainerAutomation>();
                    AutomationElement item = null;
                    do
                    {
                        item = itemContainerPattern.FindItemByProperty(item, null, null);
                        if (item != null)
                        {
                            allItems.Add(item.AsRichCanvasContainerAutomation());
                        }
                    }
                    while (item != null);
                    return allItems.ToArray();
                }
                return null;
            }
        }

        public RichItemContainerAutomation[] SelectedItems
        {
            get
            {
                if (Patterns.Selection.TryGetPattern(out ISelectionPattern selectionPattern))
                {
                    var allItems = new List<RichItemContainerAutomation>();
                    foreach (AutomationElement selection in selectionPattern.Selection.ValueOrDefault)
                    {
                        if (selection != null)
                        {
                            allItems.Add(selection.AsRichCanvasContainerAutomation());
                        }
                    }
                    return allItems.ToArray();
                }
                return null;
            }
        }

        public RichItemContainerAutomation SelectedItem
        {
            get
            {
                if (Patterns.Selection.TryGetPattern(out ISelectionPattern selectionPattern) && !selectionPattern.CanSelectMultiple)
                {
                    return selectionPattern.Selection.ValueOrDefault.SingleOrDefault().AsRichCanvasContainerAutomation();
                }
                return null;
            }
        }

        public RichCanvasData RichCanvasData => Patterns.Value.Pattern.Value.Value.AsRichCanvasData();

        public IScrollPattern ScrollInfo => Patterns.Scroll.PatternOrDefault;

        public Window ParentWindow { get; internal set; }

        public RichCanvasAutomation(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }

        public void ScrollByArrowKeyOrButton(Direction scrollingMode)
        {
            if (Patterns.Scroll.TryGetPattern(out IScrollPattern scrollPattern))
            {
                if (scrollingMode == Direction.Up)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallDecrement);
                }
                else if (scrollingMode == Direction.Down)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallIncrement);
                }
                else if (scrollingMode == Direction.Left)
                {
                    scrollPattern.Scroll(ScrollAmount.SmallDecrement, ScrollAmount.NoAmount);
                }
                else if (scrollingMode == Direction.Right)
                {
                    scrollPattern.Scroll(ScrollAmount.SmallIncrement, ScrollAmount.NoAmount);
                }
            }
        }

        public void ScrollByPage(Direction scrollingMode)
        {
            if (Patterns.Scroll.TryGetPattern(out IScrollPattern scrollPattern))
            {
                if (scrollingMode == Direction.Up)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeDecrement);
                }
                else if (scrollingMode == Direction.Down)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
                }
                else if (scrollingMode == Direction.Left)
                {
                    scrollPattern.Scroll(ScrollAmount.LargeDecrement, ScrollAmount.NoAmount);
                }
                else if (scrollingMode == Direction.Right)
                {
                    scrollPattern.Scroll(ScrollAmount.LargeIncrement, ScrollAmount.NoAmount);
                }
            }
        }

        public void ScrollByScrollbarsDragging(Direction scrollingMode)
        {
            if (Patterns.Scroll.TryGetPattern(out IScrollPattern scrollPattern))
            {
                if (scrollingMode == Direction.Up)
                {
                    scrollPattern.SetScrollPercent(0, -1);
                }
                else if (scrollingMode == Direction.Down)
                {
                    scrollPattern.SetScrollPercent(0, 1);
                }
                else if (scrollingMode == Direction.Left)
                {
                    scrollPattern.SetScrollPercent(1, 0);
                }
                else
                {
                    scrollPattern.SetScrollPercent(-1, 0);
                }
            }
        }

        public void SetScrollPercent(double horizontalOffset, double verticalOffset)
        {
            if (Patterns.Scroll.TryGetPattern(out IScrollPattern scrollPattern))
            {
                scrollPattern.SetScrollPercent(horizontalOffset, verticalOffset);
            }
        }

        public void DragContainerOutsideViewportWithOffset(RichItemContainerAutomation richItemContainer, Direction direction, int offsetDistance, System.Windows.Size visualViewportSize)
        {
            Point containerLocation = richItemContainer.BoundingRectangle.Location;
            Rectangle currentItemBounds = richItemContainer.BoundingRectangle;
            Point offset = direction == Direction.Left ? new Point(offsetDistance, 0) : new Point(0, 0);
            Point draggingEndPoint = direction switch
            {
                Direction.Left => new Point(-offsetDistance, containerLocation.Y),
                Direction.Right => new Point((int)visualViewportSize.Width - currentItemBounds.Width + offsetDistance, containerLocation.Y),
                Direction.Up => new Point(containerLocation.X, -offsetDistance),
                Direction.Down => new Point(containerLocation.X, (int)visualViewportSize.Height - currentItemBounds.Height + offsetDistance),
                _ => throw new NotImplementedException(),
            };

            containerLocation.Offset(offset);
            Input.WithGesture(RichCanvasGestures.Drag).Drag(containerLocation, draggingEndPoint.ToCanvasDrawingPoint());
        }

        public void DefferedDragContainerOutsideViewportWithOffset(RichItemContainerAutomation richItemContainer,
            Direction direction,
            int offsetBetweenPoints,
            Action<Point, int> assertStepAction,
            System.Windows.Size visualViewportSize)
        {
            Rectangle currentItemBounds = richItemContainer.BoundingRectangle;
            Point containerLocation = currentItemBounds.Location;

            Point firstDraggingPoint = direction switch
            {
                Direction.Left => new Point(-offsetBetweenPoints, containerLocation.Y),
                Direction.Right => new Point((int)visualViewportSize.Width - currentItemBounds.Width + offsetBetweenPoints, containerLocation.Y),
                Direction.Up => new Point(containerLocation.X, -offsetBetweenPoints),
                Direction.Down => new Point(containerLocation.X, (int)visualViewportSize.Height - currentItemBounds.Height + offsetBetweenPoints),
                _ => throw new NotImplementedException(),
            };

            var data = new GeneratorData(3, direction, firstDraggingPoint.ToCanvasDrawingPoint(), offsetBetweenPoints);
            Input.WithGesture(RichCanvasGestures.Drag).DefferedDrag(containerLocation, data, assertStepAction);
        }

        public void DragCurrentSelectionOutsideViewport(RichItemContainerAutomation fromContainer, Direction direction, System.Windows.Size visualViewportSize)
            => DragContainerOutsideViewportWithOffset(fromContainer, direction, 0, visualViewportSize);

        public void DefferedDragCurrentSelectionOutsideViewport(RichItemContainerAutomation fromContainer,
            Direction direction,
            Action<Point, int> assertStepAction,
            System.Windows.Size visualViewportSize,
            int stepOffset = 0)
            => DefferedDragContainerOutsideViewportWithOffset(fromContainer, direction, stepOffset, assertStepAction, visualViewportSize);

        public void DrawEmptyContainer(System.Windows.Size visualViewportSize, Direction direction, int offset, Action assertCallbackAction)
        {
            ParentWindow.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);
            var viewportCenter = new Point((int)visualViewportSize.Width / 2, (int)visualViewportSize.Height / 2);
            Point draggingEndPoint = direction switch
            {
                Direction.Left => new Point(-offset, viewportCenter.Y),
                Direction.Right => new Point((int)visualViewportSize.Width + offset, viewportCenter.Y),
                Direction.Up => new Point(viewportCenter.X, -offset),
                Direction.Down => new Point(viewportCenter.X, (int)visualViewportSize.Height + offset),
                _ => throw new NotImplementedException(),
            };
            Input.WithGesture(RichCanvasGestures.Drawing).DefferedDrag(viewportCenter, (draggingEndPoint.ToCanvasDrawingPoint(), assertCallbackAction));
        }

        public void Pan(Point fromPoint, Point toPoint)
        {
            Input.WithGesture(RichCanvasGestures.Pan).Drag(fromPoint, toPoint);
        }

        public void ResetViewportLocation() => ParentWindow.InvokeButton(AutomationIds.ResetViewportLocationButtonId);

        public void PanItemOutsideViewport(RichItemContainerAutomation itemContainer, Direction direction, int outsideDistance, System.Windows.Size visualViewportSize)
        {
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
                Direction.Right => new Point((int)visualViewportSize.Width + outsideDistance, itemContainer.BoundingRectangle.Top),
                Direction.Left => new Point(itemContainer.BoundingRectangle.Left - outsideDistance, itemContainer.BoundingRectangle.Top),
                Direction.Up => new Point(itemContainer.BoundingRectangle.Left, -outsideDistance),
                Direction.Down => new Point(itemContainer.BoundingRectangle.Left, (int)visualViewportSize.Height + outsideDistance),
                _ => throw new NotImplementedException()
            };
            Pan(panningStartPoint, outsideViewportPoint.ToCanvasDrawingPoint());
        }
    }
}
