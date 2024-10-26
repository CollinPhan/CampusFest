using Backend.Infrastructures.Data.DTO;

namespace Backend.API.Services.Interface
{
    public interface IRoleService: IDisposable
    {
        public Task<RoleDTO> GetRoleInformation(int roleId);

        public Task<RoleDTO>GetRoleInformation(string roleName);

        public Task CreateRole(RoleDTO roleDTO);

        public Task UpdateRole(RoleDTO roleDTO);

        public Task DeleteRole(int roleId);

        public Task<int> CountRoleMember(int roleId);
    }
}
