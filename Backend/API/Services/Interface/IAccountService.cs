using Backend.Cores.DTO;
using Backend.Cores.ViewModels;

namespace Backend.API.Services.Interface
{
    public interface IAccountService: IDisposable
    {
        Task<AccountDTO> GetAccountInformation(Guid accountId);

        Task<AccountDTO> GetAccountInformation(string username, string password);

        Task<IEnumerable<AccountDTO>> GetAccountPaginated(int page, int page_size, string username = "", string fullname = "", string email = "", string phone = "", string sortby = "", bool IncludeDeleted = false, bool OnlyVerified = false, bool isDecending = false);

        Task AddAccount(AccountDTO accountInfo);

        Task UpdateAccount(AccountDTO account);

        Task UpdatePassword(Guid AccountId, string password);

        Task UpdateUsername(Guid accountId, string username);

        Task UpdateEmail(Guid accountId, string email);

        Task UpdateFullname(Guid accountId, string fullname);

        Task UpdateVerificationStatus(Guid accountId, bool verifyStatus);

        Task RemoveAccount(Guid accountId);

        Task<RoleDTO> GetRole(int roleId);

        Task<IEnumerable<RoleDTO>> GetRoles();

        Task AddRole(RoleDTO roleInformation);

        Task UpdateRole(RoleDTO roleInformation);

        Task RemoveRole(int roleId);
    }
}
