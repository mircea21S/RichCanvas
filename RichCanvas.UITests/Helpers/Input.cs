using FlaUI.Core.WindowsAPI;
using RichCanvas.Gestures;
using System.Windows.Input;
using Mouse = FlaUI.Core.Input.Mouse;
using Keyboard = FlaUI.Core.Input.Keyboard;
using MouseButton = FlaUI.Core.Input.MouseButton;
using System.Drawing;
using System;

namespace RichCanvas.UITests.Helpers
{
    internal static class Input
    {
        internal static GestureHandler WithGesture(InputGesture gesture)
        {
            if (gesture is MouseKeyGesture mouseKeyGesture)
            {
                return new GestureHandler(mouseKeyGesture);
            }
            if (gesture is MouseGesture mouseGesture)
            {
                return new GestureHandler(mouseGesture);
            }
            if (gesture is KeyGesture keyGesture)
            {
                return new GestureHandler(keyGesture);
            }
            return null;
        }

        internal static VirtualKeyShort ToVirtualKeyShort(this Key key) => key switch
        {
            Key.Q => VirtualKeyShort.KEY_Q,
            _ => throw new MissingMemberException("Key not mapped")
        };
        internal static MouseButton ToMouseButton(this MouseAction mouseAction) => mouseAction switch
        {
            MouseAction.LeftClick => MouseButton.Left,
            _ => throw new MissingMemberException("Key not mapped")
        };
        internal static VirtualKeyShort ToVirtualKeyShort(this ModifierKeys modifierKey) => modifierKey switch
        {
            ModifierKeys.Control => VirtualKeyShort.CONTROL,
            ModifierKeys.Shift => VirtualKeyShort.SHIFT,
            ModifierKeys.Windows => VirtualKeyShort.LWIN,
            ModifierKeys.Alt => VirtualKeyShort.ALT,
            _ => throw new MissingMemberException("Key not mapped")
        };
    }

    internal class GestureHandler
    {
        private readonly MouseKeyGesture _mouseKeyGesture;
        private readonly MouseGesture _mouseGesture;
        private readonly KeyGesture _keyGesture;

        internal GestureHandler(MouseKeyGesture inputGesture)
        {
            _mouseKeyGesture = inputGesture;
        }
        internal GestureHandler(MouseGesture inputGesture)
        {
            _mouseGesture = inputGesture;
        }
        internal GestureHandler(KeyGesture inputGesture)
        {
            _keyGesture = inputGesture;
        }

        internal void Drag(Point startPoint, Point endPoint)
        {
            if (_mouseKeyGesture != null)
            {
                MouseKeyGestureDrag(startPoint, endPoint);
            }
            else if (_mouseGesture != null)
            {
                MouseGestureDrag(startPoint, endPoint);
            }
            else if (_keyGesture != null)
            {
                KeyGestureDrag(startPoint, endPoint);
            }
        }

        private void MouseKeyGestureDrag(Point startPoint, Point endPoint)
        {
            var key = _mouseKeyGesture.Keys[0].ToVirtualKeyShort();
            var mouseGesture = _mouseKeyGesture.MouseGesture;

            Keyboard.Press(key);
            Mouse.Drag(startPoint, endPoint, mouseGesture.MouseAction.ToMouseButton());
            Keyboard.Release(key);
        }

        private void MouseGestureDrag(Point startPoint, Point endPoint)
        {
            VirtualKeyShort key = VirtualKeyShort.KEY_X;
            if (_mouseGesture.Modifiers != ModifierKeys.None)
            {
                key = _mouseGesture.Modifiers.ToVirtualKeyShort();
            }

            if (key != VirtualKeyShort.KEY_X)
            {
                Keyboard.Press(key);
            }
            Mouse.Drag(startPoint, endPoint, _mouseGesture.MouseAction.ToMouseButton());
            if (key != VirtualKeyShort.KEY_X)
            {
                Keyboard.Release(key);
            }
        }

        private void KeyGestureDrag(Point startPoint, Point endPoint)
        {
            throw new NotImplementedException();
        }

        internal void Click(Point point)
        {
            if (_mouseKeyGesture != null)
            {
                MouseKeyGestureClick(point);
            }
            else if (_mouseGesture != null)
            {
                MouseGestureClick(point);
            }
            else if (_keyGesture != null)
            {
                KeyGestureClick(point);
            }
        }

        private void KeyGestureClick(Point point)
        {
            throw new NotImplementedException();
        }

        private void MouseGestureClick(Point point)
        {
            VirtualKeyShort key = VirtualKeyShort.KEY_X;
            if (_mouseGesture.Modifiers != ModifierKeys.None)
            {
                key = _mouseGesture.Modifiers.ToVirtualKeyShort();
            }

            if (key != VirtualKeyShort.KEY_X)
            {
                Keyboard.Press(key);
            }
            Mouse.Click(point, _mouseGesture.MouseAction.ToMouseButton());
            if (key != VirtualKeyShort.KEY_X)
            {
                Keyboard.Release(key);
            }
        }

        private void MouseKeyGestureClick(Point point)
        {
            // TODO: get key from KeyGestures and check both arrays
            var key = _mouseKeyGesture.Keys[0].ToVirtualKeyShort();
            var mouseGesture = _mouseKeyGesture.MouseGesture;

            Keyboard.Press(key);
            Mouse.Click(point, mouseGesture.MouseAction.ToMouseButton());
            Keyboard.Release(key);
        }
    }
}
