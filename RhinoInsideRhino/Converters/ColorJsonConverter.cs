using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhinoInsideRhino.Converters
{
    public class ColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var argb = reader.GetInt32();
            return Color.FromArgb(argb);
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.ToArgb());
        }
    }
}