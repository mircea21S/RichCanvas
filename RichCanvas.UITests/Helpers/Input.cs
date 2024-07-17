using FlaUI.Core.WindowsAPI;
using RichCanvas.Gestures;
using System.Windows.Input;
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
                return new MouseKeyGestureHandler(mouseKeyGesture);
            }
            if (gesture is MouseGesture mouseGesture)
            {
                return new MouseGestureHandler(mouseGesture);
            }
            if (gesture is KeyGesture keyGesture)
            {
                return new KeyGestureHandler(keyGesture);
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
            ModifierKeys.None => VirtualKeyShort.NONAME,
            _ => throw new MissingMemberException("Key not mapped")
        };
    }

    internal abstract class GestureHandler
    {
        internal abstract void Drag(Point startPoint, Point endPoint);
        internal abstract void Click(Point point);
        internal abstract void DefferedDrag(Point startPoint, params (Point Position, Action StepAction)[] dragStepPoints);
    }
}
