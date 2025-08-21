using FlaUI.Core.AutomationElements;

namespace RichCanvas.UITests
{
    internal static class AutomationElementExtensions
    {
        internal static RichCanvasAutomation AsRichCanvasAutomation(this AutomationElement self, Window parentWindow)
            => self == null ? null : new RichCanvasAutomation(self.FrameworkAutomationElement)
            {
                ParentWindow = parentWindow
            };

        internal static RichItemContainerAutomation AsRichCanvasContainerAutomation(this AutomationElement self)
           => self == null ? null : new RichItemContainerAutomation(self.FrameworkAutomationElement);
    }
}
