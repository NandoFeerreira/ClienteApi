using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Application.Common.Constants;
using ClienteApi.Application.Common.Utils;
using ClienteApi.Domain.Interfaces;
using MediatR;

namespace ClienteApi.Application.Handlers.Cliente
{
    public class DeleteClienteCommandHandler : IRequestHandler<DeleteClienteCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteClienteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
        {
            var clienteId = IdConverter.FromBase64(request.Id);
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(clienteId);

            if (cliente == null)
                return false;

            _unitOfWork.Clientes.Remove(cliente);
            var affectedRows = await _unitOfWork.SaveChangesAsync();

            return affectedRows > 0;
        }
    }
}
