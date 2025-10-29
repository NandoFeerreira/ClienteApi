using AutoMapper;
using ClienteApi.Application.Common.Utils;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Application.Handlers.Cliente;
using ClienteApi.Application.Mappings;
using ClienteApi.Application.Queries.Cliente;
using ClienteApi.Domain.Entities;
using ClienteApi.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClienteApi.Tests.Unit.Handlers.Cliente
{
    public class GetClienteByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly IMapper _mapper;

        public GetClienteByIdQueryHandlerTests()
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
        public async Task Handle_Should_ReturnClienteDto_WhenClienteExists()
        {
            // Arrange
            var clienteId = 1;
            var cliente = new Domain.Entities.Cliente { Id = clienteId, Nome = "Cliente Teste" };
            var base64Id = IdConverter.ToBase64(clienteId);

            _clienteRepositoryMock.Setup(repo => repo.GetByIdWithRelationsAsync(clienteId))
                .ReturnsAsync(cliente);

            var handler = new GetClienteByIdQueryHandler(_unitOfWorkMock.Object, _mapper);
            var query = new GetClienteByIdQuery { Id = base64Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ClienteDto>();
            result.Nome.Should().Be(cliente.Nome);
            _clienteRepositoryMock.Verify(repo => repo.GetByIdWithRelationsAsync(clienteId), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnNull_WhenClienteDoesNotExist()
        {
            // Arrange
            var clienteId = 99;
            var base64Id = IdConverter.ToBase64(clienteId);

            _clienteRepositoryMock.Setup(repo => repo.GetByIdWithRelationsAsync(clienteId))
                .ReturnsAsync(value: null);

            var handler = new GetClienteByIdQueryHandler(_unitOfWorkMock.Object, _mapper);
            var query = new GetClienteByIdQuery { Id = base64Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _clienteRepositoryMock.Verify(repo => repo.GetByIdWithRelationsAsync(clienteId), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ThrowFormatException_WhenIdIsInvalidBase64()
        {
            // Arrange
            var handler = new GetClienteByIdQueryHandler(_unitOfWorkMock.Object, _mapper);
            var query = new GetClienteByIdQuery { Id = "id-invalido" };

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
        }
    }
}