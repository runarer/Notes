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



// Logging with OpenTelementry
const string serviceName = "NoteWeb";
builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(serviceName));

    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.AddProcessor(new RedactionProcessor());

    // Sending log to Seq
    options.AddOtlpExporter(otlpOptions =>
    {
        var location = builder.Configuration["Seq:Location"] ?? "http://localhost:5341/ingest/otlp/v1/logs";
        otlpOptions.Endpoint = new Uri(location);
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
    });

    if (builder.Environment.IsDevelopment())
        options.AddConsoleExporter();
});

// The database
builder.Services.AddDbContext<NoteBoardDBContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Need timeprovider, also makes testing easier as it can be swapped with a fake one.
builder.Services.AddSingleton(TimeProvider.System);



// Password hasher, Here 'User' can be any class.
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// JWT
var jwtkey = builder.Configuration["Auth:JwtSecretKey"];
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = jwtkey);
builder.Services.AddAuthentication(o => o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();


builder.Services.AddFastEndpoints();


//Document the endpoint
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

// Migrate the database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NoteBoardDBContext>();
    db.Database.Migrate();
}


app.Run();
