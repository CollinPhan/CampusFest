using Backend.Cores.DTO;
using Backend.Cores.Entities;
using System.Linq.Expressions;

namespace Backend.API.Services.Interface
{
    public interface IEventService: IDisposable
    {
        Task<IEnumerable<EventDTO>> GetPaginated(int page, int page_size, string keyword = "", int? campus = null, Guid? club = null, string orderBy = "", bool isAcending = false);

        Task<IEnumerable<EventDTO>> GetPaginated(int page, int page_size, Expression<Func<EventDTO, bool>> predicate);

        Task<EventDTO?> GetInformation(Guid eventId);

        Task<Guid?> CreateEvent(EventDTO eventInformation);

        Task<Guid?> UpdateEvent(EventDTO eventInformation);

        Task<IEnumerable<EventRegisterDTO>> GetEventRegisteredList(Guid eventId);

        Task<Guid?> DeleteEvent(Guid eventId);

        Task<bool> ValidateEventInformation(EventDTO eventInformation);
    }
}
