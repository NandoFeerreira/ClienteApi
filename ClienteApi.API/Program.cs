using ClienteApi.API.Extensions;
using ClienteApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabaseConfiguration(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddApiConfiguration()
    .AddSwaggerConfiguration();


var app = builder.Build();


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


app.Run();
