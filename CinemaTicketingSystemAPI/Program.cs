using System.Text;
using CinemaTicketingSystemAPI.BaseDb;
using CinemaTicketingSystemAPI.ExceptionFilter;
using CinemaTicketingSystemAPI.LoggingMiddleware;
using CinemaTicketingSystemAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options => {
    options.Filters.Add<GlobalExceptionFilter>();
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMemoryCache(); 
builder.Services.AddControllers(options => 
{
    options.Filters.Add<GlobalExceptionFilter>(); 
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddControllers();
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "super_secret_key_1234567890123456");


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<RequestLoggingMiddleware>(); 



app.UseMiddleware<RequestLoggingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/genres", () => Results.Ok(new { Status = "System Online", Time = DateTime.Now }));

app.MapGet("/api/minimal/movies/{id}", async (int id, CinemaTicketingSystemAPI.BaseDb.AppDbContext db) =>
    await db.Movies.FindAsync(id) is CinemaTicketingSystemAPI.Models.Movie movie? Results.Ok(movie)
        : Results.NotFound());

app.Run();
