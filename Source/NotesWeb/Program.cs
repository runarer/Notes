using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<NoteBoardDBContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<
    NotesWeb.Features.Users.SignUp.Persistence.ISignUpRepository,
    NotesWeb.Features.Users.SignUp.Persistence.SignUpRepository>();


builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

var app = builder.Build();
app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();
