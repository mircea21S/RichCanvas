using Newtonsoft.Json;

using RichCanvas.Automation.ControlInformations;

namespace RichCanvas.UITests
{
    internal static class StringExtensions
    {
        internal static RichCanvasData AsRichCanvasData(this string value) => JsonConvert.DeserializeObject<RichCanvasData>(value, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
    }
}
