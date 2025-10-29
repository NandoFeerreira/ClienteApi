using ClienteApi.Application.DTOs.Cliente;
using MediatR;

namespace ClienteApi.Application.Commands.Cliente
{
    public class UpdateClienteCommand : IRequest<ClienteDto>
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public List<UpdateEnderecoDto> Enderecos { get; set; } = [];
        public List<UpdateContatoDto> Contatos { get; set; } = [];
    }
}
