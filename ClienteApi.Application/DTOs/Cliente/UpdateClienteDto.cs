namespace ClienteApi.Application.DTOs.Cliente
{
    public class UpdateClienteDto
    {
        public string Nome { get; set; } = string.Empty;
        public List<UpdateEnderecoDto> Enderecos { get; set; } = new();
        public List<UpdateContatoDto> Contatos { get; set; } = new();
    }

    public class UpdateEnderecoDto
    {
        public string? Id { get; set; }  
        public string Cep { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
    }

    public class UpdateContatoDto
    {
        public string? Id { get; set; }  
        public string Tipo { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
    }
}
