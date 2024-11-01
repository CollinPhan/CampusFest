using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.DTO;
using Backend.Cores.Entities;
using Backend.Cores.Exceptions;
using Backend.Infrastructures.Repositories.Interface;
using Backend.Utilities.Helpers;

namespace Backend.API.Services.Implementation
{
    public class CampusService : ICampusService
    {
        private readonly IBaseRepository<Campus> campusRepo;
        private readonly IMapper mapper;
        private bool disposedValue;

        public CampusService(IBaseRepository<Campus> campusRepo, IMapper mapper)
        {
            this.campusRepo = campusRepo;
            this.mapper = mapper;
        }

        public async Task AddCampus(CampusDTO campusInfo)
        {
            var entity = mapper.Map<CampusDTO,Campus>(campusInfo);

            if (await campusRepo.FindFirstMatch(x => x.Name.ToLower().Contains(campusInfo.Name.ToLower())) != null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "This campus seems to be added before",
                    "Campus_Creation_Exception",
                    "Invalid",
                    "Campus name is already existed.",
                    "There is a campus with the same name in the database, if you want to update it, please use PUT method instead.");
                return;
            }

            await campusRepo.Create(entity);
        }

        public async Task DeleteCampus(int campusId)
        {
            var entity = await campusRepo.GetById(campusId);

            if (entity == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Campus information not found with provided Id.",
                    "Campus_Deletion_Exception",
                    "Invalid",
                    "Campus does not exist.",
                    "There is no campus data found for the provided Id");
                return;
            }
            else
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Campus table is create-read only",
                    "Campus_Deletion_Exception",
                    "Invalid",
                    "Campus does not exist.",
                    "There is no campus data found for the provided Id");
                return;
            }
        }

        public async Task<CampusDTO?> GetCampus(int campusId)
        {
            var target = await campusRepo.GetById(campusId);

            return target != null ? mapper.Map<Campus, CampusDTO>(target) : null;
        }

        public async Task<IEnumerable<CampusDTO>> GetCampusPaginated(int page, int page_size, string keyword = "")
        {
            var result = await campusRepo.GetPaginated(page, page_size, null, x => x.Name.ToLower().Contains(keyword.ToLower()));

            return mapper.Map<IEnumerable<Campus>, IEnumerable<CampusDTO>>(result);
        }

        public async Task UpdateCampus(CampusDTO campusInfo)
        {
            var target = await campusRepo.FindFirstMatch("Id", campusInfo.Id);

            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Campus information not found with provided Id.",
                    "Campus_Deletion_Exception",
                    "Invalid",
                    "Campus does not exist.",
                    "There is no campus data found for the provided Id");
                return;
            }

            target.Name = campusInfo.Name;
            target.Description = campusInfo.Description;
            target.Address = campusInfo.Address;
            target.Phone = campusInfo.Phone;
            target.Email = campusInfo.Email;

            await campusRepo.Update(target);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    campusRepo.Dispose();
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
