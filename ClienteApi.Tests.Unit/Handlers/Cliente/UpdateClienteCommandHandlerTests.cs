using AutoMapper;
using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Application.DTOs.ViaCep;
using ClienteApi.Application.Handlers.Cliente;
using ClienteApi.Application.Mappings;
using ClienteApi.Application.Services;
using ClienteApi.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;
using ClienteApi.Application.Common.Utils;

namespace ClienteApi.Tests.Unit.Handlers.Cliente
{
    public class UpdateClienteCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly Mock<IViaCepService> _viaCepServiceMock;
        private readonly IMapper _mapper;

        public UpdateClienteCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _viaCepServiceMock = new Mock<IViaCepService>();

            _unitOfWorkMock.Setup(uow => uow.Clientes).Returns(_clienteRepositoryMock.Object);

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();
        }

        [Fact]
        public async Task Handle_Should_UpdateClienteAndReturnClienteDto_WhenCommandIsValid()
        {
            // Arrange
            var clienteId = 1;
            var command = new UpdateClienteCommand
            {
                Id = IdConverter.ToBase64(clienteId),
                Nome = "Cliente Atualizado",
                Enderecos = new List<UpdateEnderecoDto>(),
                Contatos = new List<UpdateContatoDto>()
            };

            var existingCliente = new Domain.Entities.Cliente { Id = clienteId, Nome = "Cliente Original" };

            _clienteRepositoryMock.Setup(repo => repo.GetByIdWithRelationsAsync(clienteId)).ReturnsAsync(existingCliente);
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);          
            _clienteRepositoryMock.Setup(repo => repo.GetByIdWithRelationsAsync(clienteId)).ReturnsAsync(existingCliente);


            var handler = new UpdateClienteCommandHandler(_unitOfWorkMock.Object, _mapper, _viaCepServiceMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(command.Nome);
            
            _clienteRepositoryMock.Verify(repo => repo.Update(It.Is<Domain.Entities.Cliente>(c => c.Id == clienteId)), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ThrowInvalidOperationException_WhenClienteNotFound()
        {
            // Arrange
            var clienteId = 99;
            var command = new UpdateClienteCommand { Id = IdConverter.ToBase64(clienteId), Nome = "Cliente Fantasma" };

            _clienteRepositoryMock.Setup(repo => repo.GetByIdWithRelationsAsync(clienteId)).ReturnsAsync(value: null);

            var handler = new UpdateClienteCommandHandler(_unitOfWorkMock.Object, _mapper, _viaCepServiceMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Handle_Should_ThrowInvalidOperationException_WhenCepIsInvalid()
        {
            // Arrange
            var clienteId = 1;
            var command = new UpdateClienteCommand
            {
                Id = IdConverter.ToBase64(clienteId),
                Nome = "Cliente Atualizado",
                Enderecos = new List<UpdateEnderecoDto> { new UpdateEnderecoDto { Cep = "99999-999" } }
            };

            var existingCliente = new Domain.Entities.Cliente { Id = clienteId, Nome = "Cliente Original" };

            _clienteRepositoryMock.Setup(repo => repo.GetByIdWithRelationsAsync(clienteId)).ReturnsAsync(existingCliente);
            _viaCepServiceMock.Setup(s => s.GetAddressByCepAsync(It.IsAny<string>())).ReturnsAsync((ViaCepResponse)null);

            var handler = new UpdateClienteCommandHandler(_unitOfWorkMock.Object, _mapper, _viaCepServiceMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}