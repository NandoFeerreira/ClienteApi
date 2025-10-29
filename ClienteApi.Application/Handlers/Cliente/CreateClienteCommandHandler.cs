using AutoMapper;
using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Application.Common.Constants;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Application.Services;
using ClienteApi.Domain.Enums;
using ClienteApi.Domain.Interfaces;
using MediatR;

namespace ClienteApi.Application.Handlers.Cliente
{
    public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, ClienteDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IViaCepService _viaCepService;

        public CreateClienteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IViaCepService viaCepService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _viaCepService = viaCepService ?? throw new ArgumentNullException(nameof(viaCepService));
        }

        public async Task<ClienteDto> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = _mapper.Map<Domain.Entities.Cliente>(request);

            await PreencherEnderecosComViaCep(cliente.Enderecos);

            await _unitOfWork.Clientes.AddAsync(cliente);
            await _unitOfWork.SaveChangesAsync();

            var clienteCriado = await _unitOfWork.Clientes.GetByIdWithRelationsAsync(cliente.Id);
            return _mapper.Map<ClienteDto>(clienteCriado);
        }

        private async Task PreencherEnderecosComViaCep(IEnumerable<Domain.Entities.Endereco> enderecos)
        {
            foreach (var endereco in enderecos)
            {
                var viaCepData = await _viaCepService.GetAddressByCepAsync(endereco.Cep);

                if (viaCepData == null)
                    throw new InvalidOperationException(string.Format(ErrorMessages.CepNaoEncontrado, endereco.Cep));

                endereco.Logradouro = viaCepData.Logradouro;
                endereco.Cidade = viaCepData.Localidade;

                if (string.IsNullOrWhiteSpace(endereco.Complemento) && !string.IsNullOrWhiteSpace(viaCepData.Complemento))
                {
                    endereco.Complemento = viaCepData.Complemento;
                }
            }
        }
    }
}
