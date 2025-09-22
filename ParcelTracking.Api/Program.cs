using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ParcelTracking.Api.Data;
using ParcelTracking.Api.Helpers;
using ParcelTracking.Api.Repositories.Implementations;
using ParcelTracking.Api.Repositories.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// ✅ Register DapperContext
builder.Services.AddSingleton<DapperContext>();

// ✅ Register JwtHelper
builder.Services.AddSingleton<JwtHelper>();

// Add Services (Repository pattern)
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<IParcelRepository, ParcelRepository>();

// ✅ JWT Authentication
var jwtSetting = builder.Configuration.GetSection("Jwt");
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
        ValidIssuer = jwtSetting["Issuer"],
        ValidAudience = jwtSetting["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSetting["Key"]!)
        )
    };
});

// ✅ Swagger with JWT support (Authorize button)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Parcel Tracking API",
        Version = "v1"
    });

    // 🔑 Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// (Optional) Add CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // ✅ Authorize button will show here
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parcel Tracking API v1");
    });
}

app.UseHttpsRedirection();

// ✅ Enable JWT Authentication
app.UseAuthentication();
app.UseAuthorization();

// (Optional) Enable CORS
app.UseCors("AllowAngular");

app.MapControllers();

app.Run();
