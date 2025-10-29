using ClienteApi.Application.DTOs.Cliente;
using MediatR;

namespace ClienteApi.Application.Queries.Cliente
{
    public class SearchClientesQuery : IRequest<IEnumerable<ClienteDto>>
    {
        public string Nome { get; set; } = string.Empty;
    }
}
