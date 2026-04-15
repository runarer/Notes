
using System.Text.Json;

namespace NotesWeb.Features.Users.SignUp;

// public class SignUpPreProcessor(ILogger<SignUpPreProcessor> logger) : IPreProcessor<Request>
// {
//     private readonly ILogger _logger = logger;
//     public Task PreProcessAsync(IPreProcessorContext<Request> ctx, CancellationToken ct)
//     {
//         if (_logger.IsEnabled(LogLevel.Information))
//         {
//             var endpointName = ctx.HttpContext.GetEndpoint()?.DisplayName ?? "Unknown";
//             var request = ctx.Request;

//             string requestJson = request is not null
//                 ? $"{{\"email\":\"{request.Email}\", \"password\": \"removed\"}}"
//                 : "No request body";

//             _logger.LogInformation(
//                 "Request: {Endpoint} with : {Request}",
//                 endpointName,
//                 requestJson);
//         }

//         return Task.CompletedTask;
//     }

// }

public class SignUpPreProcessor(ILogger<SignUpPreProcessor> logger) : IPreProcessor<Request>
{
    private readonly ILogger _logger = logger;

    private static readonly JsonSerializerOptions readOptions = new()
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
                ? JsonSerializer.Serialize(new Request { Username = request.Username, FullName = request.FullName, Email = request.Email, Password = "Redacted" }, readOptions)
                : "No request body";

            _logger.LogInformation(
                "Request: {Endpoint} with : {Request}",
                endpointName,
                requestJson);
        }

        return Task.CompletedTask;
    }
}