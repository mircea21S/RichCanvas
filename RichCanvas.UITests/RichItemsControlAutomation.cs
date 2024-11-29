using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Patterns;
using RichCanvas.Automation;
using RichCanvas.Automation.ControlInformations;
using RichCanvas.Gestures;
using RichCanvas.UITests.Helpers;
using RichCanvas.UITests.Tests;
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

        public void DragContainer(RichItemContainerAutomation richItemContainer, Point endDraggingPoint, Point startPointOffset = default)
        {
            var containerLocation = richItemContainer.BoundingRectangle.Location;
            containerLocation.Offset(startPointOffset);
            Input.WithGesture(RichCanvasGestures.Drag).Drag(containerLocation, endDraggingPoint);
        }

        public void DragContainerOutsideViewportWithOffset(RichItemContainerAutomation richItemContainer, Direction direction, int offsetDistance)
        {
            var containerLocation = richItemContainer.BoundingRectangle.Location;
            var currentItemBounds = richItemContainer.BoundingRectangle;
            var offset = new Point(0, 0);
            Point draggingEndPoint;
            if (direction == Direction.Left)
            {
                offset = new Point(1, 0);
                draggingEndPoint = new Point(ViewportLocation.X - offsetDistance, containerLocation.Y);
            }
            else if (direction == Direction.Right)
            {
                draggingEndPoint = new Point(ViewportSize.Width - currentItemBounds.Width + offsetDistance, containerLocation.Y);
            }
            else if (direction == Direction.Up)
            {
                draggingEndPoint = new Point(containerLocation.X, ViewportLocation.Y - offsetDistance);
            }
            else
            {
                draggingEndPoint = new Point(containerLocation.X, ViewportSize.Height - currentItemBounds.Height + offsetDistance);
            }

            containerLocation.Offset(offset);
            Input.WithGesture(RichCanvasGestures.Drag).Drag(containerLocation, draggingEndPoint.ToCanvasDrawingPoint());
        }

        public void DefferedDragContainerOutsideViewportWithOffset(RichItemContainerAutomation richItemContainer, Direction direction, int offsetDistance, Action<Point> assertStepAction)
        {
            var containerLocation = richItemContainer.BoundingRectangle.Location;
            var currentItemBounds = richItemContainer.BoundingRectangle;
            var offset = new Point(0, 0);
            Point draggingEndPoint;
            var stepPoints = new List<Point>(3);

            //TODO: implement a way to assert each step point diff and changes
            if (direction == Direction.Left)
            {
                offset = new Point(1, 0);
                draggingEndPoint = new Point(ViewportLocation.X - offsetDistance, containerLocation.Y);
                stepPoints.Add(draggingEndPoint);
                stepPoints.Add(stepPoints[0].OffsetNew(new Point(-offsetDistance, 0)));
                stepPoints.Add(stepPoints[1].OffsetNew(new Point(-offsetDistance, 0)));
            }
            else if (direction == Direction.Right)
            {
                draggingEndPoint = new Point(ViewportSize.Width - currentItemBounds.Width + offsetDistance, containerLocation.Y);
                stepPoints.Add(draggingEndPoint);
                stepPoints.Add(draggingEndPoint.OffsetNew(new Point(1, 0)));
                stepPoints.Add(draggingEndPoint.OffsetNew(new Point(2, 0)));
            }
            else if (direction == Direction.Up)
            {
                draggingEndPoint = new Point(containerLocation.X, ViewportLocation.Y - offsetDistance);
                stepPoints.Add(draggingEndPoint);
                stepPoints.Add(draggingEndPoint.OffsetNew(new Point(0, -1)));
                stepPoints.Add(draggingEndPoint.OffsetNew(new Point(0, -2)));
            }
            else
            {
                draggingEndPoint = new Point(containerLocation.X, ViewportSize.Height - currentItemBounds.Height + offsetDistance);
                stepPoints.Add(draggingEndPoint);
                stepPoints.Add(draggingEndPoint.OffsetNew(new Point(0, 1)));
                stepPoints.Add(draggingEndPoint.OffsetNew(new Point(0, 2)));
            }
            draggingEndPoint = draggingEndPoint.ToCanvasDrawingPoint();
            containerLocation.Offset(offset);
            Input.WithGesture(RichCanvasGestures.Drag).DefferedDrag(containerLocation, [.. stepPoints], assertStepAction);
        }
    }
}
