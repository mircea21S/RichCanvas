using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.Tests.Helpers;

using RichCanvasUIA.Client.Automation;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation
    {
        public double ViewportZoom => RichCanvasSettings.ViewportZoom;
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
            TextBox viewportZoomTextBox = ParentWindow.FindFirstDescendant(d => d.ByAutomationId(AutomationIds.ViewportZoomTextBoxId)).AsTextBox();
            viewportZoomTextBox.Patterns.Value.Pattern.SetValue(zoomValue.ToString());
        }

        internal void SelectAllItems()
        {
            ParentWindow.InvokeButton(AutomationIds.SelectAllItemsButtonId);
        }
    }
}
