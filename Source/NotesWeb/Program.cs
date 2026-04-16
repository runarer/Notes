using FastEndpoints.Swagger;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;
using Microsoft.AspNetCore.Identity;
using NotesWeb.Entities;
using OpenTelemetry.Logs;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService("NoteWeb")); // Identify your service

    options.IncludeFormattedMessage = true; // Includes the readable message
    options.IncludeScopes = true;          // Captures properties from ILogger.BeginScope

    options.AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
});

builder.Services.AddDbContext<NoteBoardDBContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

//TODO: Flytt til en exension class, eller bruk source generator for å finne og registrere repositories */
builder.Services.AddScoped<
    NotesWeb.Features.Users.SignUp.Persistence.ISignUpRepository,
    NotesWeb.Features.Users.SignUp.Persistence.SignUpRepository>();

builder.Services.AddScoped<
    NotesWeb.Features.Users.Login.Persistence.IUserLoginRepository,
    NotesWeb.Features.Users.Login.Persistence.UserLoginRepository>();


builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = "secret");
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Users", x => x.RequireRole("User").RequireClaim("UserId")); // This might not be needed
builder.Services.AddFastEndpoints();

builder.Services.SwaggerDocument();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All; // Customize based on PII/Performance needs
});

var app = builder.Build();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultExceptionHandler();
app.UseFastEndpoints(c =>
{
    c.Errors.UseProblemDetails();
    c.Endpoints.RoutePrefix = "api";
});

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();
