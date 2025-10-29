
using FluentValidation.AspNetCore;

namespace ClienteApi.API.Extensions
{
    public static class ApiConfigurationSetup
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            services.AddFluentValidationAutoValidation();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Cliente API - Desafio C#",
                    Version = "v1",
                    Description = "API RESTful para gerenciamento de clientes com integração ViaCEP",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Desafio C#",
                        Email = "feernando.dev@gmail.com"
                    }
                });
            });

            return services;
        }
    }
}
