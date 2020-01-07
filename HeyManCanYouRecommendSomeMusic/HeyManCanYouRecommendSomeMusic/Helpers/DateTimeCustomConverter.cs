using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic.Helpers
{
    public class DateTimeConverterUsingDateTimeParseAsFallback : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));

            if (!reader.TryGetDateTime(out DateTime value))
            {
                if (!DateTime.TryParse(reader.GetString(), out value))
                {
                    value = new DateTime(1, 1, 1);
                }
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("dd/MM/yyyy"));
        }
    }
}
