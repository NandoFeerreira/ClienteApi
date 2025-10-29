
using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Application.Validators;
using FluentAssertions;
using Xunit;
using ClienteApi.Application.Common.Utils;

namespace ClienteApi.Tests.Unit.Validators
{
    public class UpdateClienteCommandValidatorTests
    {
        private readonly UpdateClienteCommandValidator _validator = new UpdateClienteCommandValidator();

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var command = new UpdateClienteCommand { Id = string.Empty, Nome = "Nome Valido" };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Id");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var command = new UpdateClienteCommand
            {
                Id = IdConverter.ToBase64(1),
                Nome = "Cliente Válido",
                Enderecos = new List<UpdateEnderecoDto> 
                {
                    new UpdateEnderecoDto { Cep = "12345-678", Numero = "123" }
                },
                Contatos = new List<UpdateContatoDto> 
                {
                    new UpdateContatoDto { Tipo = "Email", Texto = "teste@valido.com" }
                }
            };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Have_Error_When_Nome_Is_Empty()
        {
            var command = new UpdateClienteCommand { Id = IdConverter.ToBase64(1), Nome = string.Empty };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Nome");
        }
    }
}
