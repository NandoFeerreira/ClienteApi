using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Domain.Enums;
using FluentValidation;

namespace ClienteApi.Application.Validators
{
    public class CreateClienteCommandValidator : AbstractValidator<CreateClienteCommand>
    {
        public CreateClienteCommandValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do cliente é obrigatório")
                .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres")
                .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres");

            RuleFor(x => x.Enderecos)
                .NotEmpty().WithMessage("O cliente deve ter pelo menos um endereço")
                .Must(enderecos => !TemCepDuplicado(enderecos)).WithMessage("Não é permitido cadastrar o mesmo CEP mais de uma vez.");

            RuleForEach(x => x.Enderecos)
                .SetValidator(new CreateEnderecoDtoValidator());

            RuleFor(x => x.Contatos)
                .NotEmpty().WithMessage("O cliente deve ter pelo menos um contato")
                .Must(contatos => !TemContatoDuplicado(contatos)).WithMessage("Não é permitido cadastrar o mesmo tipo e texto de contato mais de uma vez.");

            RuleForEach(x => x.Contatos)
                .SetValidator(new CreateContatoDtoValidator());
        }

        private static bool TemCepDuplicado(IEnumerable<DTOs.Cliente.CreateEnderecoDto> enderecos)
        {
            if (enderecos == null) return false;
            var ceps = enderecos.Select(e => e.Cep);
            return ceps.GroupBy(c => c).Any(g => g.Count() > 1);
        }

        private static bool TemContatoDuplicado(IEnumerable<DTOs.Cliente.CreateContatoDto> contatos)
        {
            if (contatos == null) return false;

            var valoresUnicos = new HashSet<string>();
            foreach (var contato in contatos)
            {
                var valorNormalizado = NormalizarValorContato(contato.Texto, contato.Tipo);
                if (string.IsNullOrEmpty(valorNormalizado)) continue;

                if (!valoresUnicos.Add(valorNormalizado))
                    return true; // Encontrou um duplicado
            }
            return false;
        }

        private static string NormalizarValorContato(string valor, string tipo)
        {
            if (string.IsNullOrWhiteSpace(valor)) return string.Empty;

            if (tipo.Equals(TipoContato.Email, StringComparison.OrdinalIgnoreCase))
            {
                return valor.Trim().ToLower();
            }
            if (tipo.Equals(TipoContato.Celular, StringComparison.OrdinalIgnoreCase) || tipo.Equals(TipoContato.Telefone, StringComparison.OrdinalIgnoreCase))
            {
                return new string(valor.Where(char.IsDigit).ToArray());
            }

            return valor.Trim().ToLower(); // Fallback para outros tipos
        }
    }

    public class CreateEnderecoDtoValidator : AbstractValidator<DTOs.Cliente.CreateEnderecoDto>
    {
        public CreateEnderecoDtoValidator()
        {
            RuleFor(x => x.Cep)
                .NotEmpty().WithMessage("O CEP é obrigatório")
                .Matches(@"^\d{5}-?\d{3}$").WithMessage("O CEP deve estar no formato 00000-000 ou 00000000");

            RuleFor(x => x.Numero)
                .NotEmpty().WithMessage("O número do endereço é obrigatório")
                .MaximumLength(20).WithMessage("O número deve ter no máximo 20 caracteres");

            RuleFor(x => x.Complemento)
                .MaximumLength(200).WithMessage("O complemento deve ter no máximo 200 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.Complemento));
        }
    }

    public class CreateContatoDtoValidator : AbstractValidator<DTOs.Cliente.CreateContatoDto>
    {
        public CreateContatoDtoValidator()
        {
            RuleFor(x => x.Tipo)
                .NotEmpty().WithMessage("O tipo de contato é obrigatório")
                .Must(TipoContato.EhValido).WithMessage($"O tipo de contato é inválido. Tipos aceitos: {string.Join(", ", TipoContato.TiposValidos)}");

            RuleFor(x => x.Texto)
                .NotEmpty().WithMessage("O texto do contato é obrigatório")
                .MaximumLength(200).WithMessage("O texto deve ter no máximo 200 caracteres");

            RuleFor(x => x.Texto)
                .EmailAddress().WithMessage("Email inválido")
                .When(x => x.Tipo.Equals(TipoContato.Email.ToString(), StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.Texto)
                .Matches(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$").WithMessage("Telefone/Celular deve estar em um formato válido, como (00) 90000-0000.")
                .When(x => x.Tipo.Equals(TipoContato.Telefone.ToString(), StringComparison.OrdinalIgnoreCase) || x.Tipo.Equals(TipoContato.Celular.ToString(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
