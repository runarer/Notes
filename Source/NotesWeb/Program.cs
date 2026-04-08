using FastEndpoints.Swagger;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;
using Microsoft.AspNetCore.Identity;
using NotesWeb.Entities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<NoteBoardDBContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddScoped<
    NotesWeb.Features.Users.SignUp.Persistence.ISignUpRepository,
    NotesWeb.Features.Users.SignUp.Persistence.SignUpRepository>();

builder.Services.AddScoped<
    NotesWeb.Features.Users.Login.Persistence.IUserLoginRepository,
    NotesWeb.Features.Users.Login.Persistence.UserLoginRepository>();


builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = "secret")
                .AddAuthorization()
                .AddFastEndpoints();
builder.Services.SwaggerDocument();

var app = builder.Build();
app.UseAuthentication()
   .UseAuthorization()
   .UseFastEndpoints();

app.UseSwaggerGen();

app.Run();
