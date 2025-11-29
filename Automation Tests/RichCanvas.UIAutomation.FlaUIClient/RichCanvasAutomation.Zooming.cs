using FlaUI.Core.Input;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.FlaUIClient.Input;

namespace RichCanvas.UIAutomation.FlaUIClient
{
    public partial class RichCanvasAutomation
    {
        public double ViewportZoom
        {
            get => GetRichCanvasSettings().ViewportZoom;
            set => SetValue(value);
        }

        public double ScaleFactor => GetRichCanvasSettings().ScaleFactor;

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
