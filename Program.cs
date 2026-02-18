using LoginAndAuth.Data;
using LoginAndAuth.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<LoginContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserService, UserServices>();

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
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddGoogle(opt =>
    {
        var googleAuth = builder.Configuration.GetSection("Authentication:Google");
        opt.ClientId = googleAuth["ClientId"];
        opt.ClientSecret = googleAuth["ClientSecret"];
    });

builder.Services.AddCors(opt => opt.AddPolicy("MyCorsPolicy", policy =>
{
    policy.WithOrigins("https://localhost:52565").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
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

// For popup if needed
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
    context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
    await next();
});

app.MapControllers();

app.Run();
