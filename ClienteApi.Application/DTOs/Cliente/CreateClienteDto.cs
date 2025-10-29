namespace ClienteApi.Application.DTOs.Cliente
{
    public class CreateClienteDto
    {
        public string Nome { get; set; } = string.Empty;
        public List<CreateEnderecoDto> Enderecos { get; set; } = [];
        public List<CreateContatoDto> Contatos { get; set; } = [];
    }

    public class CreateEnderecoDto
    {
        public string Cep { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
    }

    public class CreateContatoDto
    {
        public string Tipo { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
    }
}
