using Newtonsoft.Json;

using RichCanvas.UIAutomation.Core.ControlInformations;

namespace RichCanvas.UIAutomation.Tests
{
    internal static class StringExtensions
    {
        internal static RichCanvasData AsRichCanvasData(this string value) => JsonConvert.DeserializeObject<RichCanvasData>(value, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
    }
}
