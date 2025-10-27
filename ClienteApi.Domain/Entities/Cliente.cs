namespace ClienteApi.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        
        public Endereco? Endereco { get; set; }
        public ICollection<Contato> Contatos { get; set; } = new List<Contato>();
    }
}
