using FlaUI.Core.AutomationElements;

namespace RichCanvas.UIAutomation.Tests
{
    internal static class AutomationElementExtensions
    {
        internal static RichCanvasAutomation AsRichCanvasAutomation(this AutomationElement self,
            Window parentWindow,
            IPC.RichCanvasUITestsPipeServer uITestsAppChannel)
            => self == null ? null : new RichCanvasAutomation(self.FrameworkAutomationElement)
            {
                ParentWindow = parentWindow,
                UITestsAppChannel = uITestsAppChannel
            };

        internal static RichCanvasContainerAutomation AsRichCanvasContainerAutomation(this AutomationElement self)
           => self == null ? null : new RichCanvasContainerAutomation(self.FrameworkAutomationElement);
    }
}
