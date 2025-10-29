
using AutoMapper;
using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Application.Handlers.Cliente;
using ClienteApi.Application.Mappings;
using ClienteApi.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;
using ClienteApi.Application.Common.Utils;

namespace ClienteApi.Tests.Unit.Handlers.Cliente
{
    public class DeleteClienteCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;

        public DeleteClienteCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _clienteRepositoryMock = new Mock<IClienteRepository>();

            _unitOfWorkMock.Setup(uow => uow.Clientes).Returns(_clienteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenClienteIsDeletedSuccessfully()
        {
            // Arrange
            var clienteId = 1;
            var command = new DeleteClienteCommand { Id = IdConverter.ToBase64(clienteId) };
            var existingCliente = new Domain.Entities.Cliente { Id = clienteId, Nome = "Cliente Para Deletar" };

            _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteId)).ReturnsAsync(existingCliente);
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteClienteCommandHandler(_unitOfWorkMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _clienteRepositoryMock.Verify(repo => repo.Remove(It.Is<Domain.Entities.Cliente>(c => c.Id == clienteId)), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFalse_WhenClienteNotFound()
        {
            // Arrange
            var clienteId = 99;
            var command = new DeleteClienteCommand { Id = IdConverter.ToBase64(clienteId) };

            _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteId)).ReturnsAsync(value: null);

            var handler = new DeleteClienteCommandHandler(_unitOfWorkMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _clienteRepositoryMock.Verify(repo => repo.Remove(It.IsAny<Domain.Entities.Cliente>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ThrowFormatException_WhenIdIsInvalid()
        {
            // Arrange
            var command = new DeleteClienteCommand { Id = "id-invalido" };
            var handler = new DeleteClienteCommandHandler(_unitOfWorkMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
        }
    }
}
