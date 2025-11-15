using System;
using System.Drawing;
using System.Linq;

using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;

using RichCanvas.Gestures;

namespace RichCanvas.UIAutomation.Tests.Helpers
{
    internal static class InputMapper
    {
        internal static FlaUIInputData MapToFlaUIInput(System.Windows.Input.InputGesture input)
        {
            if (input is System.Windows.Input.MouseGesture mouseGesture)
            {
                return new FlaUIInputData([mouseGesture.Modifiers.ToVirtualKeyShort()], mouseGesture.MouseAction.ToMouseButton());
            }
            if (input is MouseKeyGesture mouseKeyGesture)
            {
                return new FlaUIInputData([.. mouseKeyGesture.KeyGestures.Select(x => x.Key.ToVirtualKeyShort())], mouseKeyGesture.MouseGesture.MouseAction.ToMouseButton());
            }
            throw new NotSupportedException($"Input gesture {input.GetType().Name} not supported.");
        }
    }

    internal class FlaUIInputData(VirtualKeyShort[] keys, MouseButton mouseButton)
    {
        public VirtualKeyShort[] Keys { get; } = keys;
        public MouseButton MouseButton { get; } = mouseButton;

        public void Start()
        {
            PressKeys();
            Mouse.Down(MouseButton);
            Wait.UntilInputIsProcessed();
        }

        public void Stop()
        {
            Mouse.Up(MouseButton);
            ReleaseKeys();
            Wait.UntilInputIsProcessed();
        }

        public void Drag(Point startPoint, Point endPoint)
        {
            PressKeys();
            Mouse.Drag(startPoint, endPoint, MouseButton);
            ReleaseKeys();
        }

        private void PressKeys()
        {
            foreach (var key in Keys)
            {
                Keyboard.Press(key);
            }
        }

        private void ReleaseKeys()
        {
            foreach (var key in Keys)
            {
                Keyboard.Release(key);
            }
        }
    }
}
