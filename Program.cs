using LoginAndAuth.Data;
using LoginAndAuth.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];
var googleAuth = builder.Configuration.GetSection("Authentication:Google");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<LoginContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddScoped<TokenServices>();

builder.Services.AddHttpClient();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(opt =>
    {
        opt.Cookie.Name = "ShoppingAppSession";
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SameSite = SameSiteMode.Lax;
    })
    .AddGoogle(opt =>
    {
        opt.ClientId = googleAuth["ClientId"];
        opt.ClientSecret = googleAuth["ClientSecret"];
    });

builder.Services.AddCors(opt => opt.AddPolicy("MyCorsPolicy", policy =>
{
    policy.WithOrigins("http://localhost:52565").AllowAnyMethod().AllowAnyHeader();
    //policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("MyCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
    await next();
});

app.MapControllers();

app.Run();
