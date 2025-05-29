using FlaUI.Core.AutomationElements;
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

        public void SetViewportZoom(double zoomValue)
        {
            var viewportZoomTextBox = ParentWindow.FindFirstDescendant(d => d.ByAutomationId(AutomationIds.ViewportZoomTextBoxId)).AsTextBox();
            viewportZoomTextBox.Patterns.Value.Pattern.SetValue(zoomValue.ToString());
        }
    }
}
