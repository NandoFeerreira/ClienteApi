using ClienteApi.Application.Commands.Cliente;
using FluentValidation;

namespace ClienteApi.Application.Validators
{
    public class DeleteClienteCommandValidator : AbstractValidator<DeleteClienteCommand>
    {
        public DeleteClienteCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O ID do cliente é obrigatório.");;
        }
    }
}
