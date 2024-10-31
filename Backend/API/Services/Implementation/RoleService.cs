using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.Entities;
using Backend.Infrastructures.Data.DTO;
using Backend.Infrastructures.Repositories.Interface;
using Microsoft.Identity.Client;
using System.Linq.Expressions;

namespace Backend.API.Services.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly IBaseRepository<Role> roleRepo;
        private readonly IMapper mapper;
        private bool disposedValue;

        public RoleService(IBaseRepository<Role> roleRepo, IMapper mapper)
        {
            this.roleRepo = roleRepo;
            this.mapper = mapper;
        }

        public async Task<int> CountRoleMember(int roleId)
        {
            var entity = await roleRepo.GetById(roleId);

            if (entity == null)
            {
                var exception = new Exception("Role information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Role_Exception");
                exception.Data.Add("detail", "Role_Not_Existed");
                exception.Data.Add("value", roleId);

                throw exception;
            }

            return entity.Accounts.Count;
        }

        public async Task CreateRole(RoleDTO roleDTO)
        {
            if (await roleRepo.IsExist("Name", roleDTO.Name))
            {
                var exception = new Exception("Role already existed");

                // Add Data to Exception
                exception.Data.Add("error", "Role_Exception");
                exception.Data.Add("detail", "Role_Existed");
                exception.Data.Add("value", roleDTO.Name);

                throw exception;
            }

            var entity = mapper.Map<RoleDTO, Role>(roleDTO);

            await roleRepo.Create(entity);
        }

        public async Task DeleteRole(int roleId)
        {
            var target = await roleRepo.GetById(roleId);

            if (target == null)
            {
                var exception = new Exception("Role information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Role_Exception");
                exception.Data.Add("detail", "Role_Not_Existed");
                exception.Data.Add("value", roleId);

                throw exception;
            }



            await roleRepo.Remove(target);
        }

        public async Task<RoleDTO> GetRoleInformation(int roleId)
        {
            var entity = await roleRepo.GetById(roleId);

            if (entity == null)
            {
                var exception = new Exception("Role information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Role_Exception");
                exception.Data.Add("detail", "Role_Not_Existed");
                exception.Data.Add("value", roleId);

                throw exception;
            }

            return mapper.Map<Role, RoleDTO>(entity);
        }

        public async Task<RoleDTO> GetRoleInformation(string roleName)
        {
            Expression<Func<Role, bool>> filter = x => x.Name == roleName;

            var entity = (await roleRepo.GetPaginated(1, 1, null, filter)).FirstOrDefault();

            if (entity == null)
            {
                var exception = new Exception("Role information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Role_Exception");
                exception.Data.Add("detail", "Role_Not_Existed");
                exception.Data.Add("value", roleName);

                throw exception;
            }

            return mapper.Map<Role, RoleDTO>(entity);
        }

        public async Task UpdateRole(RoleDTO roleDTO)
        {
            var target = await roleRepo.GetById(roleDTO.Id);

            if (target == null)
            {
                var exception = new Exception("Role information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Role_Exception");
                exception.Data.Add("detail", "Role_Not_Existed");
                exception.Data.Add("value", roleDTO.Id);

                throw exception;
            }

            target.Name = roleDTO.Name;

            await roleRepo.Update(target);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    roleRepo.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
