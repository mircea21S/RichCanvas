using FlaUI.Core.AutomationElements;

namespace RichCanvas.UIAutomation.FlaUIClient
{
    public static class AutomationElementExtensions
    {
        public static RichCanvasAutomation? AsRichCanvasAutomation(this AutomationElement self, Window parentWindow) =>
            self == null ? null : new RichCanvasAutomation(self.FrameworkAutomationElement)
            {
                ParentWindow = parentWindow
            };

        public static RichCanvasContainerAutomation? AsRichCanvasContainerAutomation(this AutomationElement self)
           => self == null ? null : new RichCanvasContainerAutomation(self.FrameworkAutomationElement);
    }
}
