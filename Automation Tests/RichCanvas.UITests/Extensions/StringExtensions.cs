using Newtonsoft.Json;

using RichCanvas.Automation.ControlInformations;

namespace RichCanvas.UITests
{
    internal static class StringExtensions
    {
        internal static RichItemsControlData AsRichItemsControlData(this string value) => JsonConvert.DeserializeObject<RichItemsControlData>(value, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
    }
}
