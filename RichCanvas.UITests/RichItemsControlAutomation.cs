using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Patterns;
using RichCanvas.Automation;
using RichCanvas.Automation.ControlInformations;
using RichCanvas.Gestures;
using RichCanvas.UITests.Helpers;
using RichCanvas.UITests.Tests;
using RichCanvasUITests.App.Automation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RichCanvas.UITests
{
    public class RichItemsControlAutomation : AutomationElement
    {
        public Point ViewportLocation => RichItemsControlData.ViewportLocation.AsDrawingPoint();
        public Size ViewportSize => new Size((int)RichItemsControlData.ViewportSize.Width, (int)RichItemsControlData.ViewportSize.Height);
        public RichItemContainerAutomation[] Items
        {
            get
            {
                if (Patterns.ItemContainer.TryGetPattern(out var itemContainerPattern))
                {
                    var allItems = new List<RichItemContainerAutomation>();
                    AutomationElement item = null;
                    do
                    {
                        item = itemContainerPattern.FindItemByProperty(item, null, null);
                        if (item != null)
                        {
                            allItems.Add(item.AsRichItemContainerAutomation());
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
                if (Patterns.Selection.TryGetPattern(out var selectionPattern))
                {
                    var allItems = new List<RichItemContainerAutomation>();
                    foreach (var selection in selectionPattern.Selection.ValueOrDefault)
                    {
                        if (selection != null)
                        {
                            allItems.Add(selection.AsRichItemContainerAutomation());
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
                if (Patterns.Selection.TryGetPattern(out var selectionPattern) && !selectionPattern.CanSelectMultiple)
                {
                    return selectionPattern.Selection.ValueOrDefault.SingleOrDefault().AsRichItemContainerAutomation();
                }
                return null;
            }
        }

        public RichItemsControlData RichItemsControlData => Patterns.Value.Pattern.Value.Value.AsRichItemsControlData();

        public IScrollPattern ScrollInfo => Patterns.Scroll.PatternOrDefault;

        public Window ParentWindow { get; internal set; }

        public RichItemsControlAutomation(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }

        public void ScrollByArrowKeyOrButton(Direction scrollingMode)
        {
            if (Patterns.Scroll.TryGetPattern(out var scrollPattern))
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
            if (Patterns.Scroll.TryGetPattern(out var scrollPattern))
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
            if (Patterns.Scroll.TryGetPattern(out var scrollPattern))
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
            if (Patterns.Scroll.TryGetPattern(out var scrollPattern))
            {
                scrollPattern.SetScrollPercent(horizontalOffset, verticalOffset);
            }
        }

        public void DragContainerOutsideViewportWithOffset(RichItemContainerAutomation richItemContainer, Direction direction, int offsetDistance)
        {
            var containerLocation = richItemContainer.BoundingRectangle.Location;
            var currentItemBounds = richItemContainer.BoundingRectangle;
            var offset = direction == Direction.Left ? new Point(offsetDistance, 0) : new Point(0, 0);
            Point draggingEndPoint = direction switch
            {
                Direction.Left => new Point(ViewportLocation.X - offsetDistance, containerLocation.Y),
                Direction.Right => new Point(ViewportSize.Width - currentItemBounds.Width + offsetDistance, containerLocation.Y),
                Direction.Up => new Point(containerLocation.X, ViewportLocation.Y - offsetDistance),
                Direction.Down => new Point(containerLocation.X, ViewportSize.Height - currentItemBounds.Height + offsetDistance),
                _ => throw new NotImplementedException(),
            };

            containerLocation.Offset(offset);
            Input.WithGesture(RichCanvasGestures.Drag).Drag(containerLocation, draggingEndPoint.ToCanvasDrawingPoint());
        }

        public void DefferedDragContainerOutsideViewportWithOffset(RichItemContainerAutomation richItemContainer, Direction direction, int offsetBetweenPoints, Action<Point, int> assertStepAction)
        {
            var currentItemBounds = richItemContainer.BoundingRectangle;
            var containerLocation = currentItemBounds.Location;

            Point firstDraggingPoint = direction switch
            {
                Direction.Left => new Point(ViewportLocation.X - offsetBetweenPoints, containerLocation.Y),
                Direction.Right => new Point(ViewportSize.Width - currentItemBounds.Width + offsetBetweenPoints, containerLocation.Y),
                Direction.Up => new Point(containerLocation.X, ViewportLocation.Y - offsetBetweenPoints),
                Direction.Down => new Point(containerLocation.X, ViewportSize.Height - currentItemBounds.Height + offsetBetweenPoints),
                _ => throw new NotImplementedException(),
            };

            var data = new GeneratorData(3, direction, firstDraggingPoint.ToCanvasDrawingPoint(), offsetBetweenPoints);
            Input.WithGesture(RichCanvasGestures.Drag).DefferedDrag(containerLocation, data, assertStepAction);
        }

        public void DragCurrentSelectionOutsideViewport(RichItemContainerAutomation fromContainer, Direction direction)
            => DragContainerOutsideViewportWithOffset(fromContainer, direction, 0);

        public void DefferedDragCurrentSelectionOutsideViewport(RichItemContainerAutomation fromContainer, Direction direction, Action<Point, int> assertStepAction, int stepOffset = 0)
            => DefferedDragContainerOutsideViewportWithOffset(fromContainer, direction, stepOffset, assertStepAction);

        internal void DrawEmptyContainer(Direction direction, int offset, Action assertCallbackAction)
        {
            ParentWindow.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);
            var viewportCenter = new Point(ViewportSize.Width / 2, ViewportSize.Height / 2);
            Point draggingEndPoint = direction switch
            {
                Direction.Left => new Point(ViewportLocation.X - offset, viewportCenter.Y),
                Direction.Right => new Point(ViewportSize.Width + offset, viewportCenter.Y),
                Direction.Up => new Point(viewportCenter.X, ViewportLocation.Y - offset),
                Direction.Down => new Point(viewportCenter.X, ViewportSize.Height + offset),
                _ => throw new NotImplementedException(),
            };
            Input.WithGesture(RichCanvasGestures.Drawing).DefferedDrag(viewportCenter, (draggingEndPoint.ToCanvasDrawingPoint(), assertCallbackAction));
        }

        internal void Pan(Point fromPoint, Point toPoint)
        {
            Input.WithGesture(RichCanvasGestures.Pan).Drag(fromPoint, toPoint);
        }

        internal void ResetViewportLocation() => ParentWindow.InvokeButton(AutomationIds.ResetViewportLocationButtonId);
    }
}
