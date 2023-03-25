﻿using System.Text;
using AutoHub.Core.Contracts;
using AutoHub.Core.Services;
using AutoHub.Data;
using AutoHub.Data.Contracts;
using AutoHub.Data.Models;
using AutoHub.Data.Profiles;
using AutoHub.Data.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AutoHub.API.Configuration;

public static class ServiceConfigurator
{
    /// <summary>
    /// Configures identity for the API including Swagger documentation, JWT authentication, and IdentityCore.
    /// </summary>
    /// <param name="services">The IServiceCollection to configure.</param>
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "apiWithAuthBackend",
                    ValidAudience = "apiWithAuthBackend",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("!SomethingSecret!")
                    ),
                };
            });

        services
            .AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<AutoHubDbContext>();
    }

    /// <summary>
    /// Configures services for the API.
    /// </summary>
    /// <param name="services">The IServiceCollection to configure.</param>
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<TokenService, TokenService>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); 
        services.AddScoped(typeof(IService<>), typeof(Service<>));
        
        var mapperConfig = new MapperConfiguration(mc => {
            mc.AddProfile(new CarProfile());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);
    }

    /// <summary>
    /// Configures the database for the API. 
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigureDatabase(this WebApplicationBuilder builder)
    { 
        builder.Services.AddDbContext<AutoHubDbContext>(options => options
            .UseNpgsql(builder.Configuration
                .GetConnectionString("DefaultConnection")));
    }
}