
using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Application.Validators;
using FluentAssertions;
using Xunit;

namespace ClienteApi.Tests.Unit.Validators
{
    public class CreateClienteCommandValidatorTests
    {
        private readonly CreateClienteCommandValidator _validator = new CreateClienteCommandValidator();

        [Fact]
        public void Should_Have_Error_When_Nome_Is_Empty()
        {
            var command = new CreateClienteCommand { Nome = string.Empty };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Nome");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var command = new CreateClienteCommand
            {
                Nome = "Cliente Válido",
                Enderecos = new List<CreateEnderecoDto> 
                {
                    new CreateEnderecoDto { Cep = "12345-678", Numero = "123" }
                },
                Contatos = new List<CreateContatoDto> 
                {
                    new CreateContatoDto { Tipo = "Email", Texto = "teste@valido.com" }
                }
            };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Have_Error_When_Enderecos_Is_Empty()
        {
            var command = new CreateClienteCommand { Nome = "Cliente Válido", Contatos = new List<CreateContatoDto> { new CreateContatoDto { Tipo = "Email", Texto = "teste@valido.com" } } };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Enderecos");
        }

        [Fact]
        public void Should_Have_Error_When_Cep_Is_Invalid()
        {
            var command = new CreateClienteCommand
            {
                Nome = "Cliente Válido",
                Enderecos = new List<CreateEnderecoDto> { new CreateEnderecoDto { Cep = "12345", Numero = "123" } },
                Contatos = new List<CreateContatoDto> { new CreateContatoDto { Tipo = "Email", Texto = "teste@valido.com" } }
            };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Enderecos[0].Cep");
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new CreateClienteCommand
            {
                Nome = "Cliente Válido",
                Enderecos = new List<CreateEnderecoDto> { new CreateEnderecoDto { Cep = "12345-678", Numero = "123" } },
                Contatos = new List<CreateContatoDto> { new CreateContatoDto { Tipo = "Email", Texto = "email-invalido" } }
            };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Contatos[0].Texto");
        }

        [Fact]
        public void Should_Have_Error_When_Cep_And_Numero_Are_Duplicated()
        {
            var command = new CreateClienteCommand
            {
                Nome = "Cliente Válido",
                Enderecos = new List<CreateEnderecoDto>
                {
                    new CreateEnderecoDto { Cep = "12345-678", Numero = "1" },
                    new CreateEnderecoDto { Cep = "12345-678", Numero = "1" } // Mesmo CEP E Número
                },
                Contatos = new List<CreateContatoDto> { new CreateContatoDto { Tipo = "Email", Texto = "teste@valido.com" } }
            };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Enderecos");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Same_Cep_With_Different_Numeros()
        {
            var command = new CreateClienteCommand
            {
                Nome = "Cliente Válido",
                Enderecos = new List<CreateEnderecoDto>
                {
                    new CreateEnderecoDto { Cep = "12345-678", Numero = "1" },
                    new CreateEnderecoDto { Cep = "12345-678", Numero = "2" } // Mesmo CEP, números diferentes
                },
                Contatos = new List<CreateContatoDto> { new CreateContatoDto { Tipo = "Email", Texto = "teste@valido.com" } }
            };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeTrue();
        }
    }
}
