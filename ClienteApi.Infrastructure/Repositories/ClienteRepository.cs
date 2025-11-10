using ClienteApi.Domain.Entities;
using ClienteApi.Domain.Interfaces;
using ClienteApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClienteApi.Infrastructure.Repositories
{
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        public ClienteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Cliente?> GetByIdWithRelationsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Enderecos)
                .Include(c => c.Contatos)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cliente>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Enderecos)
                .Include(c => c.Contatos)
                .OrderByDescending(c => c.DataCadastro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cliente>> SearchByNameAsync(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return [];

            var lowerCaseNome = nome.ToLower();

            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Enderecos)
                .Include(c => c.Contatos)
                .Where(c => c.Nome.ToLower().Contains(lowerCaseNome))
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public void RemoveEnderecos(IEnumerable<Endereco> enderecos)
        {
            _context.Set<Endereco>().RemoveRange(enderecos);
        }

        public void RemoveContatos(IEnumerable<Contato> contatos)
        {
            _context.Set<Contato>().RemoveRange(contatos);
        }
    }
}
