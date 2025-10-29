namespace ClienteApi.Domain.Entities
{
    public class Endereco
    {
        public int Id { get; set; }

        public string Cep { get; set; } = string.Empty;

        public string Logradouro { get; set; } = string.Empty;

        public string Cidade { get; set; } = string.Empty;

        public string Numero { get; set; } = string.Empty;

        public string? Complemento { get; set; }
        
        public int ClienteId { get; set; }

        public Cliente Cliente { get; set; } = new Cliente();
    }
}
