
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
    public class GetAllClientesQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly IMapper _mapper;

        public GetAllClientesQueryHandlerTests()
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
        public async Task Handle_Should_ReturnAllClientesAsClienteDto()
        {
            // Arrange
            var clientes = new List<Domain.Entities.Cliente>
            {
                new Domain.Entities.Cliente { Id = 1, Nome = "Cliente 1" },
                new Domain.Entities.Cliente { Id = 2, Nome = "Cliente 2" }
            };

            _clienteRepositoryMock.Setup(repo => repo.GetAllWithRelationsAsync()).ReturnsAsync(clientes);

            var handler = new GetAllClientesQueryHandler(_unitOfWorkMock.Object, _mapper);
            var query = new GetAllClientesQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Nome.Should().Be("Cliente 1");
        }
    }
}
