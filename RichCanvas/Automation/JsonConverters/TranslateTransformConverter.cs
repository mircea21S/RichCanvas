using Newtonsoft.Json;
using System;
using System.Windows.Media;

namespace RichCanvas.Automation.JsonConverters
{
    public class TranslateTransformConverter : JsonConverter<TranslateTransform>
    {
        public override TranslateTransform? ReadJson(JsonReader reader, Type objectType, TranslateTransform? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, TranslateTransform? value, JsonSerializer serializer)
        {
        }
    }
}