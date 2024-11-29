using FlaUI.Core.Input;
using System;
using System.Drawing;

namespace RichCanvas.UITests.Helpers
{
    internal class MouseGestureHandler : GestureHandler
    {
        private readonly System.Windows.Input.MouseGesture _mouseGesture;

        public MouseGestureHandler(System.Windows.Input.MouseGesture gesture)
        {
            _mouseGesture = gesture;
        }

        internal override void Click(Point point)
        {
            var pressedKey = _mouseGesture.Modifiers.ToVirtualKeyShort();

            Keyboard.Press(pressedKey);
            Mouse.Click(point, _mouseGesture.MouseAction.ToMouseButton());
            Keyboard.Release(pressedKey);
        }

        internal override void DefferedDrag(Point startPoint, params (Point Position, Action StepAction)[] dragStepPoints)
        {
            var mouseButton = _mouseGesture.MouseAction.ToMouseButton();
            var pressedKey = _mouseGesture.Modifiers.ToVirtualKeyShort();

            Keyboard.Press(pressedKey);

            Mouse.Position = startPoint;
            Mouse.Down(mouseButton);

            foreach ((Point Position, Action StepAction) pointToActionPair in dragStepPoints)
            {
                Mouse.Position = pointToActionPair.Position;
                pointToActionPair.StepAction?.Invoke();
            }

            Mouse.Up(mouseButton);

            Keyboard.Release(pressedKey);
        }

        internal override void DefferedDrag(Point startPoint, Point[] pointSteps, Action<Point> assertStepAction)
        {
            var mouseButton = _mouseGesture.MouseAction.ToMouseButton();
            var pressedKey = _mouseGesture.Modifiers.ToVirtualKeyShort();

            Keyboard.Press(pressedKey);

            Mouse.Position = startPoint;
            Mouse.Down(mouseButton);

            foreach (Point stepPoint in pointSteps)
            {
                Mouse.Position = stepPoint;
                assertStepAction?.Invoke(stepPoint);
            }

            Mouse.Up(mouseButton);

            Keyboard.Release(pressedKey);
        }

        internal override void Drag(Point startPoint, Point endPoint)
        {
            var pressedKey = _mouseGesture.Modifiers.ToVirtualKeyShort();

            Keyboard.Press(pressedKey);
            Mouse.Drag(startPoint, endPoint, _mouseGesture.MouseAction.ToMouseButton());
            Keyboard.Release(pressedKey);
        }
    }
}