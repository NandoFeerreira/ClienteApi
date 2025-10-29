using ClienteApi.Application.DTOs.Cliente;
using MediatR;

namespace ClienteApi.Application.Queries.Cliente
{
    public class GetClienteByIdQuery : IRequest<ClienteDto?>
    {
        public string Id { get; set; } = string.Empty;
    }
}
