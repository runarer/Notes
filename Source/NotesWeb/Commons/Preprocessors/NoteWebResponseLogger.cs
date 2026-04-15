
using System.Text.Json;

namespace NotesWeb.Commons.Preprocessors;

public class NoteWebResponseLogger<TRequest, TResponse>(ILogger<NoteWebResponseLogger<TRequest, TResponse>> logger) : IPostProcessor<TRequest, TResponse>
{
    private readonly ILogger _logger = logger;
    private static readonly JsonSerializerOptions writeOptions = new()
    {
        WriteIndented = false
    };
    public Task PostProcessAsync(IPostProcessorContext<TRequest, TResponse> ctx, CancellationToken ct)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            var endpointName = ctx.HttpContext.GetEndpoint()?.DisplayName ?? "Unknown";
            var response = ctx.Response;

            var responseJson = response is not null
                ? JsonSerializer.Serialize(response, writeOptions)
                : "No response body";

            _logger.LogInformation(
                "Response: {Endpoint} with : {Response}",
                endpointName,
                responseJson);
        }

        return Task.CompletedTask;
    }
}