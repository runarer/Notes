
using System.Text.Json;

namespace NotesWeb.Commons.Preprocessors;

public class NoteWebRequestLogger<TRequest>(ILogger<NoteWebRequestLogger<TRequest>> logger) : IPreProcessor<TRequest>
{
    private readonly ILogger<NoteWebRequestLogger<TRequest>> _logger = logger;

    private static readonly JsonSerializerOptions readOptions = new()
    {
        WriteIndented = false
    };

    public Task PreProcessAsync(IPreProcessorContext<TRequest> ctx, CancellationToken ct)
    {

        if (_logger.IsEnabled(LogLevel.Information))
        {
            var endpointName = ctx.HttpContext.GetEndpoint()?.DisplayName ?? "Unknown";
            var request = ctx.Request;

            var requestJson = request is not null
                ? JsonSerializer.Serialize(request, readOptions)
                : "No request body";

            _logger.LogInformation(
                "Request: {Endpoint} with : {Request}",
                endpointName,
                requestJson);
        }

        return Task.CompletedTask;
    }
}
