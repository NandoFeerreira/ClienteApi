namespace ClienteApi.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }

        public ICollection<Endereco> Enderecos { get; set; } = [];
        public ICollection<Contato> Contatos { get; set; } = [];
    }
}
