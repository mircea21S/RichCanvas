using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using RichCanvas.Gestures;
using System;
using System.Drawing;

namespace RichCanvas.UITests.Helpers
{
    internal class MouseKeyGestureHandler : GestureHandler
    {
        private readonly MouseKeyGesture _mouseKeyGesture;

        public MouseKeyGestureHandler(MouseKeyGesture gesture)
        {
            _mouseKeyGesture = gesture;
        }

        internal override void Click(Point point)
        {
            // TODO: get key from KeyGestures and check both arrays
            var key = _mouseKeyGesture.Keys[0].ToVirtualKeyShort();
            var mouseGesture = _mouseKeyGesture.MouseGesture;

            Keyboard.Press(key);
            Mouse.Click(point, mouseGesture.MouseAction.ToMouseButton());
            Keyboard.Release(key);
        }

        internal override void DefferedDrag(Point startPoint, params (Point Position, Action StepAction)[] dragStepPoints) => throw new NotImplementedException();

        internal override void Drag(Point startPoint, Point endPoint)
        {
            VirtualKeyShort key = _mouseKeyGesture.Keys?[0].ToVirtualKeyShort() ?? _mouseKeyGesture.KeyGestures?[0].Key.ToVirtualKeyShort() ?? default;
            var mouseGesture = _mouseKeyGesture.MouseGesture;

            Keyboard.Press(key);
            Mouse.Drag(startPoint, endPoint, mouseGesture.MouseAction.ToMouseButton());
            Keyboard.Release(key);
        }
    }
}