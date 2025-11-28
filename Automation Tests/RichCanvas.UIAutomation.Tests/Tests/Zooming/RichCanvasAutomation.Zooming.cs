using FlaUI.Core.Input;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.Tests.Helpers;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation
    {
        public double ViewportZoom
        {
            get => RichCanvasSettings.ViewportZoom;
            set => SetValue(value);
        }

        public double ScaleFactor => RichCanvasSettings.ScaleFactor;

        public void ZoomIn()
        {
            var virtualKeyMap = RichCanvasGestures.ZoomModifierKey.ToVirtualKeyShort();
            Keyboard.Press(virtualKeyMap);
            Mouse.Scroll(1);
            Keyboard.Release(virtualKeyMap);
        }

        public void ZoomOut()
        {
            var virtualKeyMap = RichCanvasGestures.ZoomModifierKey.ToVirtualKeyShort();
            Keyboard.Press(virtualKeyMap);
            Mouse.Scroll(-1);
            Keyboard.Release(virtualKeyMap);
        }

        public void ResetZoom() => ViewportZoom = 1;

        public void Zoom(bool zoomIn)
        {
            if (zoomIn)
            {
                ZoomIn();
            }
            else
            {
                ZoomOut();
            }
        }
    }
}
