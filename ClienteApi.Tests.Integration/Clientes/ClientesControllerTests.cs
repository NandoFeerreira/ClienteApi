using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace ClienteApi.Tests.Integration.Clientes
{
    public class ClientesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public ClientesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                SeedDatabase(context);
            }
        }

        private void SeedDatabase(ApplicationDbContext context)
        {
            context.Clientes.Add(new Domain.Entities.Cliente { Nome = "Cliente Teste 1" });
            context.Clientes.Add(new Domain.Entities.Cliente { Nome = "Cliente Teste 2" });
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAll_Should_Return_Success_And_Seeded_Clientes()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/clientes");

            // Assert
            response.EnsureSuccessStatusCode(); 
            var clientes = await response.Content.ReadFromJsonAsync<List<ClienteDto>>();
            
            clientes.Should().NotBeNull();
            clientes.Should().HaveCount(2);
            clientes.Should().Contain(c => c.Nome == "Cliente Teste 1");
        }
        [Fact]
        public async Task Post_Should_Create_Cliente_And_Return_Created()
        {
            // Arrange
            var command = new Application.Commands.Cliente.CreateClienteCommand
            {
                Nome = "Novo Cliente Integration",
                Enderecos = new List<CreateEnderecoDto> 
                {
                    new CreateEnderecoDto { Cep = "01001-000", Numero = "123" } 
                },
                Contatos = new List<CreateContatoDto> 
                {
                    new CreateContatoDto { Tipo = "Email", Texto = "novo@teste.com" }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/clientes", command);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            var cliente = await response.Content.ReadFromJsonAsync<ClienteDto>();
            cliente.Should().NotBeNull();
            cliente.Nome.Should().Be(command.Nome);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_Should_Return_Correct_Cliente()
        {
            // Arrange          
            var clienteId = 1;
            var base64Id = Application.Common.Utils.IdConverter.ToBase64(clienteId);

            // Act
            var response = await _client.GetAsync($"/api/v1/clientes/{base64Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var cliente = await response.Content.ReadFromJsonAsync<ClienteDto>();
            cliente.Should().NotBeNull();
            cliente.Nome.Should().Be("Cliente Teste 1");
        }
        [Fact]
        public async Task Put_Should_Update_Cliente_And_Return_Ok()
        {
            // Arrange
            var clienteId = 2;
            var base64Id = Application.Common.Utils.IdConverter.ToBase64(clienteId);
            var command = new Application.Commands.Cliente.UpdateClienteCommand
            {
                Id = base64Id,
                Nome = "Cliente Integration Atualizado",
                Enderecos = new List<UpdateEnderecoDto>
                {
                    new UpdateEnderecoDto { Cep = "01001-000", Numero = "100" } 
                },
                Contatos = new List<UpdateContatoDto>
                {
                    new UpdateContatoDto { Tipo = "Email", Texto = "update@teste.com" }
                }
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/v1/clientes/{base64Id}", command);

            // Assert
            response.EnsureSuccessStatusCode();
            var cliente = await response.Content.ReadFromJsonAsync<ClienteDto>();
            cliente.Should().NotBeNull();
            cliente.Nome.Should().Be(command.Nome);
        }

        [Fact]
        public async Task Delete_Should_Remove_Cliente_And_Return_NoContent()
        {
            // Arrange
            var clienteId = 1; 
            var base64Id = Application.Common.Utils.IdConverter.ToBase64(clienteId);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/v1/clientes/{base64Id}");

            // Assert
            deleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

            var getResponse = await _client.GetAsync($"/api/v1/clientes/{base64Id}");
            getResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
