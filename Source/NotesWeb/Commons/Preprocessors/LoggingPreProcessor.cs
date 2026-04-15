
using System.Text.Json;

namespace NotesWeb.Commons.Preprocessors;

public class LoggingPreProcessor<TRequest>(ILogger<LoggingPreProcessor<TRequest>> logger) : IGlobalPreProcessor
{
    private readonly ILogger<LoggingPreProcessor<TRequest>> _logger = logger;

    private static readonly JsonSerializerOptions writeOptions = new()
    {
        WriteIndented = false
    };

    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        var endpointName = context.HttpContext.GetEndpoint()?.DisplayName ?? "Unknown";
        var request = context.Request;

        var requestJson = request is not null
            ? JsonSerializer.Serialize(request, writeOptions)
            : "No request body";

        _logger.LogInformation(
            "Request: {Endpoint} with : {Request}",
            endpointName,
            requestJson);

        return Task.CompletedTask;
    }
}

