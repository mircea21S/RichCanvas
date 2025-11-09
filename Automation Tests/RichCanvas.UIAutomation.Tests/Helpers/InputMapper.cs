using System;
using System.Drawing;

using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;

namespace RichCanvas.UIAutomation.Tests.Helpers
{
    internal static class InputMapper
    {
        internal static FlaUIInputData MapToFlaUIInput(System.Windows.Input.InputGesture input)
        {
            if (input is System.Windows.Input.MouseGesture mouseGesture)
            {
                return new FlaUIInputData(mouseGesture.Modifiers.ToVirtualKeyShort(), mouseGesture.MouseAction.ToMouseButton());
            }
            throw new NotSupportedException($"Input gesture {input.GetType().Name} not supported.");
        }
    }

    internal class FlaUIInputData(VirtualKeyShort key, MouseButton mouseButton)
    {
        public VirtualKeyShort Key { get; } = key;
        public MouseButton MouseButton { get; } = mouseButton;

        public void Start()
        {
            Keyboard.Press(Key);
            Mouse.Down(MouseButton);
            Wait.UntilInputIsProcessed();
        }

        public void Stop()
        {
            Mouse.Up(MouseButton);
            Keyboard.Release(Key);
            Wait.UntilInputIsProcessed();
        }

        public void Drag(Point startPoint, Point endPoint)
        {
            Keyboard.Press(Key);
            Mouse.Drag(startPoint, endPoint, MouseButton);
            Keyboard.Release(Key);
        }

        internal void Click(Point pointOnCanvas)
        {
            Keyboard.Press(Key);
            Mouse.Click(pointOnCanvas, MouseButton);
            Keyboard.Release(Key);
        }
    }
}
