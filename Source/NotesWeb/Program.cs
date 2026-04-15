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

var app = builder.Build();
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
