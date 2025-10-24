using System;
using System.Drawing;

using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;

using Newtonsoft.Json;

using RichCanvas.Automation.ControlInformations;
using RichCanvas.Gestures;
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

        public void StartDragging()
        {
            if (_dragging)
            {
                throw new InvalidOperationException("Dragging operation already in progress.");
            }

            _dragging = true;
            Mouse.Position = Point.Add(Location, new Size(1, 1)).ToCanvasDrawingPoint();

            FlaUIInputData flaUIDragInput = InputMapper.MapToFlaUIInput(RichCanvasGestures.Drag);
            flaUIDragInput.Start();
        }

        public void Move(int offset)
        {
            if (!_dragging)
            {
                throw new InvalidOperationException("Dragging operation not started. Drag input should be processed before moving.");
            }
            Mouse.Position = new Point(Mouse.Position.X + offset, Mouse.Position.Y + offset).ToCanvasDrawingPoint();
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

        internal void Drag(int offset)
        {
            StartDragging();
            Move(offset);
            EndDragging();
        }
    }
}