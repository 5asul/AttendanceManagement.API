using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AttendanceManagement.API.Services
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {

        private const string DateFormat = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (DateOnly.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }

            throw new JsonException($"Invalid date format. Expected format: {DateFormat}");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat));
        }

    }
}
