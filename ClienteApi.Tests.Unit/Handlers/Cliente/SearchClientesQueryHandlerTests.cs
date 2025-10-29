
using AutoMapper;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Application.Handlers.Cliente;
using ClienteApi.Application.Mappings;
using ClienteApi.Application.Queries.Cliente;
using ClienteApi.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClienteApi.Tests.Unit.Handlers.Cliente
{
    public class SearchClientesQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly IMapper _mapper;

        public SearchClientesQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _clienteRepositoryMock = new Mock<IClienteRepository>();

            _unitOfWorkMock.Setup(uow => uow.Clientes).Returns(_clienteRepositoryMock.Object);

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();
        }

        [Fact]
        public async Task Handle_Should_ReturnMatchingClientesAsClienteDto()
        {
            // Arrange
            var searchTerm = "Test";
            var clientes = new List<Domain.Entities.Cliente>
            {
                new Domain.Entities.Cliente { Id = 1, Nome = "Cliente Teste 1" },
                new Domain.Entities.Cliente { Id = 2, Nome = "Outro Cliente Teste" }
            };

            _clienteRepositoryMock.Setup(repo => repo.SearchByNameAsync(searchTerm)).ReturnsAsync(clientes);

            var handler = new SearchClientesQueryHandler(_unitOfWorkMock.Object, _mapper);
            var query = new SearchClientesQuery { Nome = searchTerm };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.All(c => c.Nome.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }
    }
}
