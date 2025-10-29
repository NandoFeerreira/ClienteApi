
using ClienteApi.Application.Mappings;
using ClienteApi.Application.Services;
using ClienteApi.Domain.Interfaces;
using ClienteApi.Infrastructure.Data;
using ClienteApi.Infrastructure.ExternalServices;
using ClienteApi.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ClienteApi.API.Extensions
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var databaseProvider = configuration["DatabaseProvider"] ?? "InMemory";

            if (databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("DefaultConnection"),
                        npgsqlOptions => npgsqlOptions.MigrationsAssembly("ClienteApi.Infrastructure")
                    )
                );
            }
            else
            {
                // Configuração para InMemory como Singleton para persistir dados entre requisições
                services.AddSingleton<DbContextOptions<ApplicationDbContext>>(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase("ClienteApiDb")
                    .Options);
                services.AddSingleton<ApplicationDbContext>();
            }

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Repositories e Unit of Work
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // HTTPClient Factory para o ViaCEP Service
            services.AddHttpClient<IViaCepService, ViaCepService>(client =>
            {
                var viaCepBaseUrl = configuration["ExternalApis:ViaCepBaseUrl"];
                client.BaseAddress = new Uri(viaCepBaseUrl ?? "https://viacep.com.br/ws/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.Load("ClienteApi.Application"));
            });

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // FluentValidation
            services.AddValidatorsFromAssembly(Assembly.Load("ClienteApi.Application"));

            return services;
        }
    }
}
