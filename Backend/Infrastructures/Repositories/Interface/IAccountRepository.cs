using Backend.Cores.Entities;

namespace Backend.Infrastructures.Repositories.Interface
{
    public interface IAccountRepository : IBaseRepository<Account>, IDisposable
    {
        Task<IEnumerable<Account>> GetUnverifiedAccounts();

        Task<IEnumerable<Account>> GetDeletedAccounts();
    }
}
