using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Utilities
{
    public class DecimalJsonConverter : JsonConverter
    {
        public DecimalJsonConverter()
        {
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(float) || objectType == typeof(double));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is double)
                writer.WriteRawValue(((double)value).ToString("F4", CultureInfo.InvariantCulture.NumberFormat));
            else
                writer.WriteRawValue(JsonConvert.ToString(value));        
        }
    }
}
