using ClienteApi.Application.DTOs.Cliente;
using MediatR;

namespace ClienteApi.Application.Commands.Cliente
{
    public class CreateClienteCommand : IRequest<ClienteDto>
    {
        public string Nome { get; set; } = string.Empty;
        public List<CreateEnderecoDto> Enderecos { get; set; } = [];
        public List<CreateContatoDto> Contatos { get; set; } = [];
    }
}
