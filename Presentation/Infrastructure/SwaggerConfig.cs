using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Presentation.Infrastructure
{
    public static class SwaggerConfig
    {
        private const string JwtIssuerConfigKey = "Jwt:Issuer";
        private const string JwtAudienceConfigKey = "Jwt:Audience";
        private const string JwtKeyConfigKey = "Jwt:Key";
        
        public static void ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            AddSwagger(services);
            AddJwtAuthentication(services, configuration);
        }

        private static void AddSwagger(IServiceCollection services)
        {
            OpenApiSecurityScheme securityScheme = new()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JSON Web Token based security",
            };

            OpenApiSecurityRequirement securityReq = new()
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
            };

            OpenApiInfo info = new()
            {
                Version = "v1",
                Title = "Minimal API - Find a Table - Management",
                Description = $"Minimal API - Management Authenticate UserProfile e Table - {DateTime.UtcNow.Year}",
                TermsOfService = new Uri("http://www.example.com"),
            };

            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", info);
                o.AddSecurityDefinition("Bearer", securityScheme);
                o.AddSecurityRequirement(securityReq);
            });
        }

        private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration[JwtIssuerConfigKey],
                    ValidateAudience = true,
                    ValidAudience = configuration[JwtAudienceConfigKey],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[JwtKeyConfigKey] ?? string.Empty)),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }
}