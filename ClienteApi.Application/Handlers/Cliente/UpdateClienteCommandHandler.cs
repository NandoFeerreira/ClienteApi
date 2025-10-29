using AutoMapper;
using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Application.Common.Constants;
using ClienteApi.Application.Common.Utils;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Application.Services;
using ClienteApi.Domain.Enums;
using ClienteApi.Domain.Interfaces;
using MediatR;

namespace ClienteApi.Application.Handlers.Cliente
{
    public class UpdateClienteCommandHandler : IRequestHandler<UpdateClienteCommand, ClienteDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IViaCepService _viaCepService;

        public UpdateClienteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IViaCepService viaCepService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _viaCepService = viaCepService ?? throw new ArgumentNullException(nameof(viaCepService));
        }

        public async Task<ClienteDto> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
        {
            var clienteId = IdConverter.FromBase64(request.Id);
            var cliente = await _unitOfWork.Clientes.GetByIdWithRelationsAsync(clienteId);

            if (cliente == null)
                throw new InvalidOperationException(string.Format(ErrorMessages.ClienteNaoEncontrado, request.Id));

            // Mapeia as propriedades simples
            cliente.Nome = request.Nome;

            // Lógica para atualizar, adicionar e remover entidades relacionadas
            await AtualizarEnderecos(cliente, request.Enderecos);
            AtualizarContatos(cliente, request.Contatos);

            _unitOfWork.Clientes.Update(cliente);
            await _unitOfWork.SaveChangesAsync();

            var clienteAtualizado = await _unitOfWork.Clientes.GetByIdWithRelationsAsync(clienteId);
            return _mapper.Map<ClienteDto>(clienteAtualizado);
        }

        private async Task AtualizarEnderecos(Domain.Entities.Cliente cliente, List<UpdateEnderecoDto> enderecosDto)
        {
            var idsRecebidos = enderecosDto
                .Where(e => !string.IsNullOrEmpty(e.Id))
                .Select(e => IdConverter.FromBase64(e.Id!))
                .ToList();

            // Remove endereços que não vieram no payload
            var enderecosParaRemover = cliente.Enderecos.Where(e => !idsRecebidos.Contains(e.Id)).ToList();
            if (enderecosParaRemover.Any())
                _unitOfWork.Clientes.RemoveEnderecos(enderecosParaRemover);

            foreach (var enderecoDto in enderecosDto)
            {
                var viaCepData = await _viaCepService.GetAddressByCepAsync(enderecoDto.Cep);
                if (viaCepData == null)
                    throw new InvalidOperationException(string.Format(ErrorMessages.CepNaoEncontrado, enderecoDto.Cep));

                if (!string.IsNullOrEmpty(enderecoDto.Id))
                {
                    // Atualiza endereço existente
                    var enderecoId = IdConverter.FromBase64(enderecoDto.Id);
                    var enderecoExistente = cliente.Enderecos.FirstOrDefault(e => e.Id == enderecoId);
                    if (enderecoExistente != null)
                    {
                        _mapper.Map(enderecoDto, enderecoExistente);
                        enderecoExistente.Logradouro = viaCepData.Logradouro;
                        enderecoExistente.Cidade = viaCepData.Localidade;
                    }
                }
                else
                {
                    // Adiciona novo endereço
                    var novoEndereco = _mapper.Map<Domain.Entities.Endereco>(enderecoDto);
                    novoEndereco.Logradouro = viaCepData.Logradouro;
                    novoEndereco.Cidade = viaCepData.Localidade;
                    cliente.Enderecos.Add(novoEndereco);
                }
            }
        }

        private void AtualizarContatos(Domain.Entities.Cliente cliente, List<UpdateContatoDto> contatosDto)
        {
            var idsRecebidos = contatosDto
                .Where(c => !string.IsNullOrEmpty(c.Id))
                .Select(c => IdConverter.FromBase64(c.Id!))
                .ToList();

            // Remove contatos que não vieram no payload
            var contatosParaRemover = cliente.Contatos.Where(c => !idsRecebidos.Contains(c.Id)).ToList();
            if (contatosParaRemover.Any())
                _unitOfWork.Clientes.RemoveContatos(contatosParaRemover);

            foreach (var contatoDto in contatosDto)
            {
                if (!string.IsNullOrEmpty(contatoDto.Id))
                {
                    // Atualiza contato existente
                    var contatoId = IdConverter.FromBase64(contatoDto.Id);
                    var contatoExistente = cliente.Contatos.FirstOrDefault(c => c.Id == contatoId);
                    if (contatoExistente != null)
                    {
                        _mapper.Map(contatoDto, contatoExistente);
                    }
                }
                else
                {
                    // Adiciona novo contato
                    cliente.Contatos.Add(_mapper.Map<Domain.Entities.Contato>(contatoDto));
                }
            }
        }
    }
}
