using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Domain.Enums;
using FluentValidation;

namespace ClienteApi.Application.Validators
{
    public class UpdateClienteCommandValidator : AbstractValidator<UpdateClienteCommand>
    {
        public UpdateClienteCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O ID do cliente é obrigatório.");

            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do cliente é obrigatório.")
                .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.")
                .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres.");

            RuleFor(x => x.Enderecos)
                .NotEmpty().WithMessage("O cliente deve ter pelo menos um endereço.")
                .Must(NaoTerEnderecoDuplicado).WithMessage("Não é permitido cadastrar o mesmo CEP e número mais de uma vez.");

            RuleForEach(x => x.Enderecos)
                .SetValidator(new UpdateEnderecoDtoValidator());

            RuleFor(x => x.Contatos)
                .NotEmpty().WithMessage("O cliente deve ter pelo menos um contato.")
                .Must(NaoTerContatoDuplicado).WithMessage("Não é permitido cadastrar o mesmo contato mais de uma vez.");

            RuleForEach(x => x.Contatos)
                .SetValidator(new UpdateContatoDtoValidator());
        }

        private static bool NaoTerEnderecoDuplicado(IEnumerable<UpdateEnderecoDto> enderecos)
        {
            if (enderecos == null) return true;
            var unicos = new HashSet<Tuple<string, string>>();
            foreach (var end in enderecos)
            {
                if (!unicos.Add(Tuple.Create(end.Cep, end.Numero)))
                    return false; 
            }
            return true;
        }

        private static bool NaoTerContatoDuplicado(IEnumerable<UpdateContatoDto> contatos)
        {
            if (contatos == null) return true;

            var textosUnicos = new HashSet<string>();
            foreach (var contato in contatos)
            {
                var textoNormalizado = NormalizarTextoContato(contato.Texto, contato.Tipo);
                if (string.IsNullOrEmpty(textoNormalizado)) continue;

                if (!textosUnicos.Add(textoNormalizado))
                    return false;
            }
            return true;
        }

        private static string NormalizarTextoContato(string texto, string tipo)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            if (tipo.Equals(TipoContato.Email, StringComparison.OrdinalIgnoreCase))
            {
                return texto.Trim().ToLower();
            }
      
            if (tipo.Equals(TipoContato.Celular, StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals(TipoContato.Telefone, StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals(TipoContato.WhatsApp, StringComparison.OrdinalIgnoreCase))
            {
                return new string(texto.Where(char.IsDigit).ToArray());
            }
       
            return texto.Trim().ToLower();
        }
    }

    public class UpdateEnderecoDtoValidator : AbstractValidator<UpdateEnderecoDto>
    {
        public UpdateEnderecoDtoValidator()
        {
            RuleFor(x => x.Cep)
                .NotEmpty().WithMessage("O CEP é obrigatório.")
                .Matches(@"^\d{5}-?\d{3}$").WithMessage("O CEP deve estar no formato 00000-000 ou 00000000.");

            RuleFor(x => x.Numero)
                .NotEmpty().WithMessage("O número do endereço é obrigatório.")
                .MaximumLength(20).WithMessage("O número deve ter no máximo 20 caracteres.");
        }
    }

    public class UpdateContatoDtoValidator : AbstractValidator<UpdateContatoDto>
    {
        public UpdateContatoDtoValidator()
        {
            RuleFor(x => x.Tipo)
                .NotEmpty().WithMessage("O tipo de contato é obrigatório.")
                .Must(TipoContato.EhValido).WithMessage($"O tipo de contato é inválido. Tipos aceitos: {string.Join(", ", TipoContato.TiposValidos)}.");

            RuleFor(x => x.Texto)
                .NotEmpty().WithMessage("O texto do contato é obrigatório.")
                .MaximumLength(200).WithMessage("O texto deve ter no máximo 200 caracteres.");
        }
    }
}