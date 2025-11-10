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

namespace ClienteApi.Tests.Unit.Handlers.Cliente
{
    public class CreateClienteCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly Mock<IViaCepService> _viaCepServiceMock;
        private readonly IMapper _mapper;

        public CreateClienteCommandHandlerTests()
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
        public async Task Handle_Should_CreateClienteAndReturnClienteDto_WhenCommandIsValid()
        {
            // Arrange
            var command = new CreateClienteCommand
            {
                Nome = "Novo Cliente",
                Enderecos = new List<CreateEnderecoDto> 
                {
                    new CreateEnderecoDto { Cep = "12345-678", Numero = "10" }
                },
                Contatos = new List<CreateContatoDto> 
                {
                    new CreateContatoDto { Tipo = "Email", Texto = "test@test.com" } 
                }
            };

            var viaCepResponse = new ViaCepResponse
            {
                Logradouro = "Rua Teste",
                Localidade = "Cidade Teste",
                Uf = "TS"
            };

            _viaCepServiceMock.Setup(s => s.GetAddressByCepAsync(It.IsAny<string>())).ReturnsAsync(viaCepResponse);          
            _clienteRepositoryMock.Setup(repo => repo.GetByIdWithRelationsAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _mapper.Map<Domain.Entities.Cliente>(command));


            var handler = new CreateClienteCommandHandler(_unitOfWorkMock.Object, _mapper, _viaCepServiceMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(command.Nome);
            
            _clienteRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Domain.Entities.Cliente>(c => c.Nome == command.Nome)), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ThrowInvalidOperationException_WhenCepIsInvalid()
        {
            // Arrange
            var command = new CreateClienteCommand
            {
                Nome = "Novo Cliente",
                Enderecos = new List<CreateEnderecoDto> { new CreateEnderecoDto { Cep = "99999-999" } }
            };

            _viaCepServiceMock.Setup(s => s.GetAddressByCepAsync(It.IsAny<string>())).ReturnsAsync((ViaCepResponse?)null);

            var handler = new CreateClienteCommandHandler(_unitOfWorkMock.Object, _mapper, _viaCepServiceMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}