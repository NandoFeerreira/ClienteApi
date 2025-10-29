using ClienteApi.Application.DTOs.Cliente;
using MediatR;

namespace ClienteApi.Application.Queries.Cliente
{
    public class GetAllClientesQuery : IRequest<IEnumerable<ClienteDto>>
    {
    }
}
