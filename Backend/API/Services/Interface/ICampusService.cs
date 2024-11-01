using Backend.Cores.DTO;

namespace Backend.API.Services.Interface
{
    public interface ICampusService: IDisposable
    {
        Task<CampusDTO?> GetCampus(int campusId);
        Task<IEnumerable<CampusDTO>> GetCampusPaginated(int page, int page_size, string keyword = "");
        Task AddCampus(CampusDTO campusInfo);
        Task UpdateCampus(CampusDTO campusInfo);
        Task DeleteCampus(int campusId);
    }
}
