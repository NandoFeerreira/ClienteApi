using ClienteApi.Domain.Entities;

namespace ClienteApi.Domain.Interfaces
{
   
    public interface IClienteRepository : IRepository<Cliente>
    {
        
        Task<Cliente?> GetByIdWithRelationsAsync(int id);
        
        Task<IEnumerable<Cliente>> GetAllWithRelationsAsync();
       
        Task<IEnumerable<Cliente>> SearchByNameAsync(string nome);

        void RemoveEnderecos(IEnumerable<Endereco> enderecos);

        void RemoveContatos(IEnumerable<Contato> contatos);
    }
}
