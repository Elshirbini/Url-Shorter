using Serilog.Events;
using Serilog.Formatting;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace UrlShorter.Common.Formatters;


public class PrettyJsonFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        var obj = new
        {
            Timestamp = logEvent.Timestamp,
            Level = logEvent.Level.ToString(),
            Message = logEvent.RenderMessage(),
            Exception = logEvent.Exception?.ToString(),
            Properties = logEvent.Properties.ToDictionary(
                x => x.Key,
                x => x.Value.ToString())
        };

        output.WriteLine(
            JsonSerializer.Serialize(
                obj,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
    }
}