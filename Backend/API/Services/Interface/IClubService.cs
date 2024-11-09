using Backend.Cores.DTO;

namespace Backend.API.Services.Interface
{
    public interface IClubService: IDisposable
    {
        Task<ClubDTO?> GetClub(Guid id);
        
        Task<IEnumerable<ClubDTO>> GetPaginated(int page = 1, int page_size = 10, string keyword = "", string orderBy = "", bool includeInactivated = false, bool includeDeleted = false);

        Task<Guid?> AddNewClub(ClubDTO clubInformation);

        Task<ClubDTO> UpdateClub(ClubDTO clubInformation);

        Task<Guid?> DeleteClub(Guid id);

        Task<bool> ValidateClubRegisterInformation(ClubDTO clubInformation);
    }
}
