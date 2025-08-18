using Ankets.Data;
using Ankets.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders; // Bu sat�r� ekleyin
using System.Security.Claims; // Bu satr ekleyin

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials() // �NEML�: Credentials i�in gerekli
              .SetPreflightMaxAge(TimeSpan.FromHours(1));
    });
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = builder.Configuration["Swagger:Title"] ?? "Ankets API",
        Version = builder.Configuration["Swagger:Version"] ?? "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,  // ApiKey yerine Http
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Sadece JWT token'ınızı yapıştırın (Bearer yazmaya gerek yok)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtConfig = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfig["Key"] ?? throw new InvalidOperationException("JWT Key is missing"))),
            ClockSkew = TimeSpan.Zero
        };
        
        // DEBUG: Authentication events eklendi
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Token;
                
                // Eğer Authorization header var ama Bearer ile başlamıyorsa, token'ı direkt al
                if (string.IsNullOrEmpty(token))
                {
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    if (!string.IsNullOrEmpty(authHeader))
                    {
                        // Bearer prefix varsa kaldır, yoksa direkt kullan
                        token = authHeader.StartsWith("Bearer ") ? authHeader.Substring(7) : authHeader;
                        context.Token = token;
                    }
                }
                
                if (!string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"Token alındı: {token.Substring(0, Math.Min(20, token.Length))}...");
                }
                
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"Token validated for user ID: {userId}");
                return Task.CompletedTask;
            }
        };
    });

// Custom Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Global Error Handling Middleware
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            StatusCode = 500,
            Message = "Internal Server Error",
            Detailed = ex.Message,
            StackTrace = app.Environment.IsDevelopment() ? ex.StackTrace : null
        });

        Console.WriteLine($"HATA: {DateTime.UtcNow}");
        Console.WriteLine($"Mesaj: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    }
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ankets API V1"));
}

// OPTIONS Request Handler
// Bu �zel OPTIONS handler'� art�k gerekli de�il, ��nk� AddCors() ve UseCors()
// metodlar� bunu otomatik olarak hallediyor. Silebilirsiniz.

app.UseStaticFiles();

// �NEML�: app.UseRouting() ve app.UseCors() s�ralamas�n� d�zeltin.
app.UseRouting();

// CORS'u y�nlendirmeden (routing) sonra ve kimlik do�rulamadan (authentication) �nce �a��r�n.
// Bu, hem preflight OPTIONS isteklerinin hem de ger�ek API �a�r�lar�n�n do�ru �ekilde
// CORS politikas� taraf�ndan ele al�nmas�n� sa�lar.
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    environment = app.Environment.EnvironmentName
}));

app.Run();
