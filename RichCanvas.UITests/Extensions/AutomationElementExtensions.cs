using FlaUI.Core.AutomationElements;

namespace RichCanvas.UITests
{
    internal static class AutomationElementExtensions
    {
        internal static RichItemsControlAutomation AsRichItemsControlAutomation(this AutomationElement self, Window parentWindow)
            => self == null ? null : new RichItemsControlAutomation(self.FrameworkAutomationElement)
            {
                ParentWindow = parentWindow
            };

        internal static RichItemContainerAutomation AsRichItemContainerAutomation(this AutomationElement self)
           => self == null ? null : new RichItemContainerAutomation(self.FrameworkAutomationElement);
    }
}
