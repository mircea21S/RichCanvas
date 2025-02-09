using FlaUI.Core.Input;
using RichCanvas.Gestures;
using RichCanvas.UITests.Helpers;
using RichCanvasUITests.App.Automation;

namespace RichCanvas.UITests
{
    public partial class RichItemsControlAutomation
    {
        public double ViewportZoom => RichItemsControlData.ViewportZoom;
        public double ScaleFactor => RichItemsControlData.ScaleFactor;

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

        public void ResetZoom() => ParentWindow.InvokeButton(AutomationIds.ResetViewportZoomButtonId);
    }
}
