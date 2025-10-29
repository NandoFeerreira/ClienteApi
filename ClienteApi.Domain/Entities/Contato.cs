namespace ClienteApi.Domain.Entities
{
    public class Contato
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;        
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = new Cliente();
    }
}
