using Newtonsoft.Json;

using RichCanvas.Automation.ControlInformations;

namespace RichCanvas.Automation
{
    public static class RichItemsControlDataExtensions
    {
        public static string ToJson(this RichItemsControlData data)
            => JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        public static RichItemsControlData AsRichItemsControlData(this string data)
            => JsonConvert.DeserializeObject<RichItemsControlData>(data, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
    }
}
