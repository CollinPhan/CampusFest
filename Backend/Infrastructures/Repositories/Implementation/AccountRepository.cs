using Backend.Cores.Entities;
using Backend.Infrastructures.Data;
using Backend.Infrastructures.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructures.Repositories.Implementation
{
    public class AccountRepository : BaseRepositoy<Account>, IAccountRepository
    {
        public AccountRepository(CampusFestDbContext context) : base(context) { }

        public async Task<IEnumerable<Account>> GetDeletedAccounts()
        {
            return await Set.Where(x => x.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetUnverifiedAccounts()
        {
            return await Set.Where(x => !x.IsVerified).ToListAsync();
        }
    }
}
