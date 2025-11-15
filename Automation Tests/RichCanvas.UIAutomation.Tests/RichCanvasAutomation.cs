using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Patterns;

using Newtonsoft.Json;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.ControlInformations;
using RichCanvas.UIAutomation.Tests.Helpers;
using RichCanvas.UIAutomation.Tests.IPC;
using RichCanvas.UIAutomation.Tests.Tests.Scrolling;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation : AutomationElement
    {
        public Point ViewportLocation
        {
            get => RichCanvasData.ViewportLocation.AsDrawingPoint();
            set => SetValue(value.AsWindowsPoint());
        }

        public Size ViewportSizeInteger => new Size((int)RichCanvasData.ViewportSize.Width, (int)RichCanvasData.ViewportSize.Height);

        public RichCanvasContainerAutomation[] Items
        {
            get
            {
                if (Patterns.ItemContainer.TryGetPattern(out IItemContainerPattern itemContainerPattern))
                {
                    var allItems = new List<RichCanvasContainerAutomation>();
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

        public RichCanvasContainerAutomation[] SelectedItems
        {
            get
            {
                if (Patterns.Selection.TryGetPattern(out ISelectionPattern selectionPattern))
                {
                    var allItems = new List<RichCanvasContainerAutomation>();
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

        public RichCanvasContainerAutomation SelectedItem
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

        public Window ParentWindow { get; internal set; }

        public RichCanvasUITestsPipeServer UITestsAppChannel { get; internal set; }

        public bool RealTimeDraggingEnabled
        {
            get => RichCanvasData.RealTimeDraggingEnabled;
            set => SetValue(value);
        }

        public RichCanvasAutomation(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }

        public void DragContainerOutsideViewportWithOffset(RichCanvasContainerAutomation richItemContainer, Direction direction, int offsetDistance, System.Windows.Size visualViewportSize)
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

        public void DefferedDragContainerOutsideViewportWithOffset(RichCanvasContainerAutomation richItemContainer,
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

        public void DragCurrentSelectionOutsideViewport(RichCanvasContainerAutomation fromContainer, Direction direction, System.Windows.Size visualViewportSize)
            => DragContainerOutsideViewportWithOffset(fromContainer, direction, 0, visualViewportSize);

        public void DefferedDragCurrentSelectionOutsideViewport(RichCanvasContainerAutomation fromContainer,
            Direction direction,
            Action<Point, int> assertStepAction,
            System.Windows.Size visualViewportSize,
            int stepOffset = 0)
            => DefferedDragContainerOutsideViewportWithOffset(fromContainer, direction, stepOffset, assertStepAction, visualViewportSize);

        private void SetValue(object value, [CallerMemberName] string propertyName = default)
        {
            var containerInfoClone = (RichCanvasData)RichCanvasData.Clone();
            PropertyInfo property = containerInfoClone.GetType().GetProperty(propertyName);
            property.SetValue(containerInfoClone, value);
            Patterns.Value.Pattern.SetValue(JsonConvert.SerializeObject(containerInfoClone));
        }
    }
}
