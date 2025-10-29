
using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Application.Validators;
using FluentAssertions;
using Xunit;

namespace ClienteApi.Tests.Unit.Validators
{
    public class DeleteClienteCommandValidatorTests
    {
        private readonly DeleteClienteCommandValidator _validator = new DeleteClienteCommandValidator();

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var command = new DeleteClienteCommand { Id = string.Empty };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Id");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Id_Is_Provided()
        {
            var command = new DeleteClienteCommand { Id = "some-id" };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeTrue();
        }
    }
}
