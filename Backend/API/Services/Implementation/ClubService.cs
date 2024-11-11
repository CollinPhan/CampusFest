using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.DTO;
using Backend.Cores.Entities;
using Backend.Cores.Exceptions;
using Backend.Infrastructures.Repositories.Interface;
using Backend.Utilities.Helpers;
using Microsoft.Identity.Client;
using System.Linq.Expressions;
using System.Security.Principal;

namespace Backend.API.Services.Implementation
{
    public class ClubService : IClubService
    {
        private readonly IBaseRepository<Club> clubRepo;
        private readonly IBaseRepository<Campus> campusRepo;
        private readonly IBaseRepository<Account> accountRepo;
        private readonly IMapper mapper;
        private bool disposedValue;

        public ClubService(IBaseRepository<Club> clubRepo, IBaseRepository<Campus> campusRepo, IBaseRepository<Account> accountRepo, IMapper mapper)
        {
            this.clubRepo = clubRepo;
            this.campusRepo = campusRepo;
            this.accountRepo = accountRepo;
            this.mapper = mapper;
        }

        public async Task<ClubDTO?> GetClub(Guid id)
        {
            var target = await clubRepo.GetById(id);

            if (target == null)
            {
                return null;
            }

            return mapper.Map<Club, ClubDTO>(target);
        }

        public async Task<IEnumerable<ClubDTO>> GetPaginated(int page = 1, int page_size = 10, string keyword = "", string orderBy = "", bool includeInactivated = false, bool includeDeleted = false)
        {
            Expression<Func<Club, bool>> filter =  x => x.Name.ToLower().Contains(keyword.ToLower());
            IEnumerable<string> includes = new List<string> { "Campus" };

            var result = await clubRepo.GetPaginated(page, page_size, includes, filter, null!, false, false);

            return mapper.Map<IEnumerable<Club>, IEnumerable<ClubDTO>>(result);
        }

        public async Task<Guid?> AddNewClub(ClubDTO clubInformation)
        {
            Club entity = mapper.Map<ClubDTO, Club>(clubInformation);

            await clubRepo.Create(entity);

            return entity.Id;
        }

        public async Task<ClubDTO> UpdateClub(ClubDTO clubInformation)
        {
            var target = await clubRepo.GetById(clubInformation.Id);

            // Data validation for club update
            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "club does not exist",
                   error: "club_Update_Exception",
                   type: "Invalid",
                   summary: "club does not exist.",
                   detail: @"There is no club exist for the provided clubId.",
                   value: clubInformation.Id);
                return null!;
            }

            if (target.IsDeleted)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "club state is invalid",
                   error: "club_Update_Exception",
                   type: "Invalid",
                   summary: "Can not update a deleted club",
                   detail: @"This club is marked as deleted. No futher update operations can be used on this club.",
                   value: clubInformation.Id);
                return null!;
            }


            target.Name = clubInformation.Name;
            target.Email = clubInformation.Email;
            target.Description = clubInformation.Description;

            await clubRepo.Update(target);

            return clubInformation;
        }

        public async Task<Guid?> DeleteClub(Guid id)
        {
            var target = await clubRepo.GetById(id);

            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Club does not exist",
                   error: "CLub_Deletion_Exception",
                   type: "Invalid",
                   summary: "Club id is not valid",
                   detail: @"There is no club with provided id exist",
                   value:id);
                return null;
            }

            target.IsDeleted = true;

            await clubRepo.Update(target);

            return target.Id;
        }

        public async Task<bool> ValidateClubRegisterInformation(ClubDTO clubInformation)
        {
            // Check for a valid campus
            if (await campusRepo.GetById(clubInformation.CampusId) == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Campus information not found",
                    "Campus_Not_Found",
                    "Invalid",
                    "Campus information not found",
                    "Campus data not founbd for the provided id",
                    clubInformation.CampusId);
                return false!;
            }
            
            // check for any other club with "same name" in the same campus
            if (await clubRepo.FindFirstMatch(x => x.CampusId == clubInformation.CampusId && x.Name.ToLower() == clubInformation.Name.ToLower()) != null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Already existed a club with the same name",
                    "Club_Existed",
                    "Invalid",
                    "Club already existed.",
                    "This campus already has a club existed wiuth the same name.",
                    clubInformation.Name);
                return false!;
            }

            return true;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    clubRepo.Dispose();
                    campusRepo.Dispose();
                    accountRepo.Dispose();
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
