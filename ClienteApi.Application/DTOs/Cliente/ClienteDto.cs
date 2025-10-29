namespace ClienteApi.Application.DTOs.Cliente
{
    public class ClienteDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public List<EnderecoDto> Enderecos { get; set; } = [];
        public List<ContatoDto> Contatos { get; set; } = [];
    }
}

