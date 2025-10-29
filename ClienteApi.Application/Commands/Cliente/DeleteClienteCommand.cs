using MediatR;

namespace ClienteApi.Application.Commands.Cliente
{
    public class DeleteClienteCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
    }
}