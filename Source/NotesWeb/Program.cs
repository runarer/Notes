using FastEndpoints.Swagger;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;
using NotesWeb.Commons;
using Microsoft.AspNetCore.Identity;
using NotesWeb.Entities;
using OpenTelemetry.Logs;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.HttpLogging;
using OpenTelemetry.Metrics;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

const string serviceName = "NoteWeb";

var jwtkey = builder.Configuration["Auth:JwtSecretKey"];

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(serviceName));

    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.AddProcessor(new RedactionProcessor());
    options.AddOtlpExporter(otlpOptions =>
    {
        var location = builder.Configuration["Seq:Location"] ?? "http://localhost:5341/ingest/otlp/v1/logs";
        otlpOptions.Endpoint = new Uri(location);
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
    if (builder.Environment.IsDevelopment())
        options.AddConsoleExporter();
});

builder.Services.AddDbContext<NoteBoardDBContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = jwtkey);
builder.Services.AddAuthentication(o => o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();
builder.Services.AddFastEndpoints();

builder.Services.SwaggerDocument(options =>
{
    options.DocumentSettings = s =>
    {
        s.Title = "ToDo Lists Api";
        s.Version = "v1";
    };
});

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
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

app.UseSwaggerGen();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NoteBoardDBContext>();
    db.Database.Migrate();
}
app.Run();
