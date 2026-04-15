
using System.Text.Json;

namespace NotesWeb.Features.Users.Login;

public class UserLoginPreProcessor(ILogger<UserLoginPreProcessor> logger) : IPreProcessor<Request>
{
    private readonly ILogger _logger = logger;

    private static readonly JsonSerializerOptions writeOptions = new()
    {
        WriteIndented = false
    };
    public Task PreProcessAsync(IPreProcessorContext<Request> ctx, CancellationToken ct)
    {

        if (_logger.IsEnabled(LogLevel.Information))
        {

            var endpointName = ctx.HttpContext.GetEndpoint()?.DisplayName ?? "Unknown";
            var request = ctx.Request;

            var requestJson = request is not null
                ? JsonSerializer.Serialize(new Request { Email = request.Email, Password = "Redacted" }, writeOptions)
                : "No request body";

            _logger.LogInformation(
                "Request: {Endpoint} with : {Request}",
                endpointName,
                requestJson);
        }

        return Task.CompletedTask;
    }
}

public class UserLoginPostProcessor(ILogger<UserLoginPostProcessor> logger) : IPostProcessor<Request, Response>
{
    private readonly ILogger _logger = logger;
    private static readonly JsonSerializerOptions writeOptions = new()
    {
        WriteIndented = false
    };
    public Task PostProcessAsync(IPostProcessorContext<Request, Response> ctx, CancellationToken ct)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            var endpointName = ctx.HttpContext.GetEndpoint()?.DisplayName ?? "Unknown";
            var response = ctx.Response;

            var responseJson = response is not null
                ? JsonSerializer.Serialize(new Response { Token = "Redacted" }, writeOptions)
                : "No response body";

            _logger.LogInformation(
                "Response: {Endpoint} with : {Request}",
                endpointName,
                responseJson);
        }

        return Task.CompletedTask;
    }
}
