using AutoMapper;
using ClienteApi.Application.Common.Utils;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Application.Queries.Cliente;
using ClienteApi.Domain.Interfaces;
using MediatR;

namespace ClienteApi.Application.Handlers.Cliente
{
    public class GetClienteByIdQueryHandler : IRequestHandler<GetClienteByIdQuery, ClienteDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetClienteByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ClienteDto?> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
        {
            var clienteId = IdConverter.FromBase64(request.Id);
            var cliente = await _unitOfWork.Clientes.GetByIdWithRelationsAsync(clienteId);

            return _mapper.Map<ClienteDto?>(cliente);
        }
    }
}
