using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddSingleton(TimeProvider.System)
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();
app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();
