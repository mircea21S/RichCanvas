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

        public Point Location => new(GetRichCanvasContainerSettings().Left.ToInt(), GetRichCanvasContainerSettings().Top.ToInt());

        public bool IsDraggable
        {
            get => GetRichCanvasContainerSettings().IsDraggable;
            set => SetValue(value);
        }

        public bool IsSelected
        {
            get => GetRichCanvasContainerSettings().IsSelected;
            set => SetValue(value);
        }

        public RichCanvasContainerData GetRichCanvasContainerSettings()
            => JsonConvert.DeserializeObject<RichCanvasContainerData>(Patterns.Value.Pattern.Value.Value,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

        public void StartDragging(Point startLocation)
        {
            if (_dragging)
            {
                throw new InvalidOperationException("Dragging operation already in progress.");
            }

            _dragging = true;
            Mouse.Position = startLocation;

            FlaUIInputData flaUIDragInput = InputMapper.MapToFlaUIInput(RichCanvasGestures.Drag);
            flaUIDragInput.Start();
        }

        public void Move(Point dragTo)
        {
            if (!_dragging)
            {
                throw new InvalidOperationException("Dragging operation not started. Drag input should be processed before moving.");
            }

            Mouse.Position = dragTo;
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

        public void Drag(Point from, Point to)
        {
            StartDragging(from);
            Move(to);
            EndDragging();
        }

        protected void SetValue(object value, [CallerMemberName] string propertyName = default)
        {
            var containerInfoClone = (RichCanvasContainerData)GetRichCanvasContainerSettings().Clone();
            PropertyInfo property = containerInfoClone.GetType().GetProperty(propertyName);
            property.SetValue(containerInfoClone, value);
            Patterns.Value.Pattern.SetValue(JsonConvert.SerializeObject(containerInfoClone));
        }
    }
}