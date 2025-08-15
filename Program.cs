using Ankets.Data;
using Ankets.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS configuration to allow requests from different origins.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// Adds controllers and API endpoints.
builder.Services.AddControllers();

// Database connection: Using Npgsql for PostgreSQL.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adds and configures the JWT Authentication Service.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validates the token's issuer.
        ValidateIssuer = true,
        // Validates the token's audience.
        ValidateAudience = true,
        // Validates the token's lifetime.
        ValidateLifetime = true,
        // Validates the token's signing key.
        ValidateIssuerSigningKey = true,

        // Values from the appsettings.json file.
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        // Converts the JWT key to a byte array in UTF-8 format.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Adds service dependencies. AuthService can now be used throughout the application.
builder.Services.AddScoped<AuthService>();

// Adds Swagger/OpenAPI services for API documentation.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configures the HTTP request pipeline.

// Enables Swagger UI in all environments.
app.UseSwagger();
app.UseSwaggerUI();

// Enables static files middleware.
app.UseStaticFiles();

// Enables CORS middleware.
app.UseCors();

// Adds authentication middleware. This must come before `app.UseAuthorization()`.
app.UseAuthentication();
// Adds authorization middleware.
app.UseAuthorization();

// Maps controller routes.
app.MapControllers();

// Runs the application.
app.Run();
