using Ankets.Data;
using Ankets.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders; // Bu satýrý ekleyin

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
              .AllowCredentials() // ÖNEMLÝ: Credentials için gerekli
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
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
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
    });

// Custom Services
builder.Services.AddScoped<AuthService>();
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
// Bu özel OPTIONS handler'ý artýk gerekli deðil, çünkü AddCors() ve UseCors()
// metodlarý bunu otomatik olarak hallediyor. Silebilirsiniz.

app.UseStaticFiles();

// ÖNEMLÝ: app.UseRouting() ve app.UseCors() sýralamasýný düzeltin.
app.UseRouting();

// CORS'u yönlendirmeden (routing) sonra ve kimlik doðrulamadan (authentication) önce çaðýrýn.
// Bu, hem preflight OPTIONS isteklerinin hem de gerçek API çaðrýlarýnýn doðru þekilde
// CORS politikasý tarafýndan ele alýnmasýný saðlar.
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
