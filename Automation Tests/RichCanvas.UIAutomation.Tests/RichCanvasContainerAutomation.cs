using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;

using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;

using Newtonsoft.Json;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.Core.ControlInformations;
using RichCanvas.UIAutomation.Tests.Helpers;

namespace RichCanvas.UIAutomation.Tests
{
    public class RichCanvasContainerAutomation(FrameworkAutomationElementBase frameworkAutomationElement) : AutomationElement(frameworkAutomationElement)
    {
        private bool _dragging;

        public RichCanvasContainerData RichCanvasContainerData => JsonConvert.DeserializeObject<RichCanvasContainerData>(Patterns.Value.Pattern.Value.Value, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });

        public Point Location => new Point(RichCanvasContainerData.Left.ToInt(), RichCanvasContainerData.Top.ToInt());

        public bool IsDraggable
        {
            get => RichCanvasContainerData.IsDraggable;
            internal set => SetValue(value);
        }

        public bool IsDrawn => ActualHeight != 0 && ActualWidth != 0 && !double.IsNaN(ActualHeight) && !double.IsNaN(ActualWidth);

        public void StartDragging(StartPosition startPosition = StartPosition.TopLeft)
        {
            if (_dragging)
            {
                throw new InvalidOperationException("Dragging operation already in progress.");
            }

            _dragging = true;
            Point startLocation = startPosition switch
            {
                StartPosition.TopLeft => Point.Add(Location, new Size(1, 1)),
                StartPosition.TopRight => Point.Add(Location, new Size(ActualWidth.ToInt(), 1)),
                StartPosition.BottomLeft => Point.Add(Location, new Size(1, ActualHeight.ToInt())),
                StartPosition.BottomRight => Point.Add(Location, new Size(ActualWidth.ToInt(), ActualHeight.ToInt())),
                _ => throw new NotImplementedException(),
            };
            Mouse.Position = startLocation.ToCanvasDrawingPoint();

            FlaUIInputData flaUIDragInput = InputMapper.MapToFlaUIInput(RichCanvasGestures.Drag);
            flaUIDragInput.Start();
        }

        public void Move(int offset, DragDirection dragDirection = DragDirection.Both)
        {
            if (!_dragging)
            {
                throw new InvalidOperationException("Dragging operation not started. Drag input should be processed before moving.");
            }
            Point dragTo = dragDirection switch
            {
                DragDirection.Both => new Point(Mouse.Position.X + offset, Mouse.Position.Y + offset),
                DragDirection.OnlyX => new Point(Mouse.Position.X + offset, Mouse.Position.Y),
                DragDirection.OnlyY => new Point(Mouse.Position.X, Mouse.Position.Y + offset),
                _ => throw new NotImplementedException()
            };
            Mouse.Position = dragTo.ToCanvasDrawingPoint();
            Wait.UntilInputIsProcessed();
        }

        public void EndDragging()
        {
            if (!_dragging)
            {
                throw new InvalidOperationException("Dragging operation not in progress. Nothing to end.");
            }
            FlaUIInputData flaUIDragInput = InputMapper.MapToFlaUIInput(RichCanvasGestures.Drag);
            flaUIDragInput.Stop();
            _dragging = false;
        }

        internal void Drag(int offset, StartPosition startPosition = StartPosition.TopLeft, DragDirection dragDirection = DragDirection.Both)
        {
            StartDragging(startPosition);
            Move(offset, dragDirection);
            EndDragging();
        }

        private void SetValue(object value, [CallerMemberName] string propertyName = default)
        {
            var containerInfoClone = (RichCanvasContainerData)RichCanvasContainerData.Clone();
            PropertyInfo property = containerInfoClone.GetType().GetProperty(propertyName);
            property.SetValue(containerInfoClone, value);
            Patterns.Value.Pattern.SetValue(JsonConvert.SerializeObject(containerInfoClone));
        }
    }

    public enum StartPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public enum DragDirection
    {
        Both,
        OnlyX,
        OnlyY
    }
}