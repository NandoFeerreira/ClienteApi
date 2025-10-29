namespace ClienteApi.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IClienteRepository Clientes { get; }

       
        Task<int> SaveChangesAsync();

        
        Task BeginTransactionAsync();

        
        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}
