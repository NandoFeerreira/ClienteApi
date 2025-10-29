using ClienteApi.API.Extensions;
using ClienteApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Adiciona as configurações de Injeção de Dependência
builder.Services
    .AddDatabaseConfiguration(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddApiConfiguration()
    .AddSwaggerConfiguration();

// 2. Constrói a aplicação
var app = builder.Build();

// 3. Configura o pipeline de middlewares HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cliente API v1");
        c.RoutePrefix = string.Empty;
    });

    // Seed de dados apenas em ambiente de desenvolvimento com InMemory
    var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "InMemory";
    if (databaseProvider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            DatabaseSeeder.SeedData(context);
        }
    }
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 4. Executa a aplicação
app.Run();
