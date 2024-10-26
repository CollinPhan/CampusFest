using Backend.Cores.Entities;
using Backend.Infrastructures.Data;
using Backend.Infrastructures.Repositories.Interface;

namespace Backend.Infrastructures.Repositories.Implementation
{
    public class RoleRepository : BaseRepositoy<Role>, IRoleRepository
    {
        public RoleRepository(CampusFestDbContext context) : base(context) { }
    }
}
