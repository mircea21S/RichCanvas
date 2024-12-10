using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using Newtonsoft.Json;
using RichCanvas.Automation.ControlInformations;

namespace RichCanvas.UITests
{
    public class RichItemContainerAutomation : AutomationElement
    {
        public RichItemContainerAutomation(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }
        public RichItemContainerData RichItemContainerData => JsonConvert.DeserializeObject<RichItemContainerData>(Patterns.Value.Pattern.Value.Value, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
    }
}