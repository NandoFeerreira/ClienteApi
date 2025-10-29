using ClienteApi.Domain.Entities;

namespace ClienteApi.Domain.Interfaces
{
   
    public interface IClienteRepository : IRepository<Cliente>
    {
        
        Task<Cliente?> GetByIdWithRelationsAsync(int id);
        
        Task<IEnumerable<Cliente>> GetAllWithRelationsAsync();
       
        Task<IEnumerable<Cliente>> SearchByNameAsync(string nome);

        Task<IEnumerable<Cliente>> GetByDateRangeAsync(DateTime dataInicio, DateTime dataFim);       

        Task<bool> ExistsByNameAsync(string nome);

        void RemoveEnderecos(IEnumerable<Endereco> enderecos);

        void RemoveContatos(IEnumerable<Contato> contatos);
    }
}
