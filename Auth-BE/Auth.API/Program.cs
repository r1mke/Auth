﻿using Auth.API.Extensions;
using Auth.API.Middleware;
using Auth.Services.Interfaces;
using Auth.Services.Services;
using Auth.Services.Settings;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// === Add services ===
builder.Services.AddPersistenceServices(builder.Configuration);

// 🔥 OVO TI FALI
builder.Services.AddIdentityServices(builder.Configuration);

// Registruješ JWT Settings (ok je ovo)
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddInfrastructureServices();

// AuthService ovisi o UserManageru ➔ ovo ide TEK nakon Identity-a
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.LoginPath = "/api/auth/login";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Name = "AuthProject.Cookies";
    options.SlidingExpiration = true;
});

var app = builder.Build();

// === Middlewares ===
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
