using AutoMapper;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Application.Queries.Cliente;
using ClienteApi.Domain.Interfaces;
using MediatR;

namespace ClienteApi.Application.Handlers.Cliente
{
    public class GetAllClientesQueryHandler : IRequestHandler<GetAllClientesQuery, IEnumerable<ClienteDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllClientesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ClienteDto>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
        {
            var clientes = await _unitOfWork.Clientes.GetAllWithRelationsAsync();
            return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        }
    }
}
