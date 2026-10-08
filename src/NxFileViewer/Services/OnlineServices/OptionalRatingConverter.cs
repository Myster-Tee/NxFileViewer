using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Emignatik.NxFileViewer.Services.OnlineServices;

// Rating is optional metadata; malformed values must not discard a valid title.
public sealed class OptionalRatingConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        double value;
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetDouble(out value) && double.IsFinite(value)) return value;
        if (reader.TokenType == JsonTokenType.String && double.TryParse(reader.GetString()?.Trim().Replace(',', '.'),
            NumberStyles.Float, CultureInfo.InvariantCulture, out value) && double.IsFinite(value)) return value;
        reader.Skip();
        return 0;
    }
    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(double.IsFinite(value) ? value : 0);
}