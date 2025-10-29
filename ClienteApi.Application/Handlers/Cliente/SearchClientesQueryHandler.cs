using AutoMapper;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Application.Queries.Cliente;
using ClienteApi.Domain.Interfaces;
using MediatR;

namespace ClienteApi.Application.Handlers.Cliente
{
    public class SearchClientesQueryHandler : IRequestHandler<SearchClientesQuery, IEnumerable<ClienteDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchClientesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ClienteDto>> Handle(SearchClientesQuery request, CancellationToken cancellationToken)
        {
            var clientes = await _unitOfWork.Clientes.SearchByNameAsync(request.Nome);
            return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        }
    }
}
