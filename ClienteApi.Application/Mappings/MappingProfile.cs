using AutoMapper;
using ClienteApi.Application.Commands.Cliente;
using ClienteApi.Application.Common.Utils;
using ClienteApi.Application.DTOs.Cliente;
using ClienteApi.Domain.Entities;

namespace ClienteApi.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Conversores de ID
            CreateMap<int, string>().ConvertUsing(id => IdConverter.ToBase64(id));
            CreateMap<string, int>().ConvertUsing(base64 => IdConverter.FromBase64(base64));

            ConfigureClienteMappings();
            ConfigureEnderecoMappings();
            ConfigureContatoMappings();
            ConfigureCommandMappings();
        }

        private void ConfigureClienteMappings()
        {
            CreateMap<Cliente, ClienteDto>();
            CreateMap<CreateClienteDto, Cliente>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<UpdateClienteDto, Cliente>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.Ignore());
        }

        private void ConfigureEnderecoMappings()
        {
            CreateMap<Endereco, EnderecoDto>();
            CreateMap<CreateEnderecoDto, Endereco>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Logradouro, opt => opt.Ignore())
                .ForMember(dest => dest.Cidade, opt => opt.Ignore())
                .ForMember(dest => dest.ClienteId, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore());
            CreateMap<UpdateEnderecoDto, Endereco>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? 0 : IdConverter.FromBase64(src.Id)))
                .ForMember(dest => dest.Logradouro, opt => opt.Ignore())
                .ForMember(dest => dest.Cidade, opt => opt.Ignore())
                .ForMember(dest => dest.ClienteId, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore());
        }

        private void ConfigureContatoMappings()
        {
            CreateMap<Contato, ContatoDto>();
            CreateMap<CreateContatoDto, Contato>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClienteId, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore());
            CreateMap<UpdateContatoDto, Contato>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? 0 : IdConverter.FromBase64(src.Id)))
                .ForMember(dest => dest.ClienteId, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore());
        }

        private void ConfigureCommandMappings()
        {
            CreateMap<CreateClienteCommand, Cliente>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<UpdateClienteCommand, Cliente>()
                .ForMember(dest => dest.DataCadastro, opt => opt.Ignore());
        }
    }
}
